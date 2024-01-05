using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    [SerializeField] private InputActionProperty jumpInputSource;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] LayerMask groundLayer;

    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        velocity.y = -1.0f;
    }

    private void Update()
    {
        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();
        
        if (IsGrounded() && velocity.y < 0.0f)
        {
            velocity.y = -1.0f;
        } else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }


        if (jumpInput && IsGrounded())
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
        }

        controller.Move(velocity * Time.deltaTime);
    }
    
    public bool IsGrounded()
    {
        Vector3 start = controller.transform.TransformPoint(controller.center);
        float rayLength = controller.height / 2 - controller.radius + 0.05f;

        bool hasHit = Physics.SphereCast(start, controller.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);

        return hasHit;
    }
}