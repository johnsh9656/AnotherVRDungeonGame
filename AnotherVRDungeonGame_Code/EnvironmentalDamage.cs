using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalDamage : MonoBehaviour
{
    [SerializeField] float damagePerSecond = 5;

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerHealth>())
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damagePerSecond * Time.deltaTime);
        }
        else if (other.GetComponentInParent<EnemyHealth>())
        {
            other.GetComponentInParent<EnemyHealth>().TakeDamage(damagePerSecond * Time.deltaTime, true);
        }
    }
}
