using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : Photon.MonoBehaviour
{
    string[] tagPlayer = { "PlayerRed", "PlayerYellow" };
    int tagNum;

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

        view = GetComponent<PhotonView>();
        ammo = 10;
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
                    cht.photonView.RPC("GetDamage", PhotonTargets.All, damage);
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
    }
}