using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum AnimalType { Dog, Cat};
    public AnimalType animalType;
    public float jumpForce;
    public float speed;
    public float startSpeed;
    public float acceleration;
    public int maxHealth;
    public int curHealth;
    public float score;

    public bool isJump;
    public bool isSlide;

    public BoxCollider2D[] runCollider;
    public PolygonCollider2D[] jumpCollider;
    public BoxCollider2D slideCollider;
    public GameManager gameManager;

    Rigidbody2D rigid;
    SpriteRenderer renderer;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        speed = startSpeed;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        speed += acceleration * Time.smoothDeltaTime;
        score += speed * Time.smoothDeltaTime;
        transform.position = new Vector3(transform.position.x + speed * Time.smoothDeltaTime, transform.position.y, transform.position.z);
    }

    void Update()
    {
        KeyDown();
        Change();
    }

    void KeyDown()
    {
        if (animalType == AnimalType.Dog)
            Jump();
        else
            Slide();
    }

    void Jump()
    {
        bool jumpDown = Input.GetKeyDown(KeyCode.Space);

        if (jumpDown && !isJump)
        {
            isJump = true;
            runCollider[0].enabled = false;
            jumpCollider[0].enabled = true;
            anim.SetBool("IsJump", true);
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void JumpAnim()
    {
        jumpCollider[0].enabled = false;
        jumpCollider[1].enabled = true;
    }

    

    void Slide()
    {
        bool slideDown = Input.GetKey(KeyCode.Space);

        isSlide = slideDown; 
        anim.SetBool("IsSlide", slideDown);
        runCollider[1].enabled = !slideDown;
        slideCollider.enabled = slideDown;
    }

    void Change()
    {
        bool changeDown = Input.GetKeyDown(KeyCode.LeftControl);

        if (changeDown && !isJump && !isSlide)
        {
            if (animalType == AnimalType.Dog)
            {
                animalType = AnimalType.Cat;
                runCollider[0].enabled = false;
                runCollider[1].enabled = true;
                anim.SetBool("IsChange", true);
            }
            else if (animalType == AnimalType.Cat)
            {
                animalType = AnimalType.Dog;
                runCollider[1].enabled = false;
                runCollider[0].enabled = true;
                anim.SetBool("IsChange", false);
            }
        }
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
            if (curHealth == 0)
            {
                Die();
                return;
            }
            StartCoroutine(Damage());
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

    }
}
