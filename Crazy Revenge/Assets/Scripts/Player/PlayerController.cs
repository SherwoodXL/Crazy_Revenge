using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : Photon.MonoBehaviour
{
    [SerializeField]
    PhotonView _photonView;
    [SerializeField]
    TMP_Text _userName;

    [SerializeField]
    GameObject _playerCamera;



    int hp = 100;
    [SerializeField]
    TMP_Text hpText;

    // Различные переменные скоростей
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
    // Высота и определение земли
    [SerializeField]
    float playerHeight;
    [SerializeField]
    LayerMask whatIsGround;
    bool grounded;
    // Направление, которое хранит все возможные координаты объекта
    [SerializeField]
    Transform orientation;

    [Header("Slope")]
    // Угол наклона для соскальзывания
    [SerializeField]
    float maxAngle;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Animator")]
    [SerializeField]
    Animator anim;

    private void Awake()
    {
        if (_photonView.isMine)
        {
            _playerCamera.SetActive(true);
            _userName.text = PhotonNetwork.playerName;
        }
        else
        {
            _userName.text = photonView.owner.name;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        _photonView = GetComponent<PhotonView>();

        readyToJump = true;
    }

    private void Update()
    {
        // Проверка Raycast-ом наличия поверхности пригодной для прыжка
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
            photonView.RPC("SetAnim", PhotonTargets.All, "Run");
        }
        else if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && !Input.GetKey(KeyCode.LeftControl))
        {
            MovePlayer(walkSpeed);
            photonView.RPC("SetAnim", PhotonTargets.All, "Walk");
        }
        else
        {
            photonView.RPC("SetAnim", PhotonTargets.All, "Idle");
        }

        Slope();
    }

    private void Slope()
    {
        // Проводим луч вниз от игрока
        RaycastHit hitInfo;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, playerHeight + 0.5f))
        {
            // Проверяем угол наклона поверхности
            float slopeAngle = Vector3.Angle(Vector3.up, hitInfo.normal);

            if (slopeAngle > maxAngle && grounded)
            {
                // Приминяем силу гравитации, учитывая угол наклона         
                Vector3 gravity = Physics.gravity * Mathf.Cos(Mathf.Deg2Rad * slopeAngle);
                rb.AddForce(gravity * Time.deltaTime * slopeAngle, ForceMode.Impulse);
            }
        }
    }

    private void MyInput()
    {
        // записываем текущее состояние клавиш
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // в каких случаях прыгаем?
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer(float move)
    {
        // рассчитаем направление
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // на земле
        if (grounded)
            rb.AddForce(moveDirection.normalized * move * 10f, ForceMode.Force);

        // в воздухе
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * move * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // ограничиваем скорость, если вдруг разогнались
        if (flatVel.magnitude > moveSpeed)
        {
            // Нормализуй вектор, прежде чем умножить - Конфуций
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

    }

    private void Jump()
    {
        // обнуляем по высоте скорость
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public void Damage(int damage)
    {
        hp -= damage;
        hpText.text = $"Health: {hp}";
    }

    [PunRPC]
    public void SetAnim(string action)
    {
        anim.SetTrigger(action);
    }
}