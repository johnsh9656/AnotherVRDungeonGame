using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MagicGroundAttack : MonoBehaviour
{
    [SerializeField] SphereCollider explosion;
    [SerializeField] BoxCollider trail;

    [SerializeField] float timeToExplosion = 0.25f;
    [SerializeField] float trailLength = 7;
    [SerializeField] float lifetime = 6;
    float timer = 0;

    private void Awake()
    {
        explosion.enabled = false;
        Destroy(gameObject, lifetime);
        explosion.transform.rotation = transform.rotation;
        trail.transform.rotation = transform.rotation;
        explosion.transform.localPosition = new Vector3(0, 0, 8.75f);
    }

    void Update()
    {
        if (timer < timeToExplosion)
        {
            if (!explosion || !trail) return;

            explosion.transform.rotation = transform.rotation;
            trail.transform.rotation = transform.rotation;
            trail.transform.localPosition = Vector3.zero;
            explosion.transform.localPosition = new Vector3(0, 0, 8.75f);

            timer += Time.deltaTime;

            float ratio = timer / timeToExplosion;
            float length = Mathf.Lerp(0, trailLength, ratio);

            trail.size = new Vector3(trail.size.x, trail.size.y, length);
            trail.center = new Vector3(trail.center.x, trail.center.y, (length / 2) + 0.25f);
            if (timer >= timeToExplosion)
            {
                explosion.enabled = true;
                Destroy(explosion.GetComponent<DealDamage>(), .5f);
            }
        }
    }
}
