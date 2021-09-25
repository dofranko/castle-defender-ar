using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{

    private Camera cam;
    private float fireRate;
    private int damage;
    void Start()
    {
        cam = Camera.main;
        Debug.Log("weapon started");
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10, LayerMask.NameToLayer("Shoot Ignore Raycast")))
            {
                Debug.Log("trafiono" + hit.transform.name);
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }
}
