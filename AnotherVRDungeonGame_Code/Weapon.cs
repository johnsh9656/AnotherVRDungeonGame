using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Weapon : MonoBehaviour
{
    [SerializeField] public float swingThreshold = 2f;
    [SerializeField] float minSlowDamage;
    [SerializeField] float maxSlowDamage;
    [SerializeField] float fastDamage;
    public bool isBlunt = false;

    Collider pickupCollider;
    [SerializeField] Collider damageCollider;

    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickupCollider = GetComponent<Collider>();
        damageCollider.enabled = false;
    }

    public void OnPickup()
    {
        damageCollider.enabled = true;
        pickupCollider.enabled = false;
    }

    public void OnDrop()
    {
        damageCollider.enabled = false;
        pickupCollider.enabled = true;
    }

    public float GetDamage()
    {
        float swingSpeed = rb.velocity.magnitude;
        if (swingSpeed > swingThreshold)
        {
            return fastDamage;
        } else
        {
            float normalizedSpeed = Mathf.Clamp01(swingSpeed / swingThreshold);
            float damageAmount = Mathf.Lerp(minSlowDamage, maxSlowDamage, normalizedSpeed);
            return damageAmount;
        }
    }

    public bool IsFast()
    {
        return rb.velocity.magnitude > swingThreshold;
    }

    /*public void RayGrab(SelectEnterEventArgs args)
    {
        if (args.interactableObject is XRRayInteractor)
        {
            trackpos
        }
    }*/
}
