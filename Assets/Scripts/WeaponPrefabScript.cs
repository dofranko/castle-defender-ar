using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefabScript : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    // Update is called once per frame
    public void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Destroy(bullet, 5.0f);
    }
}
