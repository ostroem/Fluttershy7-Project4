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

    private Vector2 inputVec = Vector2.zero;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    private bool isJumping = false;
    private bool isOnGround = false;
    private Rigidbody2D rb2d;


    [Header("Combat")]
    [SerializeField] float attackRadius = 0.5f;
    private float energyValue = 50f;
    private float maxEnergy = 100f;
    private float incrementalEnergy = 0.1f;
    private float decrementalEnergy = 10.0f;
    [SerializeField] private int hitpoints = 3;
    
    private CircleCollider2D energyCollider;
    [SerializeField] protected ParticleSystem energyParticles;
    ParticleSystem.VelocityOverLifetimeModule energyParticleVelocity;
    [SerializeField] protected ParticleSystem hitParticles;
    ParticleSystem.VelocityOverLifetimeModule hitParticleVelocity;

    [Header("Collision")]
    [SerializeField] float collisionRadius = 1f;
    [SerializeField] float energyRadius = 2f;

    protected UnityEvent attackedEnemy;
    [Header("Sprites and Animation")]
    [SerializeField] protected Sprite[] leftWalkSprites;
    [SerializeField] protected Sprite[] rightWalkSprites;
    [SerializeField] protected Sprite[] jumpingSprites;
    [SerializeField] protected Sprite idleSprite;
    [SerializeField] protected GameObject[] heartPieces;
    private int animationSpriteIndex = 0;
    private float animationUpdateRate = 0.12f;
    private float elapsedAnimationUpdateRate = 0f;
    private SpriteRenderer spriteRenderer;
    [SerializeField] protected Image energyImage;
    private int collectedHeartPieces = 0;
    private int maxCollectedHeartPieces = 3;
    [SerializeField] protected AudioSource hitSound;
    [SerializeField] protected AudioSource attackSound;
    [SerializeField] protected GameObject lostPanel;

    enum State {
        Playing,
        Lost,
        Win
    }
    State state;

    private void Awake(){
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        energyCollider = gameObject.AddComponent<CircleCollider2D>();
        energyCollider.radius = energyRadius;
        energyCollider.isTrigger = true;
        energyParticleVelocity = energyParticles.velocityOverLifetime;
        update_energy_bar(energyValue);
        DontDestroyOnLoad(gameObject);
        state = State.Playing;
    }
    
    private void Update(){

        if(hitpoints <= 0){
            state = State.Lost;
        }

        switch(state){
            case State.Playing:
                elapsedAnimationUpdateRate += Time.deltaTime;
                Vector2 prev_face_dir = facingDirection;
                if (Input.GetKey(KeyCode.A)) {
                    inputVec = Vector2.left;
                    facingDirection = adjust_facing_direction(inputVec);
                }
                else if (Input.GetKey(KeyCode.D)){
                    inputVec = Vector2.right;
                    facingDirection = adjust_facing_direction(inputVec);
                }
                else {
                    inputVec = Vector2.zero;
                }

                if (Input.GetMouseButtonUp(0) && energyValue > 0){
                    attack();
                    energyValue -= decrementalEnergy;
                    update_energy_bar(energyValue);
                }
                if (Input.GetKeyDown(KeyCode.Space)){
                    jump();
                }
                if(prev_face_dir != facingDirection){
                    currentMoveVelocity = 1f;
                }
                CalculateSpeed(inputVec);
                take_energy();
                update_player_sprite();
            break;
            case State.Lost:
                lostPanel.SetActive(true);
            break;
            case State.Win:
            break;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("HeartPiece")){
            // display the new heart
            heartPieces[collectedHeartPieces].SetActive(true);
            //increase var for collected heart pieces
            if(collectedHeartPieces < maxCollectedHeartPieces){
                collectedHeartPieces++;
            }
            Destroy(other.collider.gameObject);
        }
    }

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
        if (isOnGround) {
            isJumping = false;
        }
    }
    private void attack(){
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D enemy = Physics2D.OverlapCircle((Vector2)transform.position + facingDirection, attackRadius, enemyLayer);
        if (enemy) {
            enemy.GetComponent<Enemy>().take_damage(1);
            if(facingDirection.x > 0){
            hitParticles.transform.SetLocalPositionAndRotation(new Vector3(0.5f, 0, -1), Quaternion.Euler(0, 0, -45));
            }
            if(facingDirection.x < 0){
                hitParticles.transform.SetLocalPositionAndRotation(new Vector3(-0.5f, 0, -1), Quaternion.Euler(0, 0, -225));
            }
            hitParticles.Play();
            hitSound.Play();
        }
        else {
            attackSound.Play();
        }
    }

    private void take_energy() {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        if(energyCollider.IsTouchingLayers(enemyLayer)){
            Collider2D enemy = Physics2D.OverlapCircle(transform.position, energyRadius, enemyLayer);
            Vector2 direction = enemy.transform.position - transform.position;
            energyParticleVelocity.x = direction.x * 2;
            energyParticleVelocity.y = direction.y * 2;
            
            if(energyValue < maxEnergy){
                energyValue += incrementalEnergy;
            }
            update_energy_bar(energyValue);
        }
        else {
            energyParticleVelocity.x = 0;
            energyParticleVelocity.y = 0;
        }
    }

    private void check_velocity(){
        if (rb2d.velocity.y < 0){
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb2d.velocity.y > 0 && !Input.GetKey(KeyCode.Space)){
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }
    }

    private void move() {
        rb2d.velocity = new Vector2(facingDirection.x * currentMoveVelocity, rb2d.velocity.y);
    }

    private void jump(){
        if (!isJumping && isOnGround){
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

    public void update_player_sprite() {
        if(elapsedAnimationUpdateRate < animationUpdateRate){
            return;
        }
                
        if(inputVec == Vector2.zero){
            spriteRenderer.sprite = idleSprite;
        }
        else if(facingDirection.x < 0){
            if(rb2d.velocity.y > 1) {
                spriteRenderer.sprite = jumpingSprites[0];
                elapsedAnimationUpdateRate = 0f;
                return;
            }

            if(animationSpriteIndex >= leftWalkSprites.Length - 1){
                animationSpriteIndex = 0;
            }
            spriteRenderer.sprite = leftWalkSprites[animationSpriteIndex++];
        }
        else if(facingDirection.x > 0){
            // player right sprite
            if(rb2d.velocity.y > 1) {
                spriteRenderer.sprite = jumpingSprites[1];
                elapsedAnimationUpdateRate = 0f;
                return;
            }
            if(animationSpriteIndex >= rightWalkSprites.Length - 1){
                animationSpriteIndex = 0;
            }
            spriteRenderer.sprite = rightWalkSprites[animationSpriteIndex++];
        }
        elapsedAnimationUpdateRate = 0f;
    }

    private void update_energy_bar(float energy) {
        energyImage.fillAmount = energy / 100f;
    }
}
