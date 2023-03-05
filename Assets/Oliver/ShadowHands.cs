using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadowHands : Enemy
{
    [SerializeField] protected UnityEvent attackEvent;
    [SerializeField] public float attackDelay = 1.5f;
    enum State {
        Idle,
        Attack
    }

    private float idleUpdateFreq = 0.25f;
    private float elapsedTime = 0f;
    [SerializeField] protected float collisionRadius = 1f;
    State state;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(hitpoints <= 0){
            Destroy(this.gameObject);
        }
        elapsedTime += Time.deltaTime;
        switch(state){
            case State.Idle:
            if(elapsedTime >= idleUpdateFreq){
                if(sense_player()){
                    state = State.Attack;
                }
                elapsedTime = 0f;
            }
            break;
            case State.Attack:
            if(elapsedTime >= attackDelay){
                if(sense_player()){
                    attackEvent.Invoke();
                }
                state = State.Idle;
            }
            break;
        }
    }

    private bool sense_player(){
        LayerMask playerMask = LayerMask.GetMask("Player");
        if(Physics2D.OverlapCircle(transform.position, collisionRadius, playerMask)){
            return true;
        }
        return false;
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        if(state == State.Attack){
            Gizmos.color = Color.red;
        }
        else {
            Gizmos.color = Color.cyan;
        }
        Gizmos.DrawSphere(transform.position, collisionRadius);

    }
}
