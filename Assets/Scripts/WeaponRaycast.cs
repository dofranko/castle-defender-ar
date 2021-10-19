using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{

    private Camera cam;
    private float fireRate;
    private int damage;
    [SerializeField] private LayerMask enemyLayerMask;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10, enemyLayerMask)) //! MASKA KTÓRĄ MA RAYCASTOWAĆ
            {
                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                if (enemy) enemy.TakeDamage(damage);
            }
        }
    }
}
