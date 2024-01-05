using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/MeleeAttack")]
public class MeleeAttack : ScriptableObject
{
    public string animName;
    public bool canMove;
    public float damage;
    public bool heavy;
    public float heavyCooldown;
}
