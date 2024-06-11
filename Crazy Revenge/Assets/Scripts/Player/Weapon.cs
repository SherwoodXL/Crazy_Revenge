using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : Photon.MonoBehaviour
{
    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    ParticleSystem fireEffect;
    [SerializeField]
    Animator shotAnim;
    [SerializeField]
    AudioSource shotAu;
    [SerializeField]
    AudioSource noAmmo;
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

    [SerializeField]
    TMP_Text countAmmo;
    [SerializeField]
    PlayerController damageActive;

    float nextFire;

    private void Start()
    {
        view = GetComponent<PhotonView>();

        countAmmo.text = $"Ammo: {ammo}";
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
            nextFire = Time.time + 1 / fireRate;
            if (ammo != 0)
            {
                Shoot();
            }
            else
            {
                noAmmo.Play();
            }
        }        
    }

    private void Shoot()
    {
        fireEffect.Play();
        shotAnim.SetTrigger("Shot");
        shotAu.Play();

        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range))
        {
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * shootForce);
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    damageActive.Damage(damage);
                }
            }
        }

        ammo--;
        countAmmo.text = $"Ammo: {ammo}";
    }
}