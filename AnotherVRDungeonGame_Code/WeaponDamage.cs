using DynamicMeshCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    Weapon weapon;
    Rigidbody rb;
    Collider col;
    [SerializeField] private float ragdollForceMultiplier = 10f;
    [SerializeField] CutterBehaviour dynamicMeshCutter;
    [SerializeField] GameObject trailFX;
    [SerializeField] ParticleSystem bloodFX;
    private bool canCut = true;

    private void Start()
    {
        weapon = GetComponentInParent<Weapon>();
        rb = GetComponentInParent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if (rb.velocity.magnitude > weapon.swingThreshold)
        {
            col.isTrigger = !weapon.isBlunt;
            if (!trailFX) return;
            foreach (ParticleSystem p in trailFX.GetComponentsInChildren<ParticleSystem>()) {
                p.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canCut || !col.isTrigger) return;

        if (other.gameObject.layer == 11)
        {
            MeshTarget target = null;
            if (other.gameObject.GetComponentInParent<MeshTarget>())
                target = other.gameObject.GetComponentInParent<MeshTarget>();
            else if (other.GetComponent<MeshTarget>())
                target = other.GetComponent<MeshTarget>();
            else if (other.transform.root.GetComponentInChildren<MeshTarget>())
                target = other.transform.root.GetComponentInChildren<MeshTarget>();
            else return;

            EnemyHealth enemyHealth = other.gameObject.GetComponentInParent<EnemyHealth>();
            Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            if (enemyHealth.isAlive) enemyHealth.Die();
            dynamicMeshCutter.Cut(target, transform.position, transform.right);
            bloodFX.Play();
            //canCut = false;
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        canCut = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        canCut = true;
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11 && collision.gameObject.GetComponentInParent<EnemyHealth>()) // enemy
        {
            if (!collision.gameObject.GetComponentInParent<EnemyHealth>().isAlive)
            {
                //just FX
                EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
                Vector3 contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                GameObject blood = Instantiate(enemyHealth.bloodFX, contactPoint, Quaternion.identity);
                var contact = collision.contacts[0];
                var rot = Quaternion.FromToRotation(blood.transform.up, contact.normal);
                blood.transform.rotation *= rot;
                blood.transform.SetParent(collision.transform);
                enemyHealth.activeBloodFX.Add(blood);
                bloodFX.Play();
                return;
            }

            float damage = weapon.GetDamage();

            if (damage >= 5)
            {
                EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
                enemyHealth.TakeDamage(damage, weapon.isBlunt);

                if (damage > enemyHealth.ragdollThreshold)
                {
                    Rigidbody hitRB = collision.gameObject.GetComponent<Rigidbody>();
                    hitRB.AddForce(rb.velocity * ragdollForceMultiplier, ForceMode.Impulse);
                }

                // blood vfx
                Vector3 contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                //enemyHealth.StrikeBloodFX(contactPoint);
                bloodFX.Play();
                GameObject blood = Instantiate(enemyHealth.bloodFX, contactPoint, Quaternion.identity);
                var contact = collision.contacts[0];
                var rot = Quaternion.FromToRotation(blood.transform.up, contact.normal);
                blood.transform.rotation *= rot;
                blood.transform.SetParent(collision.transform);
                enemyHealth.activeBloodFX.Add(blood);
            }
        }
    }
}
