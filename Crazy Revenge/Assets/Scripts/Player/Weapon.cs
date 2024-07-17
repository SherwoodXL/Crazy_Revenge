using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : Photon.MonoBehaviour
{
    string[] tagPlayer = { "PlayerRed", "PlayerYellow" };
    int tagNum;
    [SerializeField]
    GameObject player;

    GameObject deathCamera;

    [SerializeField]
    TMP_Text scores;

    [SerializeField]
    GameObject redScreen;

    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    Animator shotAnim;
    [SerializeField]
    PhotonView view;

    [SerializeField]
    int damage;
    [SerializeField]
    float shootForce;
    [SerializeField]
    float fireRate;
    [SerializeField]
    float range;
    [SerializeField]
    int ammo;

    public int hp = 100;
    [SerializeField]
    TMP_Text hpText;


    [SerializeField]
    TMP_Text countAmmo;
    [SerializeField]
    PlayerController damageActive;
    [SerializeField]
    RoundsManager roundsManager;

    [SerializeField]
    GameObject[] weapon;
    [SerializeField]
    AudioSource[] weaponAu;
    [SerializeField]
    ParticleSystem[] fireEffect;

    [SerializeField]
    float nextFire;

    private void Start()
    {
        if (gameObject.tag == tagPlayer[0])
        {
            tagNum = 1;
        }
        else if (gameObject.tag == tagPlayer[1])
        {
            tagNum = 0;
        }

        Debug.Log(tagPlayer[tagNum]);

        shotAnim = GetComponent<Animator>();

        roundsManager = FindObjectOfType<RoundsManager>().GetComponent<RoundsManager>();
        deathCamera = GameObject.FindGameObjectWithTag("MainCamera");

        view = GetComponent<PhotonView>();
        countAmmo.text = $"Ammo: {ammo}";
        hpText.text = $"Health: {hp}";

        weapon[0].SetActive(true);
        weapon[1].SetActive(false);
        weapon[2].SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
            nextFire = Time.time + 1 / fireRate;
            if (ammo != 0)
            {
                nextFire = Time.time + 1 / fireRate;
                Shoot(10, 15, 10);
                view.RPC("ShootAnim", PhotonTargets.All, 0, "Shot");
            }
            else
            {
                weaponAu[3].Play();
            }
        }

        if (roundsManager.gameEvent == 2)
        {
            scores.text = $"Time:{roundsManager.timeCU}";
        }
    }

    public void Shoot(float rangeShot, float shootForce, int damage)
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, rangeShot))
        {
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * shootForce);
                if (hit.collider.CompareTag(tagPlayer[tagNum]))
                {
                    Weapon cht = hit.collider.GetComponent<Weapon>();
                    cht.photonView.RPC("GetDamage", PhotonTargets.AllBuffered, damage);
                }
            }
        }

        ammo--;
        countAmmo.text = $"Ammo: {ammo}";
    }

    [PunRPC]
    public void ShootAnim(int weapon, string action)
    {
        shotAnim.SetTrigger(action);
        fireEffect[weapon].Play();
        weaponAu[weapon].Play();
    }

    public void ChangeWeapon()
    {

    }

    [PunRPC]
    public void GetDamage(int damage)
    {
        hp -= damage;
        hpText.text = $"Health: {hp}";

        if (hp <= 0 && photonView.isMine)
        {
            photonView.RPC("Die", PhotonTargets.AllBuffered);
            hp = 100;
            hpText.text = $"Health: {hp}";

            if (gameObject.tag == tagPlayer[0])
            {
                gameObject.transform.position = new Vector3(10, 10, 10);
            }
            else if (gameObject.tag == tagPlayer[1])
            {
                gameObject.transform.position = new Vector3(-10, 10, 10);
            }
        }

        redScreen.SetActive(true);
        Invoke("RedScreen", 0.2f);
    }

    void RedScreen()
    {
        redScreen.SetActive(false);
    }

    [PunRPC]
    public void Die()
    {
        bool resetDie = true;

        if (roundsManager.gameEvent == 0 && resetDie == true)
        {
            if (gameObject.tag == tagPlayer[0])
            {
                ScoresKills(0, 1);
            }
            else if (gameObject.tag == tagPlayer[1])
            {
                ScoresKills(1, 0);
            }
            resetDie = false;
        }
    }

    private void ScoresKills(int redScore, int yellowScore)
    {
        roundsManager._redScore += redScore;
        roundsManager._yellowScore += yellowScore;
    }
}