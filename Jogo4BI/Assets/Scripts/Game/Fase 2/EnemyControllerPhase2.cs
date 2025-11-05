using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerPhase2 : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int bulletCount = 12;
    public float fireRate = 1f;
    public float bulletSpeed = 5f;
    public float spawnOffset = 0.5f; // distância para fora do inimigo
    public float bulletLifetime = 5f; // tempo até o tiro ser destruído

    private float fireTimer;

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            FireCircle();
            fireTimer = 0f;
        }
    }

    void FireCircle()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            // Calcula a posição inicial um pouco afastada do inimigo
            Vector3 spawnPos = transform.position + rot * Vector3.up * spawnOffset;

            // Instancia o tiro nessa posição e rotação
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, rot);

            // Aplica movimento ao tiro
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = bullet.transform.up * bulletSpeed;
            }

            // Evita colisão imediata com o inimigo
            Collider2D enemyCol = GetComponent<Collider2D>();
            Collider2D bulletCol = bullet.GetComponent<Collider2D>();
            if (enemyCol != null && bulletCol != null)
            {
                Physics2D.IgnoreCollision(bulletCol, enemyCol);
            }

            // 🔥 Destroi o tiro após um tempo para evitar sobrecarga
            Destroy(bullet, bulletLifetime);
        }
    }
}
