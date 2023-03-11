using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadowGhost : Enemy
{
    private Action attackEvent;
    [SerializeField] protected Vector2 facingDirection = Vector2.left;
    [SerializeField] public float attackDelay = 1.5f;
    [SerializeField] protected float wanderDuration = 1.5f;
    private float currentWanderDuration = 0f;
    enum State {
        Idle,
        Wander,
        Attack
    }

    private float idleUpdateFreq = 0.25f;
    private float elapsedTime = 0f;
    [SerializeField] protected float visionRadius = 1f;
    [SerializeField] protected float attackRadius = 1.5f;
    State state;
    private Vector2 playerPos;
    Player player;

    [SerializeField]protected Sprite[] leftSprites;
    [SerializeField]protected Sprite[] rightSprites;
    [SerializeField]protected Sprite idleSprite;
    private SpriteRenderer spriteRenderer;
    private int animationSpriteIndex;
    private float animationUpdateRate = 0.12f;
    private float elapsedAnimationUpdateRate = 0f;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Player>();
        attackEvent += attack;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    

    void Update()
    {
        if(hitpoints <= 0){
            Destroy(this.gameObject);
        }
        elapsedTime += Time.deltaTime;
        elapsedAnimationUpdateRate += Time.deltaTime;
        switch(state){
            case State.Idle:
            if(elapsedTime >= idleUpdateFreq){
                if(sense_player(visionRadius)){
                    state = State.Attack;
                    return;
                }
                state = State.Wander;
                facingDirection.x = UnityEngine.Random.Range(-1, 2);
                elapsedTime = 0f;
            }
            break;
            case State.Wander:
            currentWanderDuration += Time.deltaTime;
            if(currentWanderDuration >= wanderDuration){
                state = State.Idle;
                currentWanderDuration = 0f;
                return;
            }
            transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + facingDirection, Time.deltaTime * 1f);
            break;
            case State.Attack:

            if(elapsedTime >= attackDelay){
                if(sense_player(attackRadius)){
                    attackEvent();
                }
                state = State.Idle;
                elapsedTime = 0f;
            }
            Vector2 newPos = new Vector2(playerPos.x + facingDirection.x, playerPos.y + 1f);
            transform.position = Vector2.Lerp(transform.position, newPos, Time.deltaTime * 1f);
            break;
        }
        update_sprites();
    }

    private void attack() {
        player.take_damage(1);
    }
    private bool sense_player(float radius){
        LayerMask playerMask = LayerMask.GetMask("Player");
        Collider2D player = Physics2D.OverlapCircle(transform.position, radius, playerMask);
        if(player){
            playerPos = player.transform.position;
            update_facing_direction();
            return true;
        }
        playerPos = transform.position;
        update_facing_direction();
        return false;
    }

    private void update_facing_direction() {
        if(playerPos == (Vector2)transform.position){
            facingDirection.x = UnityEngine.Random.Range(-1, 2);
            return;
        }
        facingDirection.x = playerPos.x - transform.position.x;
        facingDirection.Normalize();
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        if(state == State.Attack){
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, attackRadius);
        }
        else {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, visionRadius);
        }

    }

    private void update_sprites() {
        if(state == State.Idle){
            spriteRenderer.sprite = idleSprite;
        }
        else if(facingDirection == Vector2.left){
            spriteRenderer.sprite = leftSprites[animationSpriteIndex];
            if(animationSpriteIndex >= leftSprites.Length - 1){
                animationSpriteIndex = 0;
                return;
            }
            animationSpriteIndex ++;
        }
        else if(facingDirection == Vector2.right){
            spriteRenderer.sprite = rightSprites[animationSpriteIndex];
            if(animationSpriteIndex >= rightSprites.Length - 1){
                animationSpriteIndex = 0;
                return;
            }
            animationSpriteIndex ++;
        }
        elapsedAnimationUpdateRate = 0f;
    }

}
