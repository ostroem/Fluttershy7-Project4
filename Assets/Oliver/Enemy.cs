using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int hitpoints = 3;
    protected int attackDamage = 1;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    public void attack(){
        Debug.Log(name + "attacked");
    }
    public void take_damage(int incoming_damage) {
        hitpoints -= incoming_damage;
    }


}
