using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveDamage : MonoBehaviour
{
    [SerializeField] float initialDamage = 15;
    [SerializeField] float damagePerSecond = 5;
    [SerializeField] float lifetime = 3;
    [SerializeField] LayerMask layerMask;
    BoxCollider col;
    bool hitOnce = false;
    Transform target;
    float rotateSpeed;

    public void SetLaser(Transform a, float b, Transform c)
    {
        target = a;
        rotateSpeed = b;
        transform.SetParent(c);
    }

    private void Awake()
    {
        Destroy(gameObject, lifetime);
        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            col.size = new Vector3(col.size.x, col.size.y, hit.distance);
            col.center = new Vector3(col.center.x, col.center.y, hit.distance / 2);
        }

        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * 2 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (hitOnce) return;

        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        
        if (player)
        {
            player.TakeDamage(initialDamage);
            hitOnce = true;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if (player)
        {
            player.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

}
