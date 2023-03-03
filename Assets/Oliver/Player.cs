using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed = 0f;
    [SerializeField] float jumpForce = 0f;
    [SerializeField] Vector2 facingDirection;

    [SerializeField] float fallMultiplier = 0f;
    [SerializeField] float lowJumpMultiplier = 0f;
    Rigidbody2D rb2d;

    bool isJumping = false;
    bool isOnGround = false;


    [Header("Combat")]
    [SerializeField] float attackRadius = 1f;

    [Header("Collision")]
    [SerializeField] float collisionRadius = 1f;

    


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        collision_check();
        Vector2 inputVec = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            inputVec = Vector2.left;
            adjust_facing_direction(inputVec);
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVec = Vector2.right;
            adjust_facing_direction(inputVec);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            attack();
        }

        check_velocity();
        move(inputVec);
    }

    private void adjust_facing_direction(Vector2 vector)
    {
        facingDirection = vector;
        facingDirection.Normalize();
        facingDirection.x = Mathf.Clamp(facingDirection.x, -1, 1);
        facingDirection.y = Mathf.Clamp(facingDirection.y, -1, 1);
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
    private void attack()
    {
        
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D enemy = Physics2D.OverlapCircle((Vector2)transform.position + facingDirection, attackRadius, enemyLayer);
        if (enemy)
        {
            Debug.Log("damage");
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere((Vector2)transform.position + facingDirection, attackRadius);
    }

}
