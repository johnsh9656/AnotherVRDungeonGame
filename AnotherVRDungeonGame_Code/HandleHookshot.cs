using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandleHookshot : MonoBehaviour
{
    [SerializeField] private float hookshotSpeed = 8;
    private float originalHookshotSpeed;
    [SerializeField] private float hookshotStopDistance = 1f;

    [SerializeField] private float hookshotCooldown = 2;
    [SerializeField] private bool onCooldown = false;

    [SerializeField] private Transform hookHand;
    [SerializeField] private Transform hookshotTransform;
    private float hookshotSize;
    [SerializeField] private InputActionProperty hookInput;
    private bool active = false;
    private bool hookThrown = false;
    private Vector3 hookshotPosition;
    private Vector3 hookshotDir;
    [SerializeField] float momentumBoost = 4;

    public static event Action HookHandActivated;
    public static event Action HookHandDeactivated;

    private CharacterController charController;
    private CharacterControllerGravity ccg;
    //[SerializeField] LayerMask layerMask;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        ccg = GetComponent<CharacterControllerGravity>();
        hookshotTransform.localScale = Vector3.zero;
        originalHookshotSpeed = hookshotSpeed;
    }

    private void Update()
    {
        if (hookInput.action.WasPressedThisFrame() && !onCooldown)
        {
            if (Physics.Raycast(hookHand.transform.position, hookHand.transform.forward, out RaycastHit raycastHit, 50))
            {
                if (raycastHit.collider.gameObject.layer == 12 || raycastHit.collider.gameObject.layer == 6)
                {
                    // Hit something
                    hookshotPosition = raycastHit.point;
                    HookHandActivated?.Invoke();
                    hookThrown = true;
                    hookshotSize = 0;
                    StartCooldown();
                }
            }
        }
        else if (hookInput.action.WasReleasedThisFrame() && active)
        {
            StopHookshot();
            Vector3 momentum = hookshotDir * hookshotSpeed * momentumBoost;
            ccg.AddMomentum(momentum);
        }

        if (active)
        {
            hookshotTransform.LookAt(hookshotPosition);
            hookshotDir = (hookshotPosition - transform.position).normalized;

            charController.Move(hookshotDir * hookshotSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, hookshotPosition) <= hookshotStopDistance)
            {
                StopHookshot();
                Vector3 momentum = hookshotDir * hookshotSpeed * momentumBoost;
                ccg.AddMomentum(momentum);
            }
        } else if (hookThrown)
        {
            hookshotTransform.LookAt(hookshotPosition);

            float hookshotThrowSpeed = 120f;
            hookshotSize += hookshotThrowSpeed * Time.deltaTime;
            hookshotTransform.localScale = new Vector3(.02f, .02f, hookshotSize);

            if (hookshotSize >= Vector3.Distance(hookshotTransform.position, hookshotPosition))
            {
                active = true;
                hookThrown = false;
            }
        }
    }

    private void StopHookshot()
    {
        hookshotTransform.localScale = Vector3.zero;
        hookThrown = false;
        HookHandDeactivated?.Invoke();
        active = false;
        hookshotSize = 0;
    }

    private void StartCooldown()
    {
        onCooldown = true;
        Invoke(nameof(EndCooldown), hookshotCooldown);
    }

    private void EndCooldown() { onCooldown = false; }

    public void SpeedPickup(float pickupSpeedMultiplier, float length)
    {
        hookshotSpeed = originalHookshotSpeed * pickupSpeedMultiplier;
        Invoke(nameof(ResetMoveSpeed), length);
    }

    private void ResetMoveSpeed()
    {
        hookshotSpeed = originalHookshotSpeed;
    }
}
