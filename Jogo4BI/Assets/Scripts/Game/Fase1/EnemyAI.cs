using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float maxSpeed = 5f; // Velocidade máxima para evitar movimento muito rápido
    public float attackRange = 1f; // Alcance do ataque do player (deve casar com o do Player)
    private Transform player; // Referência ao player
    private BeatManager Bm; // Referência ao BeatManager
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Encontra o player por tag
        Bm = FindObjectOfType<BeatManager>(); // Encontra o BeatManager

    }

    void Update()
    {
        if (player == null || Bm == null) return;

        // Calcular distância até o alcance do ataque (usando apenas a diferença horizontal)
        float horizontalDistanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float distanceToAttackRange = horizontalDistanceToPlayer - attackRange;

        if (distanceToAttackRange <= 0)
        {
            // Já está no alcance - pare
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

            // Direção para o player (apenas horizontal)
            Vector2 direction = (player.position - transform.position);
            direction.y = 0; // Zerar a componente vertical para não seguir verticalmente
            direction = direction.normalized;
            rb.velocity = direction * requiredSpeed;
        }
        else
        {
            // Se não há tempo (beat imediato), pare ou mova devagar
            rb.velocity = Vector2.zero;
        }
    }
}
