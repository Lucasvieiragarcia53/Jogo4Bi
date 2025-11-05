using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Move na direção "up" local (essa direção vem da rotação atribuída pelo firePoint)
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }
}
