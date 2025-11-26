using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    private Vector2 direction;
    private Rigidbody2D rb;

    public float rotationOffset = -90f;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.localScale = new Vector3(0.15f, 0.15f, 1f);
    }

    void Start()
    {
        rb.velocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica o offset para corrigir sprite apontado para a direita
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }
}
