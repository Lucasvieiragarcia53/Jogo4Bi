using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f;         // Velocidade do inimigo
    public float rangeVisao = 5f;    // Alcance para detectar o player
    public int dano = 1;            // Dano que causa ao encostar

    private Transform player;        // Referência ao player
    private Rigidbody2D rb;          // Rigidbody do inimigo
    private Vector2 direcao;         // Direção de movimento

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player não encontrado! Certifique-se de que ele tem a Tag 'Player'.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distancia = Vector2.Distance(transform.position, player.position);

            if (distancia <= rangeVisao)
            {
                // Calcula direção até o player
                direcao = (player.position - transform.position).normalized;
            }
            else
            {
                direcao = Vector2.zero; // Para de se mover se o player estiver fora do alcance
            }
        }
    }

    void FixedUpdate()
    {
        // Move o inimigo
        rb.velocity = new Vector2(direcao.x * speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.vidas -= dano;
                Debug.Log("Inimigo causou dano! Vidas do player: " + playerScript.vidas);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Mostra o alcance de visão no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeVisao);
    }
}
