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
    public int maxHealth;
    public int curHealth;
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

    Rigidbody2D rigid;
    SpriteRenderer renderer;
    Animator anim;
    Coroutine curParticleCoroutine;
    Coroutine curDamageCoroutine;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        speed = startSpeed;
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
                score += speed * Time.smoothDeltaTime;
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
            runCollider[0].enabled = false;
            jumpCollider[0].enabled = true;
            anim.SetBool("IsJump", true);
            rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
        }
    }

    public void JumpAnim()
    {
        jumpCollider[0].enabled = false;
        jumpCollider[1].enabled = true;
    }



    void Slide()
    {
        bool slideDown = (Input.GetKey(KeyCode.Space) || uiManager.isSlideButtonDown) && gameManager.isStart;

        isSlide = slideDown;
        anim.SetBool("IsSlide", slideDown);
        runCollider[1].enabled = !slideDown;
        slideCollider.enabled = slideDown;
    }

    public void Change(bool isButtonDown)
    {
        bool changeDown = Input.GetKeyDown(KeyCode.LeftControl) || isButtonDown;

        if (changeDown && !isJump && !isSlide && gameManager.isStart)
        {
            if (curParticleCoroutine != null)
                StopCoroutine(curParticleCoroutine);

            if (animalType == AnimalType.Dog)
            {
                curParticleCoroutine = StartCoroutine(ChangeParticle(1));
                animalType = AnimalType.Cat;
                runCollider[0].enabled = false;
                runCollider[1].enabled = true;
                anim.SetBool("IsChange", true);
            }
            else if (animalType == AnimalType.Cat)
            {
                curParticleCoroutine = StartCoroutine(ChangeParticle(0));
                animalType = AnimalType.Dog;
                runCollider[1].enabled = false;
                runCollider[0].enabled = true;
                anim.SetBool("IsChange", false);
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
                jumpCollider[1].enabled = false;
                runCollider[0].enabled = true;
                anim.SetBool("IsJump", false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            curHealth -= 1;
            BoxCollider2D obstacleColider = other.gameObject.GetComponent<BoxCollider2D>();
            obstacleColider.enabled = false;
            if (curHealth == 0)
            {
                Die();
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
                        score += 500;
                    else
                        score += 100;
                    break;
                case "Chur":
                    if (animalType == AnimalType.Cat)
                        score += 500;
                    else
                        score += 100;
                    break;
                case "Heart":
                    curHealth += 1;
                    if (curHealth > maxHealth)
                        curHealth = maxHealth;
                    break;
            }

            gameManager.ReturnPool(item);
        }
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
}