using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public float scrollSpeed = 0.5f; // Velocidade de scroll (ajuste para parecer natural)
    private Material material; // Instância do material do chão
    private Transform player; // Referência ao player
    private Vector3 lastPlayerPosition; // Posição anterior do player

    void Start()
    {
        // Encontre o player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("GroundScroller: Nenhum objeto com tag 'Player' encontrado!");
            return;
        }
        lastPlayerPosition = player.position;

        // Pegue o SpriteRenderer e crie uma instância do material
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            material = Instantiate(sr.material); // Cria uma cópia para evitar afetar outros objetos
            sr.material = material;
        }
        else
        {
            Debug.LogWarning("GroundScroller: Nenhum SpriteRenderer encontrado. Scroll via transform pode não ser infinito.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calcule o movimento do player desde o último frame
        float deltaX = player.position.x - lastPlayerPosition.x;
        lastPlayerPosition = player.position;

        // Aplique o scroll na direção oposta
        float scrollAmount = deltaX * scrollSpeed;

        if (material != null)
        {
            // Mova o offset da textura (para sprites tiláveis)
            Vector2 offset = material.mainTextureOffset;
            offset.x -= scrollAmount; // Mova para a esquerda se player for direita
            material.mainTextureOffset = offset;
        }
        else
        {
            // Alternativa: Mova o transform (menos ideal para scroll infinito)
            transform.Translate(Vector3.left * scrollAmount);
        }
    }
}