using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCaster : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    public void Cast(Vector3 startPoint, Vector3 endPoint)
    {
        if (transform.position != startPoint) transform.position = startPoint;
        float scaleZ = Vector3.Distance(startPoint, endPoint) * 0.1f;
        particles.transform.LookAt(endPoint);
        particles.transform.localScale = new Vector3(particles.transform.localScale.x, particles.transform.localScale.y, scaleZ);
        particles.Play();
        Destroy(gameObject, particles.main.duration);
    }
}
