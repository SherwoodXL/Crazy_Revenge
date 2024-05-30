using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLife = 3;

    private void Awake()
    {
        Destroy(gameObject, bulletLife);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Destroy(gameObject);
    }
}
