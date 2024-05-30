using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
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
    float damage;
    [SerializeField]
    float shootForce;
    [SerializeField]
    float fireRate;
    [SerializeField]
    float range;

    float nextFire;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
            nextFire = Time.time + 1 / fireRate;
            Shoot();
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
            }
        }
    }
}