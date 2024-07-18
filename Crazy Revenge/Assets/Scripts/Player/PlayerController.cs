using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : Photon.MonoBehaviour
{
    string[] tagPlayer = { "PlayerRed", "PlayerYellow" };

    [SerializeField]
    TMP_Text scores;
    [SerializeField]
    TMP_Text points;

    [SerializeField]
    PhotonView _photonView;
    [SerializeField]
    TMP_Text _userName;

    [SerializeField]
    GameObject _playerCamera;

    [SerializeField]
    RoundsManager roundsManager;

    [SerializeField]
    GameObject baconTaken;

    bool take = false;

    float time;

    // Различные переменные скоростей
    [Header("Movement")]
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

        roundsManager = GameObject.FindObjectOfType<RoundsManager>().GetComponent<RoundsManager>();

        readyToJump = true;
    }

    private void Update()
    {
        time += Time.deltaTime;

        // Проверка Raycast-ом наличия поверхности пригодной для прыжка
        Debug.DrawRay(transform.position, Vector3.down, Color.red, playerHeight + 0.2f);
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = roundsManager.groundDrag;
        else
            rb.drag = 0;

        if (roundsManager.gameEvent == 0)
        {
            scores.text = $"{roundsManager._redScore}:Kills:{roundsManager._yellowScore}";
        }
        else if (roundsManager.gameEvent == 1)
        {
            scores.text = $"{roundsManager._redScore}:Bacons:{roundsManager._yellowScore}";
        }
        else if (roundsManager.gameEvent == 2)
        {
            scores.text = $"{roundsManager._redScore}:Score:{roundsManager._yellowScore}";
        }

        if (roundsManager._redPoint != 3 && roundsManager._yellowPoint != 3)
            points.text = $"{roundsManager._redPoint}:Red Yellow:{roundsManager._yellowPoint}";
        else if (roundsManager._redPoint == 3 && roundsManager._yellowPoint != 3)
            points.text = $"Win :Red Yellow:{roundsManager._yellowPoint}";
        else if (roundsManager._redPoint != 3 && roundsManager._yellowPoint == 3)
            points.text = $"{roundsManager._redPoint}:Red Yellow: Win";
    }

    private void FixedUpdate()
    {
        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && Input.GetKey(KeyCode.LeftControl))
        {
            MovePlayer(sprintSpeed);
            photonView.RPC("SetAnimMove", PhotonTargets.All, "Run");
        }
        else if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && !Input.GetKey(KeyCode.LeftControl))
        {
            MovePlayer(walkSpeed);
            photonView.RPC("SetAnimMove", PhotonTargets.All, "Walk");
        }
        else
        {
            photonView.RPC("SetAnimMove", PhotonTargets.All, "Idle");
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
        if (flatVel.magnitude > roundsManager.moveSpeed)
        {
            // Нормализуй вектор, прежде чем умножить - Конфуций
            Vector3 limitedVel = flatVel.normalized * roundsManager.moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

    }

    private void Jump()
    {
        // обнуляем по высоте скорость
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * roundsManager.jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    [PunRPC]
    public void SetAnimMove(string action)
    {
        anim.SetTrigger(action);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("CollectableObject") && take == false)
        {
            take = true;
            _photonView.RPC("TakenBacon", PhotonTargets.AllViaServer, true);
        }

        if (collision.gameObject.CompareTag("CollectZone") && baconTaken.activeSelf == true && time > 1.5f)
        {
            _photonView.RPC("TakenBacon", PhotonTargets.AllViaServer, false);
            if (gameObject.tag == tagPlayer[0] && take == true)
            {
                _photonView.RPC(nameof(SumScore), PhotonTargets.AllBuffered, 1, 0);
                take = false;
            }
            else if (gameObject.tag == tagPlayer[1] && take == true)
            {
                _photonView.RPC(nameof(SumScore), PhotonTargets.AllBuffered, 0, 1);
                take = false;
            }

            _photonView.RPC(nameof(BringBacon), PhotonTargets.MasterClient);
            time = 0;
        }


        if (collision.gameObject.CompareTag("SaveZone") && roundsManager.catchup == false && time > 1.5f)
        {
            if (gameObject.tag == tagPlayer[0])
            {
                _photonView.RPC(nameof(SumScore), PhotonTargets.AllBuffered, 1, 0);
                roundsManager.catchup = true;
                roundsManager.SaveZonePosition();
            }
            else if (gameObject.tag == tagPlayer[1])
            {
                _photonView.RPC(nameof(SumScore), PhotonTargets.AllBuffered, 0, 1);
                roundsManager.catchup = true;
                roundsManager.SaveZonePosition();
            }
            time = 0;
        }
    }

    [PunRPC]
    private void BringBacon()
    {
        roundsManager.bring = true;
    }

    [PunRPC]
    public void TakenBacon(bool active)
    {
        roundsManager.bacon.SetActive(!active);
        baconTaken.SetActive(active);
    }

    [PunRPC]
    private void SumScore(int redScore, int yellowScore)
    {
        roundsManager._redScore += redScore;
        roundsManager._yellowScore += yellowScore;
    }
}