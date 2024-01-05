using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float damageMultiplier = 1f;
    Collider col;
    EnemyAI ai;

    private void Awake()
    {
        col = GetComponent<Collider>();
        ai = GetComponentInParent<EnemyAI>();
    }

    public void SetDamage(float d)
    {
        damage = d;
    }

    public void SetCollider(bool b)
    {
        col.enabled = b;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shield")
        {
            ai.AttackParried();
            return;
        } else if (other.gameObject.layer == 9) // weapon layer
        {
            if (other.GetComponentInParent<Weapon>().IsFast())
            {
                ai.AttackParried();
                return;
            }
        }

        PlayerHealth player = null;
        if (other.gameObject.GetComponent<PlayerHealth>())
        {
            player = other.gameObject.GetComponent<PlayerHealth>();
        }
        /*else if (other.gameObject.GetComponentInParent<PlayerHealth>())
        {
            player = other.gameObject.GetComponentInParent<PlayerHealth>();
        }*/

        if (player)
        {
            player.TakeDamage(damage * damageMultiplier);
        }
    }
}
