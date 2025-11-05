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
    private SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Garantias de visibilidade
        sr.enabled = true;
        sr.color = new Color(1, 1, 1, 1);
        transform.localScale = Vector3.one;
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // Garante posição Z visível
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);
    }

    void Start()
    {
        rb.velocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
