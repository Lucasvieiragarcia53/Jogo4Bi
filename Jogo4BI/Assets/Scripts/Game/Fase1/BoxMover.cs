using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMover : MonoBehaviour
{
    public float speed = 2f; // Velocidade de movimento da caixa
    private Transform player; // Referência ao player

    void Start()
    {
        // Encontrar o player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player não encontrado!");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calcular direção horizontal para o player
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Ignorar movimento vertical, apenas horizontal
        direction = direction.normalized; // Normalizar para direção unitária

        // Mover a caixa na direção calculada
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Verificar se o player pulou sobre a caixa (posição Y do player > Y da caixa + margem, e próximo em X)
        float margin = 0.5f; // Margem para detectar pulo (ajuste conforme tamanho da caixa/player)
        if (player.position.y > transform.position.y + margin && Mathf.Abs(player.position.x - transform.position.x) < 1f)
        {
            // Destruir a caixa quando o player pular por ela
            Destroy(gameObject);
        }
    }
}