using System.Collections; 
using System.Collections;
using UnityEngine;

public class EnemyControllerPhase2 : MonoBehaviour
{
    [Header("Disparo Normal")]
    public GameObject bulletPrefab;
    public int bulletCount = 12;
    public float fireRate = 1f;
    public float bulletSpeed = 5f;
    public float spawnOffset = 0.5f;
    public float bulletLifetime = 5f;

    [Header("Feixe Especial")]
    public GameObject energyBeamPrefab; // Prefab do feixe de energia
    public Transform FirePoint;
    public float beamDuration = 3f;     // Duração do feixe
    public float beamCooldown = 2f;     // Tempo antes de disparar o feixe

    private float fireTimer;
    private int specialCount;           // Contador de quantas vezes o número 5 saiu
    private bool isFiringBeam;          // Evita conflitos durante o feixe

    void Update()
    {
        if (isFiringBeam) return; // pausa ataques normais enquanto o feixe acontece

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            FireCircle();
            fireTimer = 0f;

            // Sorteia número entre 1 e 10
            int randomNum = Random.Range(1, 6);
            Debug.Log("Sorteio: " + randomNum);

            // Se sair 5, soma;
            if (randomNum >= 5)
                specialCount++;

            // Quando sair três 5 seguidos, ativa o feixe
            if (specialCount >= 3)
            {
                StartCoroutine(FireEnergyBeam());
                specialCount = 0;
            }
        }
    }

    void FireCircle()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            Vector3 spawnPos = transform.position + rot * Vector3.up * spawnOffset;

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, rot);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = bullet.transform.up * bulletSpeed;

            Collider2D enemyCol = GetComponent<Collider2D>();
            Collider2D bulletCol = bullet.GetComponent<Collider2D>();
            if (enemyCol != null && bulletCol != null)
                Physics2D.IgnoreCollision(bulletCol, enemyCol);

            Destroy(bullet, bulletLifetime);
        }
    }

    IEnumerator FireEnergyBeam()
    {
        isFiringBeam = true;
        Debug.Log("⚡ Feixe carregando...");

        // Espera 2 segundos antes do disparo (cooldown)
        yield return new WaitForSeconds(beamCooldown);

        Debug.Log("🔥 Feixe disparado!");

        // Instancia o feixe (ajuste a posição/rotação conforme o prefab)
        GameObject beam = Instantiate(energyBeamPrefab, FirePoint.position, FirePoint.rotation  );

        // Mantém o feixe ativo por 3 segundos
        yield return new WaitForSeconds(beamDuration);

        Destroy(beam);
        Debug.Log("💨 Feixe terminou.");

        isFiringBeam = false;
    }
}
