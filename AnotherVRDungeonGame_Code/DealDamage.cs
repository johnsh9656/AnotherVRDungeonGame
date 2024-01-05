using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] public float damage = 10;
    [SerializeField] private float destroyTime = 0;
    [SerializeField] private float lifetime = 10;
    [SerializeField] private bool destroyWhole = false;
    //public EnemyHealth host;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Shield" && GetComponent<Projectile>() || collision.gameObject.layer == 17)
        {
            Destroy(gameObject);
            return;
        } 

        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if (player)
        {
            player.TakeDamage(damage);
        } 

        if (destroyWhole)
        {
            Destroy(gameObject);
        }
        if (destroyTime != -1)
        {
            MagicGroundAttack mga = GetComponentInParent<MagicGroundAttack>();
            if (mga)
            {
                Destroy(mga);
                Destroy(this);
            } else
            {
                Destroy(gameObject.GetComponent<Collider>(), destroyTime);
            }
        }
    }
}
