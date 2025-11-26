using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLongRangeController : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public int vida = 3;
    public float speed = 3f;
    public float distanceRetreat = 2f;
    public float fieldOfView = 5f;
    public Transform player;
    public LayerMask playerLayer;

    [Header("Ataque à Distância")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireCooldown = 1.5f;

    private Rigidbody2D rb;
    private bool canShoot = true;
    private bool takingDamage = false;
    public bool coco;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Player dentro do campo de visão
        if (distance <= fieldOfView)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            Retreat(direction);

            if (canShoot)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    void Retreat(Vector2 direction)
    {
        Vector2 retreatPosition = (Vector2)transform.position + direction * distanceRetreat * Time.deltaTime * speed;
        rb.MovePosition(retreatPosition);
    }

IEnumerator Shoot()
{
    canShoot = false;

    // Instancia o projétil
    GameObject proj = Instantiate(projectilePrefab, new Vector3(firePoint.position.x, firePoint.position.y, 1), Quaternion.identity);


    // Calcula a direção em linha reta até o player
    Vector2 direction = (player.position - firePoint.position).normalized;

    // Define a direção no script do projétil
    proj.GetComponent<Projectile>().SetDirection(direction);

    yield return new WaitForSeconds(fireCooldown);
    canShoot = true;
}


    public void TakeDamage(int amount)
    {
        if (takingDamage) return;

        takingDamage = true;
        vida -= amount;

        if (vida <= 0)
        {
            Die();
            return;
        }

        // Recuar e atirar ao receber dano
        Vector2 direction = (transform.position - player.position).normalized;
        Retreat(direction);

        if (canShoot)
        {
            StartCoroutine(Shoot());
        }

        StartCoroutine(ResetDamageState());
    }

    IEnumerator ResetDamageState()
    {
        yield return new WaitForSeconds(0.5f);
        takingDamage = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // Apenas para visualizar o campo de visão no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldOfView);
    }
}
