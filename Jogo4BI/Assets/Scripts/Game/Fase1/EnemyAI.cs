using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float maxSpeed = 5f; // Velocidade máxima 
    public float attackRange = 1f; // Alcance do ataque do player
    private Transform player; // Referência ao player
    private BeatManager Bm; // Referência ao BeatManager
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Encontra o player
        Bm = FindObjectOfType<BeatManager>(); // Encontra o BeatManager

    }

    void Update()
    {
        if (player == null || Bm == null) return;

        // Calcular distância até o alcance do ataque
        float horizontalDistanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float distanceToAttackRange = horizontalDistanceToPlayer - attackRange;

        if (distanceToAttackRange <= 0)
        {
            // Já está no alcance
            rb.velocity = Vector2.zero;
            return;
        }

        // Obter tempo até o próximo beat
        float timeToNextBeat = Bm.GetTimeToNextBeat();

        if (timeToNextBeat > 0)
        {
            // Calcular velocidade necessária para chegar exatamente no beat
            float requiredSpeed = distanceToAttackRange / timeToNextBeat;
            // Limitar à velocidade máxima
            requiredSpeed = Mathf.Min(requiredSpeed, maxSpeed);

            // Direção para o player
            Vector2 direction = (player.position - transform.position);
            direction.y = 0;
            direction = direction.normalized;
            rb.velocity = direction * requiredSpeed;
        }
        else
        {
            // Se não há tempo, pare ou mova devagar
            rb.velocity = Vector2.zero;
        }
    }
}
