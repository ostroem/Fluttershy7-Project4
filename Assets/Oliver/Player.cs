using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed = 0f;
    [SerializeField] float jumpForce = 0f;
    [SerializeField] Vector2 facingDirection = Vector2.left;

    Vector2 inputVec = Vector2.zero;
    [SerializeField] float fallMultiplier = 0f;
    [SerializeField] float lowJumpMultiplier = 0f;
    Rigidbody2D rb2d;
    
    private float movementAcceleration = 5f;
    private float movementDeacceleration = 8f;
    private float currentVelocity = 0f;
    private float maxVelocity = 5.0f;

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
        Vector2 prev_face_dir = facingDirection;
        if (Input.GetKey(KeyCode.A))
        {
            inputVec = Vector2.left;
            facingDirection = adjust_facing_direction(inputVec);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            inputVec = Vector2.right;
            facingDirection = adjust_facing_direction(inputVec);
        }
        else {
            inputVec = Vector2.zero;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            attack();
        }
        if(prev_face_dir != facingDirection){
            currentVelocity = 1f;
        }
        check_velocity();
        CalculateSpeed(inputVec);
        move();
    }

    private Vector2 adjust_facing_direction(Vector2 vector)
    {
        Vector2 face_dir = vector;
        face_dir.Normalize();
        face_dir.x = Mathf.Clamp(face_dir.x, -1, 1);
        face_dir.y = Mathf.Clamp(facingDirection.y, -1, 1);
        return face_dir;
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

    private void move()
    {
        rb2d.velocity = new Vector2(facingDirection.x * currentVelocity, rb2d.velocity.y);
    }

    private void jump()
    {
        if (!isJumping && isOnGround)
        {
            rb2d.velocity += Vector2.up * jumpForce;
            isJumping = true;
        }
    }
    private void CalculateSpeed(Vector2 vec) {
        if(Mathf.Abs(vec.x) > 0){
            currentVelocity += movementAcceleration * Time.deltaTime;
        }
        else {
            currentVelocity -= movementDeacceleration * Time.deltaTime;
        }
        currentVelocity = Mathf.Clamp(currentVelocity, 0, maxVelocity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere((Vector2)transform.position + facingDirection, attackRadius);
        
    }
}
