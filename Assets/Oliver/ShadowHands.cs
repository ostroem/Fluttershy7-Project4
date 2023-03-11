using System;
using UnityEngine;
using UnityEngine.Events;

public class ShadowHands : Enemy
{
    private Action attackEvent;
    [SerializeField] public float attackDelay = 1.5f;
    [SerializeField] protected float visionRadius = 2f;
    [SerializeField] protected float attackRadius = 1f;

    enum State {
        Idle,
        Attack
    }

    private float idleUpdateFreq = 0.25f;
    private float elapsedTime = 0f;
    State state = State.Idle;
    Vector2 playerPos;
    Vector2 facingDirection;
    Player player;
    
    [SerializeField]protected Sprite[] leftSprites;
    [SerializeField]protected Sprite[] rightSprites;
    private SpriteRenderer spriteRenderer;
    private int animationSpriteIndex;
    private float animationUpdateRate = 0.32f;
    private float elapsedAnimationUpdateRate = 0f;
    private Animator animator;


/// <summary>
/// Awake is called when the script instance is being loaded.
/// </summary>
void Awake()
{
    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();
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
        elapsedAnimationUpdateRate += Time.deltaTime;
        elapsedTime += Time.deltaTime;
        switch(state){
            case State.Idle:
            if(elapsedTime >= idleUpdateFreq){
                if(sense_player(visionRadius)){
                    state = State.Attack;
                }
                elapsedTime = 0f;
            }
            break;
            case State.Attack:
            animator.SetTrigger("attacking");
            if(elapsedTime >= attackDelay){
                if(sense_player(attackRadius)){
                    attackEvent();
                }
                state = State.Idle;
                elapsedTime = 0f;
            }
            break;
        }
        update_sprites();
    }

    private bool sense_player(float radius){
        LayerMask playerMask = LayerMask.GetMask("Player");
        Collider2D player = Physics2D.OverlapCircle((Vector2)transform.position + facingDirection, radius, playerMask);
        if(player){
            playerPos = player.transform.position;
            update_facing_direction();
            return true;
        }
        playerPos = transform.position;
        return false;
    }

    private void update_facing_direction() {
        facingDirection.x = playerPos.x - transform.position.x;
        facingDirection.Normalize();
        animator.SetFloat("directionX", facingDirection.x);
    }
    private void attack() {
        player.take_damage(1);
    }

    private void update_sprites() {
        if(elapsedAnimationUpdateRate < animationUpdateRate){
            return;
        }

        if(facingDirection == Vector2.left){
            if(state != State.Attack){
                spriteRenderer.sprite = leftSprites[0];
                elapsedAnimationUpdateRate = 0f;
                return;
            }
/*             spriteRenderer.sprite = leftSprites[animationSpriteIndex++];
            if(animationSpriteIndex >= leftSprites.Length) {
                animationSpriteIndex = 0;
                elapsedAnimationUpdateRate = 0f;
                return;
            } */
        }
        else if(facingDirection == Vector2.right){
            if(state != State.Attack){
                spriteRenderer.sprite = rightSprites[0];
                elapsedAnimationUpdateRate = 0f;
                return;
            }
/*             spriteRenderer.sprite = rightSprites[animationSpriteIndex++];
            if(animationSpriteIndex >= rightSprites.Length){
                animationSpriteIndex = 0;
                elapsedAnimationUpdateRate = 0f;
                return;
            } */
        }
    }

    void OnDrawGizmosSelected()
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
}
