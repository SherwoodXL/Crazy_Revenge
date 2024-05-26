using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ��������� ���������� ���������
    [Header("Movement")]
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float groundDrag;
    [SerializeField]
    float jumpForce;
    [SerializeField]
    float jumpCooldown;
    [SerializeField]
    float airMultiplier;
    bool readyToJump;

    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float sprintSpeed;

    [Header("Keybinds")]
    [SerializeField]
    KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    // ������ � ����������� �����
    [SerializeField]
    float playerHeight;
    [SerializeField]
    LayerMask whatIsGround;
    bool grounded;
    // �����������, ������� ������ ��� ��������� ���������� �������
    [SerializeField]
    Transform orientation;
    // ���� ������� ��� ��������������
    [SerializeField]
    float maxAngle;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Animator")]
    [SerializeField]
    Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // �������� Raycast-�� ������� ����������� ��������� ��� ������
        Debug.DrawRay(transform.position, Vector3.down, Color.red, playerHeight + 0.2f);
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && Input.GetKey(KeyCode.LeftControl))
        {
            MovePlayer(sprintSpeed);
            anim.SetTrigger("Run");
        }
        else if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && !Input.GetKey(KeyCode.LeftControl))
        {
            MovePlayer(walkSpeed);
            anim.SetTrigger("Walk");
        }
        else
        {
            anim.SetTrigger("Idle");
        }

        Slope();
    }

    private void Slope()
    {
        // �������� ��� ���� �� ������
        RaycastHit hitInfo;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, playerHeight + 0.5f))
        {
            // ��������� ���� ������� �����������
            float slopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);

            if (slopeAngle > maxAngle)
            {
                // ��������� ���� ����������, �������� ���� �������
                Vector3 gravity = Physics.gravity * Mathf.Cos(Mathf.Deg2Rad * slopeAngle);
                rb.AddForce(gravity * Time.deltaTime * slopeAngle, ForceMode.Impulse);
            }
        }
    }

    private void MyInput()
    {
        // ���������� ������� ��������� ������
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // � ����� ������� �������?
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer(float move)
    {
        // ���������� �����������
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // �� �����
        if (grounded)
            rb.AddForce(moveDirection.normalized * move * 10f, ForceMode.Force);

        // � �������
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * move * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // ������������ ��������, ���� ����� �����������
        if (flatVel.magnitude > moveSpeed)
        {
            // ���������� ������, ������ ��� �������� - ��������
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

    }

    private void Jump()
    {
        // �������� �� ������ ��������
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}