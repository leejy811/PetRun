using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    //private 플레이어 타입
    private enum AnimalType { Dog, Cat };
    private AnimalType animalType;

    //public float형 물리, 체력, 점수 관련
    public float speed;
    public float score;
    public float curHealth;
    public readonly float maxHealth = 100;

    //private float형 물리, 체력, 점수 관련
    private readonly float maxSpeed = 30;
    private readonly float startSpeed = 3;
    private readonly float accel = 0.2f;
    private float jumpPower;

    //public 플래그 변수
    public bool isFall;
    public bool isHighScore;

    //private 플래그 변수
    private bool isJump;
    private bool isSlide;
    private bool isDead;
    private bool isMaxSpeed;

    //SerializeField된 컴포넌트 변수 
    [SerializeField]  BoxCollider2D[] runCollider;
    [SerializeField]  PolygonCollider2D[] jumpCollider;
    [SerializeField]  BoxCollider2D slideCollider;
    [SerializeField] GameManager gameManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] GameObject[] particle;

    //오디오 관련 변수
    [SerializeField] AudioClip catSound;
    [SerializeField] AudioClip dogSound;
    AudioSource audioSource;

    //컴포넌트 변수
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    
    //코루틴 변수
    Coroutine curParticleCoroutine;
    Coroutine curDamageCoroutine;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        speed = startSpeed;
        curHealth = maxHealth;
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
            PhysicsSet();
            MaxSpeedCheck();

            if (!isDead)
            {
                score += speed * Time.fixedDeltaTime * 5;

                calculateHealth();
            }
        }

        transform.position = new Vector3(transform.position.x + speed * Time.fixedDeltaTime, transform.position.y, transform.position.z);
    }

    void PhysicsSet()
    {
        if (!isMaxSpeed)
        {
            speed += accel * Time.fixedDeltaTime;
            Physics2D.gravity = new Vector2(0, -0.284f * speed * speed);
            jumpPower = 3.5f * Mathf.Sqrt(Physics2D.gravity.y * -1f);
        }
    }

    void MaxSpeedCheck()
    {
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
            isMaxSpeed = true;
        }
    }

    void calculateHealth()
    {
        curHealth -= speed * Time.fixedDeltaTime / 20;
        if (curHealth <= 0)
        {
            curHealth = 0;
            Die();
        }
    }

    void Update()
    {
        GroundCheck();
        JumpCheck();

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
            anim.SetFloat("JumpSpeed", (jumpPower / Physics.gravity.y) * -1f);
            anim.SetBool("IsJump", true);
            rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
        }
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

    void GroundCheck()
    {
        if (isJump && jumpCollider[1].enabled && transform.position.y < -2)
        {
            StartCoroutine(JumpFalse());
            runCollider[0].enabled = true;
            jumpCollider[1].enabled = false;
            anim.SetBool("IsJump", false);
        }
    }

    IEnumerator JumpFalse()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        isJump = false;
    }

    void JumpCheck()
    {
        if (isJump && jumpCollider[0].enabled && transform.position.y > 2.5f)
        {
            jumpCollider[1].enabled = true;
            jumpCollider[0].enabled = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "TileMap")
        {
            if (transform.position.y < -2.9)
                transform.position = new Vector3(transform.position.x, -2.285f, transform.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
            return;

        if (other.gameObject.tag == "Obstacle")
        {

            curHealth -= 1;
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

            switch (item.itemType)
            {
                case "Bone":
                    if (animalType == AnimalType.Dog)
                    {
                        item.PlaySound();
                        Heal(15);
                    }
                    else
                        Heal(3);
                    break;
                case "Chur":
                    if (animalType == AnimalType.Cat)
                    {
                        item.PlaySound();
                        Heal(15);
                    }
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
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
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