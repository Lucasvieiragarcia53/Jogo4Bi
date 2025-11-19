using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int vidas = 5;
    private Rigidbody2D rb;
    private float horizontalInput;
    public float speed;
    Vector2 move;
    public float jumpForce = 12f;
    private bool jump;
    private bool isGrounded;
    private BeatManager Bm; // Referência ao BeatManager

    // Novos campos para ataque
    public float attackRange = 1f; // Distância do ataque
    public LayerMask enemyLayer; // Layer dos inimigos (configure no Inspector)
    public Transform attackPoint; // Ponto de origem do ataque (ex.: frente do player)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Corrigido: Encontre o BeatManager na cena (assumindo que há apenas um)
        Bm = FindObjectOfType<BeatManager>();
        if (Bm == null)
        {
            Debug.LogError("BeatManager não encontrado na cena!");
        }
    }

    void Update()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.J))
        {
            jump = true;
            BeatManager.HitQuality quality = Bm.GetHitQuality();
            if (Bm != null && Bm.IsOnBeat())
            {
                Bm.IncrementCombo();
                Bm.AddScore(quality); // Passa a qualidade para AddScore
            }
            else
            {
                Bm.ResetCombo();
            }
        }

        // Novo: Verificar input para ataque
        if (Input.GetKeyDown(KeyCode.K))
        {
                Attack();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
    }

    void Move()
    {
        rb.velocity = new Vector2(horizontalInput * speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    void FixedUpdate()
    {
        Move();
        if (jump)
        {
            Jump();
        }
        jump = false;
    }

    // Novo método: Ataque
    void Attack()
    {
        // Detectar inimigos na área de ataque usando OverlapCircleAll
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // Verificar se o ataque está no ritmo
                BeatManager.HitQuality quality = Bm.GetHitQuality();
                if (quality != BeatManager.HitQuality.Miss)
                {
                    Bm.IncrementCombo();
                    Bm.AddScore(quality); // Passa a qualidade para AddScore
                    Destroy(enemy.gameObject);
                }
                else
                {
                    Bm.ResetCombo();
                    Destroy(enemy.gameObject); 
                }
            }
        }
    }

    // Novo: Desenhar gizmo para visualizar o alcance do ataque (no Editor)
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
