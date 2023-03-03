using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float jumpForce = 5f;

    [SerializeField] float fallMultiplier = 3f;
    [SerializeField] float lowJumpMultiplier = 2.5f;
    Rigidbody2D rb2d;

    bool isJumping = false;
    bool isOnGround = false;

    [Header("Collision")]
    [SerializeField] float collisionRadius = 1f;


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        collision_check();

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }

        check_velocity();

        Vector2 inputVec = new Vector2(x, y);
        move(inputVec);
    }

    private void collision_check()
    {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        isOnGround = Physics2D.OverlapCircle(transform.position, collisionRadius, groundLayer);
        if (isOnGround)
        {
            isJumping = false;
        }
    }

    private void check_velocity()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb2d.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }
    }

    private void move(Vector2 input)
    {
        rb2d.velocity = new Vector2(input.x * movementSpeed, rb2d.velocity.y);
    }

    private void jump()
    {
        if (!isJumping && isOnGround)
        {
            rb2d.velocity += Vector2.up * jumpForce;
            isJumping = true;
        }
    }

}
