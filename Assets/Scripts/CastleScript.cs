using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleScript : MonoBehaviour
{
    public HealthBar healthBar;

    void Start()
    {
        healthBar.SetMaxHealth(100);
    }
}
