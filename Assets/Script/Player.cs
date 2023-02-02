using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum AnimalType { Dog, Cat };
    public AnimalType animalType;
    public float jumpPower;
    public float speed;
    public float startSpeed;
    public float acceleration;
    public float maxHealth;
    public float curHealth;
    public float score;

    public bool isJump;
    public bool isSlide;
    public bool isDead;
    public bool isFall;
    public bool isHighScore;

    public BoxCollider2D[] runCollider;
    public PolygonCollider2D[] jumpCollider;
    public BoxCollider2D slideCollider;
    public GameManager gameManager;
    public UIManager uiManager;
    public GameObject[] particle;

    public AudioClip jumpSound;
    public AudioClip catSound;
    public AudioClip dogSound;
    public AudioClip getItemSound;

    Rigidbody2D rigid;
    SpriteRenderer renderer;
    Animator anim;
    AudioSource audioSource;
    Coroutine curParticleCoroutine;
    Coroutine curDamageCoroutine;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        speed = startSpeed;
        curHealth = maxHealth;

        //PlayerPrefs.SetFloat("HighScore", 0);
    }

    void FixedUpdate()
    {
        if (isFall)
            return;

        Move();
    }

    void Move()
    {
        if (gameManager.isStart) 
        {
            speed += acceleration * Time.smoothDeltaTime;
            Physics2D.gravity = new Vector2(0, -0.284f * speed * speed);
            jumpPower = 3.5f * Mathf.Sqrt(Physics2D.gravity.y * -1f);
            anim.SetFloat("JumpSpeed", (jumpPower / Physics.gravity.y) * -1f);

            if (!isDead)
            {
                score += speed * Time.smoothDeltaTime;
                curHealth -= speed * Time.smoothDeltaTime / 10;
                if (curHealth <= 0)
                {
                    curHealth = 0;
                    Die();
                }
            }
        }

        transform.position = new Vector3(transform.position.x + speed * Time.smoothDeltaTime, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (isDead)
            return;

        KeyDown();
        Change(false);
    }

    void KeyDown()
    {
        if (animalType == AnimalType.Dog)
            Jump(false);
        else
            Slide();
    }

    public void Jump(bool isButtonDown)
    {
        bool jumpDown = Input.GetKeyDown(KeyCode.Space) || isButtonDown;


        if (jumpDown && !isJump && gameManager.isStart)
        {
            isJump = true;
            jumpCollider[0].enabled = true;
            runCollider[0].enabled = false;
            anim.SetBool("IsJump", true);
            rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
            PlaySound("Jump");
        }
    }

    public void JumpAnim()
    {
        jumpCollider[1].enabled = true;
        jumpCollider[0].enabled = false;
    }



    void Slide()
    {
        bool slideDown = (Input.GetKey(KeyCode.Space) || uiManager.isSlideButtonDown) && gameManager.isStart;

        isSlide = slideDown;
        anim.SetBool("IsSlide", slideDown);
        slideCollider.enabled = slideDown;
        runCollider[1].enabled = !slideDown;
    }

    public void Change(bool isButtonDown)
    {
        bool changeDown = Input.GetKeyDown(KeyCode.LeftControl) || isButtonDown;

        if (changeDown && !isJump && !isSlide && gameManager.isStart)
        {
            if (curParticleCoroutine != null)
                StopCoroutine(curParticleCoroutine);

            if (!isButtonDown)
                uiManager.ChangeButton();

            if (animalType == AnimalType.Dog)
            {
                curParticleCoroutine = StartCoroutine(ChangeParticle(1));
                animalType = AnimalType.Cat;
                runCollider[1].enabled = true;
                runCollider[0].enabled = false;
                anim.SetBool("IsChange", true);
                PlaySound("CatChange");
            }
            else if (animalType == AnimalType.Cat)
            {
                curParticleCoroutine = StartCoroutine(ChangeParticle(0));
                animalType = AnimalType.Dog;
                runCollider[0].enabled = true;
                runCollider[1].enabled = false;
                anim.SetBool("IsChange", false);
                PlaySound("DogChange");
            }
        }
    }

    IEnumerator ChangeParticle(int index)
    {
        int preIndex = index == 0 ? 1 : 0;

        if (particle[preIndex].activeSelf == true)
            particle[preIndex].SetActive(false);

        particle[index].SetActive(true);
        yield return new WaitForSeconds(1f);
        particle[index].SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "TileMap")
        {
            if (isJump && jumpCollider[1].enabled)
            {
                isJump = false;
                runCollider[0].enabled = true;
                jumpCollider[1].enabled = false;
                anim.SetBool("IsJump", false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
            return;

        if (other.gameObject.tag == "Obstacle")
        {

            curHealth -= 30;
            BoxCollider2D obstacleColider = other.gameObject.GetComponent<BoxCollider2D>();
            obstacleColider.enabled = false;
            if (curHealth <= 0)
            {
                curHealth = 0;
                return;
            }
            if (curDamageCoroutine != null)
                StopCoroutine(curDamageCoroutine);

            curDamageCoroutine = StartCoroutine(Damage());
        }
        else if (other.gameObject.tag == "Item")
        {

            Item item = other.gameObject.GetComponent<Item>();
            item.PlaySound();

            switch (item.itemType)
            {
                case "Bone":
                    if (animalType == AnimalType.Dog)
                        Heal(15);
                    else
                        Heal(3);
                    break;
                case "Chur":
                    if (animalType == AnimalType.Cat)
                        Heal(15);
                    else
                        Heal(3);
                    break;
            }

            gameManager.ReturnPool(item);
        }
    }

    void Heal (float healAmount)
    {
        if (curHealth + healAmount > maxHealth)
            curHealth = maxHealth;
        else
            curHealth += healAmount;
    }

    IEnumerator Damage()
    {
        renderer.color = Color.gray;
        yield return new WaitForSeconds(0.5f);
        renderer.color = Color.white;
    }

    void Die()
    {
        isDead = true;

        if (PlayerPrefs.GetFloat("HighScore") < score)
        {
            isHighScore = true;
            PlayerPrefs.SetFloat("HighScore", score);
        }
    }

    public void FallAnimation()
    {
        if (isDead)
        {
            isFall = true;

            if (animalType == AnimalType.Dog)
                anim.SetTrigger("doDogFall");
            else if (animalType == AnimalType.Cat)
                anim.SetTrigger("doCatFall");

            //게임 오버 UI작동
            uiManager.GameOver();
        }
    }

    public void RestAnimation()
    {
        if (animalType == AnimalType.Dog)
            anim.SetTrigger("doDogRest");
        else if (animalType == AnimalType.Cat)
            anim.SetTrigger("doCatRest");
    }

    void PlaySound(string soundType)
    {
        switch(soundType)
        {
            case "Jump":
                audioSource.clip = jumpSound;
                break;
            case "CatChange":
                audioSource.clip = catSound;
                break;
            case "DogChange":
                audioSource.clip = dogSound;
                break;
        }

        audioSource.Play();
    }
}