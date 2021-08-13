using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 0, rotationSpeed = 0, gravity = 0, sprintSpeed = 0;

    private Rigidbody rb;

    private Vector3 moveDirection = Vector3.zero;

    private float moveSpeed = 0, hitDistance = 0.35f, maxFallSpeed = -15;

    private bool grounded = false, sprinting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            sprinting = true;
        } else
        {
            sprinting = false;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            MoveVertically();
        }
        else
        {
            float yVelocity = moveDirection.y;
            Vector3 movement = Vector3.zero;

            movement = transform.forward * 0;
            movement.y = yVelocity;

            moveDirection = movement;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Rotate(-1);
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            Rotate(1);
        }

        CheckCollisionGround();

        if (!grounded)
        {
            moveDirection.y -= gravity;
            if (moveDirection.y <= maxFallSpeed)
            {
                moveDirection.y = maxFallSpeed;
            }
        }
        else
        {
            moveDirection.y = 0;
        }

        rb.velocity = moveDirection;
    }

    private void MoveVertically()
    {
        float yVelocity = moveDirection.y;
        Vector3 movement = Vector3.zero;

        if (sprinting)
        {
            moveSpeed = sprintSpeed;
        } else
        {
            moveSpeed = walkSpeed;
        }
        
        movement = transform.forward * moveSpeed;
        movement.y = yVelocity;

        moveDirection = movement;
    }

    private void Rotate(float direction)
    {
        transform.Rotate(0, rotationSpeed * direction, 0);
    }

    private void CheckCollisionGround()
    {
        if (grounded)
        {
            hitDistance = 0.35f;
        }
        else
        {
            hitDistance = 0.15f;
        }

        if (Physics.Raycast(transform.position - new Vector3(0, 0.95f, 0), -transform.up, hitDistance))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }
}
