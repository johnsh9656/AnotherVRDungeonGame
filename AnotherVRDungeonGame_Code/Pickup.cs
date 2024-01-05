using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public bool speed;
    public bool death;
    public bool health;
    public bool mystery;

    GameObject player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerHealth>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mystery)
        {
            switch(Random.Range(0, 3))
            {
                case 0:
                    speed = true;
                    break;
                case 1:
                    death = true;
                    break;
                case 2:
                    health = true;
                    break;
            }
        }

        if (speed)
        {
            player.GetComponent<CharacterControllerGravity>().SpeedPickup(2, 5);
            player.GetComponent<HandleHookshot>().SpeedPickup(2, 10);
        } else if (death)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f);
            foreach (var hitCollider in hitColliders)
            {
                EnemyHealth enemy = hitCollider.GetComponentInParent<EnemyHealth>();
                if (enemy)
                {
                    enemy.TakeDamage(100, true);
                }
            }
        } else if (health)
        {
            player.GetComponent<PlayerHealth>().Heal(50);
        }

        if (FindObjectOfType<ArenaManager>()) { FindObjectOfType<ArenaManager>().OnPickup(transform.parent.transform); }
        Destroy(gameObject);
    }
}
