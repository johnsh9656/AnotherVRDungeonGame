using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class CharacterControllerGravity : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private InputActionProperty jumpInputSource;
    [SerializeField] private float jumpHeight = 2.0f;
    private CharacterController controller;
    private bool _climbing = false;
    private bool _hookShot = false;
    private Vector3 velocity;
    private Vector3 momentum;
    [SerializeField] float speed = 2.5f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        velocity.y = -1.0f;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        ClimbProvider.ClimbActive += ClimbActive;
        ClimbProvider.ClimbInactive += ClimbInactive;
        HandleHookshot.HookHandActivated += HookActive;
        HandleHookshot.HookHandDeactivated += HookInactive;
        speed = GetComponent<DynamicMoveProvider>().moveSpeed;
    }

    private void OnDestroy()
    {
        ClimbProvider.ClimbActive -= ClimbActive;
        ClimbProvider.ClimbInactive -= ClimbInactive;
        HandleHookshot.HookHandActivated -= HookActive;
        HandleHookshot.HookHandDeactivated -= HookInactive;
    }

    private void Update()
    {
        if (_hookShot)
        {
            momentum = Vector3.zero;
            velocity = Vector3.zero;
            return;
        }

        if (_climbing) { velocity.y = 0; return; }
        
        if (IsGrounded() && velocity.y < 0.0f)
        {
            velocity.y = -1.0f;
        }
        else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }

        // Jump
        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();
        if (jumpInput && IsGrounded())
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
        }

        // apply momentum
        velocity += momentum;

        // move character controller
        controller.Move(velocity * Time.deltaTime);

        // dampen momentum
        if (momentum.magnitude >= 0f)
        {
            float momentumDrag = 4;
            velocity -= momentum;
            momentum -= momentum * momentumDrag * Time.deltaTime;
            if (momentum.magnitude < .0f)
            {
                momentum = Vector3.zero;
                velocity = new Vector3(0, velocity.y, 0);
            }
        }
    }

    public void AddMomentum(Vector3 m)
    {
        momentum += m;
    }

    public bool IsGrounded()
    {
        Vector3 start = controller.transform.TransformPoint(controller.center);
        float rayLength = controller.height / 2 - controller.radius + 0.05f;

        bool hasHit = Physics.SphereCast(start, controller.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);

        return hasHit;
    }

    private void ClimbActive()
    {
        _climbing = true;
    }

    private void ClimbInactive()
    {
        _climbing = false;
    }

    private void HookActive()
    {
        _hookShot = true;
    }

    private void HookInactive()
    {
        _hookShot = false;
        velocity.y = 0;
    }

    public void SpeedPickup(float pickupSpeedMultiplier, float length)
    {
        GetComponent<DynamicMoveProvider>().moveSpeed = speed * pickupSpeedMultiplier;
        Invoke(nameof(ResetMoveSpeed), length);
    }

    private void ResetMoveSpeed()
    {
        GetComponent<DynamicMoveProvider>().moveSpeed = speed;
    }

    public void ResetForTeleport()
    {
        momentum = Vector3.zero;
        velocity.y = 0;
    }
}
