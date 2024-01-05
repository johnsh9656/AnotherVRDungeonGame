using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    Vector3 startPosition;
    [SerializeField] bool kill;

    private void Awake()
    {
        startPosition = FindObjectOfType<PlayerHealth>().transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        //player
        if (other.gameObject.layer == 10)
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health)
            {
                health.TakeDamage(10);
                health.GetComponent<CharacterControllerGravity>().ResetForTeleport();
                //health.transform.position = startPosition + new Vector3(0, 0, 2);
                health.transform.localPosition = Vector3.zero;
                health.transform.localRotation = Quaternion.identity;

                if (kill) { health.TakeDamage(500); }
            }
        }
        else if (other.gameObject.layer == 11) // enemy
        {
            EnemyHealth health = other.GetComponentInParent<EnemyHealth>();
            if (health) { health.Die(); }
        }
        
    }
}
