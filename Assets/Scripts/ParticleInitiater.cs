using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInitiater : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    void Start()
    {
        particles.Play();
        Destroy(gameObject, particles.main.duration);
    }

}
