using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int hitpoints = 3;
    protected int attackDamage = 1;

    public void take_damage(int incoming_damage) {
        hitpoints -= incoming_damage;
    }


}
