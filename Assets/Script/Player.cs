using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float jumpForce;
    public float speed;

    public bool isJump;
    public bool isSlide;

    public Rigidbody2D rigid;
    public BoxCollider2D collider;
    public SpriteRenderer renderer;
    public Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Jump();
        Slide();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        transform.position = new Vector3(transform.position.x + speed * Time.smoothDeltaTime, transform.position.y, transform.position.z);
    }

    void Jump()
    {
        bool jumpDown = Input.GetKeyDown(KeyCode.Space);

        if (jumpDown && !isSlide && !isJump)
        {
            isJump = true;
            anim.SetBool("IsJump", true);
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void Slide()
    {
        bool slideDown = Input.GetKey(KeyCode.C);

        if (!isJump)
        {
            isSlide = slideDown;
            anim.SetBool("IsSlide", slideDown);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
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
}
