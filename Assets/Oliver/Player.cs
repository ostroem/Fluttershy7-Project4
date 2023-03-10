using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float jumpForce = 6f;
    [SerializeField] Vector2 facingDirection = Vector2.left;
    private float moveAcceleration = 3f;
    private float moveDeacceleration = 7f;
    private float currentMoveVelocity = 0f;
    private float maxMoveVelocity = 4f;

    [SerializeField] protected int energyValue = 50;
    [SerializeField] protected int incrementalEnergy = 1;
    [SerializeField] protected int decrementalEnergy = 10;

    private CircleCollider2D energyCollider;
    [SerializeField] protected ParticleSystem energyParticles;
    ParticleSystem.VelocityOverLifetimeModule energyParticleVelocity;

    Vector2 inputVec = Vector2.zero;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    private Rigidbody2D rb2d;
    

    bool isJumping = false;
    bool isOnGround = false;


    [Header("Combat")]
    [SerializeField] float attackRadius = 0.5f;
    public int hitpoints = 3;

    [Header("Collision")]
    [SerializeField] float collisionRadius = 1f;

    [SerializeField] protected UnityEvent attackedEnemy;


    private void Awake(){
        rb2d = GetComponent<Rigidbody2D>();
        energyCollider = gameObject.AddComponent<CircleCollider2D>();
        energyCollider.radius = collisionRadius;
        energyCollider.isTrigger = true;
        energyParticleVelocity = energyParticles.velocityOverLifetime;
        DontDestroyOnLoad(gameObject);
    }

    private void Update(){
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

        if (Input.GetMouseButtonUp(0) && energyValue > 0)
        {
            attack();
            energyValue -= decrementalEnergy;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }
        take_energy();
        if(prev_face_dir != facingDirection){
            currentMoveVelocity = 1f;
        }
        CalculateSpeed(inputVec);

    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate(){
        check_velocity();
        move();
        collision_check();
    }

    private Vector2 adjust_facing_direction(Vector2 vector){
        Vector2 face_dir = vector;
        face_dir.Normalize();
        face_dir.x = Mathf.Clamp(face_dir.x, -1f, 1f);
        face_dir.y = Mathf.Clamp(facingDirection.y, -1f, 1f);
        return face_dir;
    }

    private void collision_check(){
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        isOnGround = Physics2D.OverlapCircle(transform.position, collisionRadius, groundLayer);
        if (isOnGround)
        {
            isJumping = false;
        }
    }
    private void attack(){
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D enemy = Physics2D.OverlapCircle((Vector2)transform.position + facingDirection, attackRadius, enemyLayer);
        if (enemy)
        {
            enemy.GetComponent<Enemy>().take_damage(1);
            Debug.Log("damage");
        }
    }

    private void take_energy() {
        // when enemy is near energyCollider
        // we gain energy
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");


        if(energyCollider.IsTouchingLayers(enemyLayer)){
            Collider2D enemy = Physics2D.OverlapCircle(transform.position, collisionRadius, enemyLayer);
            Vector2 direction = enemy.transform.position - transform.position;
            Debug.Log("direction " + direction);
            //direction.Normalize();
            energyParticleVelocity.x = direction.x;
            energyParticleVelocity.y = direction.y;
            // set particle effect direction to be this direction
            

            Debug.Log("touching enemy layer");
            energyValue += incrementalEnergy;
        }
        else {
            energyParticleVelocity.x = 0;
            energyParticleVelocity.y = 0;
        }
        // else
        // nothing


    }

    private void check_velocity(){
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb2d.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }
    }

    private void move() {
        rb2d.velocity = new Vector2(facingDirection.x * currentMoveVelocity, rb2d.velocity.y);
    }

    private void jump(){
        if (!isJumping && isOnGround)
        {
            rb2d.velocity += Vector2.up * jumpForce;
            isJumping = true;
        }
    }
    private void CalculateSpeed(Vector2 vec) {
        if(Mathf.Abs(vec.x) > 0){
            currentMoveVelocity += moveAcceleration * Time.deltaTime;
        }
        else {
            currentMoveVelocity -= moveDeacceleration * Time.deltaTime;
        }
        currentMoveVelocity = Mathf.Clamp(currentMoveVelocity, 0, maxMoveVelocity);
    }
    public void take_damage(int damage){
        hitpoints -= damage;
    }
    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected(){
        Gizmos.DrawSphere((Vector2)transform.position + facingDirection, attackRadius);
    }
}
