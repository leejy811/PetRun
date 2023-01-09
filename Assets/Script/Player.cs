using System.Collections;
using System.Collections.Generic;
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

    public BoxCollider2D runCollider;
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

    void Update()
    {
        Jump();
        Slide();
        Change();
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

    void Jump()
    {
        bool jumpDown = Input.GetKeyDown(KeyCode.Space);

        if (jumpDown && !isSlide && !isJump && animalType == AnimalType.Dog)
        {
            isJump = true;
            anim.SetBool("IsJump", true);
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void Slide()
    {
        bool slideDown = Input.GetKey(KeyCode.C);

        if (!isJump && animalType == AnimalType.Cat)
        {
            isSlide = slideDown;
            anim.SetBool("IsSlide", slideDown);
            runCollider.enabled = !slideDown;
            slideCollider.enabled = slideDown;
        }
    }

    void Change()
    {
        bool changeDown = Input.GetKeyDown(KeyCode.LeftControl);

        if (changeDown && !isJump && !isSlide)
        {
            if (animalType == AnimalType.Dog)
            {
                animalType = AnimalType.Cat;
                anim.SetBool("IsChange", true);
            }
            else if (animalType == AnimalType.Cat)
            {
                animalType = AnimalType.Dog;
                anim.SetBool("IsChange", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "TileMap")
        {
            if (isJump)
            {
                isJump = false;
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
