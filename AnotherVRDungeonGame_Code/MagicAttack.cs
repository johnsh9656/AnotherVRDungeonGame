using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/MagicAttack")]
public class MagicAttack : ScriptableObject
{
    public string animName;
    public GameObject attackPrefab;
    public float attackRange;
    public float attackCooldown;
    public bool isGrounded;
    public bool rotateFollowPlayer = false;
}
