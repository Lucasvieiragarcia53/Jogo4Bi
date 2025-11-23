using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    // Campo para animação de corrida (sem bounce)
    public Animator animator; // Referência ao Animator do player 

    // Novo: Referência ao Placar (arraste o objeto Placar no Inspector)
    public Placar placar;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Corrigido: Encontre o BeatManager na cena (assumindo que há apenas um)
        Bm = FindObjectOfType<BeatManager>();
        if (Bm == null)
        {
            Debug.LogError("BeatManager não encontrado na cena!");
        }

        // Inicialização para animação
        if (animator != null)
        {
            animator.SetBool("IsRunning", false); // Começa parado
            animator.SetBool("IsGrounded", true); // Começa no chão
        }
    }

    void Update()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        horizontalInput = Input.GetAxis("Horizontal");

        // Atualizar animação de corrida baseada no movimento horizontal
        if (animator != null)
        {
            animator.SetBool("IsRunning", Mathf.Abs(horizontalInput) > 0.1f); // Ativa se estiver se movendo
            animator.SetBool("IsGrounded", isGrounded); // Atualiza se está no chão
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isGrounded) // Só permite pulo se estiver no chão
            {
                jump = true;
                // Ativar trigger de animação de pulo
                if (animator != null)
                {
                    animator.SetTrigger("Jump"); // Chama a animação de pulo via trigger
                }
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
        }

        // Ataque só se estiver no beat (similar ao pulo)
        if (Input.GetKeyDown(KeyCode.K) && Bm != null && Bm.IsOnBeat())
        {
            Attack();
        }
    }

    void Jump()
    {
        // Corrigido: Aplicar jumpForce diretamente (sem multiplicar por Time.fixedDeltaTime, pois isso reduz o pulo drasticamente)
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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

    // Método de Ataque (combo reseta se miss OU se não acertar inimigo)
    void Attack()
    {
        // Ativar trigger de animação de ataque
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // Chama a animação de ataque via trigger
        }

        // Obter qualidade do hit primeiro
        BeatManager.HitQuality quality = Bm.GetHitQuality();

        // Detectar inimigos na área de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        bool enemyHit = false; // Flag para saber se acertou algum inimigo

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemyHit = true;
                Destroy(enemy.gameObject, 0.1f); // Destrói o inimigo
            }
        }

        // Lógica de combo e score baseada na qualidade e se acertou inimigo
        if (quality != BeatManager.HitQuality.Miss && enemyHit)
        {
            Bm.IncrementCombo();
            Bm.AddScore(quality);
        }
        else
        {
            Bm.ResetCombo(); // Reseta combo se miss OU se não acertar inimigo
        }
    }

    // Desenhar gizmo para visualizar o alcance do ataque (no Editor)
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // Integrado: Detectar colisão com chão (para isGrounded) e inimigos
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            // Resetar trigger de pulo se necessário (opcional, para evitar loops)
            if (animator != null)
            {
                animator.ResetTrigger("Jump");
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Chamar o método no Placar para perder uma vida
            if (placar != null)
            {
                placar.PerderVida();
            }

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Box"))
        {
            if (placar != null)
            {
                placar.PerderVida();
            }

            
            Destroy(collision.gameObject);
        }
    }

    // Novo: Detectar saída de colisão com o chão
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}