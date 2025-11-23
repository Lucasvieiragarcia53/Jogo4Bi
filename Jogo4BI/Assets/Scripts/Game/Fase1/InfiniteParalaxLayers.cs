using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// O que o código faz:
/// Controla um parallax infinito e contínuo para múltiplas camadas 2D.
/// Agora inclui uma camada de chão (groundLayer) que se move baseada no movimento do jogador,
/// similar ao GroundScroller, enquanto as outras camadas continuam com movimento constante.
/// OBS: O exemplo está configurado com 3 camadas (fastLayer, slowLayer e groundLayer).

public class InfiniteParalaxLayers : MonoBehaviour
{
    // ------------------------------
    // Estrutura de uma camada (Layer)
    // ------------------------------
    [System.Serializable] // Permite que a classe apareça no Inspector
    public class Layer
    {
        public Transform tileA; // Referência ao primeiro tile
        public Transform tileB; // Referência ao segundo tile
        public float speed = 2f; // Velocidade de deslocamento da camada (usado para camadas não-jogador)
        public bool pixelSnap = false; // Se true, ativa arredondamento para grade
        public float pixelsPerUnit = 100f; // Pixels Por Unidade
        public bool followPlayer = false; // Nova flag: se true, a camada segue o movimento do jogador (para chão)
        public float scrollSpeed = 0.5f; // Velocidade de scroll para camadas que seguem o jogador

        // Calculadas/armazenadas em tempo de execução:
        [HideInInspector] public float tileWidth;
        [HideInInspector] public float baseAX;
        [HideInInspector] public float baseAY;
        [HideInInspector] public float baseBX;
        [HideInInspector] public float baseBY;
        [HideInInspector] public float offset;
    }

    public Layer fastLayer = new Layer { speed = 2.0f }; // Camada “próxima”
    public Layer slowLayer = new Layer { speed = 0.8f }; // Camada “distante”
    public Layer groundLayer = new Layer { followPlayer = true, scrollSpeed = 0.5f }; // Nova camada de chão que segue o jogador

    private Transform player; // Referência ao jogador
    private Vector3 lastPlayerPosition; // Posição anterior do jogador

    void Start()
    {
        // Encontre o jogador
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("InfiniteParalaxLayers: Nenhum objeto com tag 'Player' encontrado!");
        }
        else
        {
            lastPlayerPosition = player.position;
        }

        // Inicializa cada camada
        InitLayer(fastLayer);
        InitLayer(slowLayer);
        InitLayer(groundLayer);
    }

    void LateUpdate() // Executa após a câmera
    {
        // Atualiza o deslocamento contínuo de cada camada
        UpdateLayer(fastLayer);
        UpdateLayer(slowLayer);
        UpdateLayer(groundLayer);
    }

    void InitLayer(Layer L)
    {
        // Mesmo código de inicialização original
        if (L.tileA == null || L.tileB == null)
        {
            Debug.LogWarning("[Parallax] Tile A/B não atribuídos.");
            return;
        }

        SpriteRenderer sr = L.tileA.GetComponent<SpriteRenderer>();
        if (sr == null) sr = L.tileA.GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            sr = L.tileB.GetComponent<SpriteRenderer>();
            if (sr == null) sr = L.tileB.GetComponentInChildren<SpriteRenderer>();
        }
        if (sr != null)
        {
            L.tileWidth = sr.bounds.size.x;
        }
        else
        {
            L.tileWidth = Mathf.Abs(L.tileB.position.x - L.tileA.position.x);
        }

        L.tileB.position = new Vector3(
            L.tileA.position.x + L.tileWidth,
            L.tileB.position.y,
            L.tileB.position.z
        );

        L.baseAX = L.tileA.position.x;
        L.baseAY = L.tileA.position.y;
        L.baseBX = L.tileB.position.x;
        L.baseBY = L.tileB.position.y;
        L.offset = 0f;
    }

    // --------------------------------------------------------------
    // Atualiza a camada aplicando offset cíclico contínuo
    // --------------------------------------------------------------
    void UpdateLayer(Layer L)
    {
        if (L.tileA == null || L.tileB == null || L.tileWidth <= 0f) return;

        if (L.followPlayer && player != null)
        {
            // Para camadas que seguem o jogador (ex.: chão), calcula offset baseado no movimento do jogador
            float deltaX = player.position.x - lastPlayerPosition.x;
            lastPlayerPosition = player.position;
            float scrollAmount = deltaX * L.scrollSpeed;
            L.offset -= scrollAmount; // Offset na direção oposta ao movimento do jogador
        }
        else
        {
            // Para camadas normais, usa velocidade constante
            L.offset += L.speed * Time.deltaTime;
        }

        // Mantém o offset sempre dentro de [0, tileWidth)
        if (L.offset >= L.tileWidth)
        {
            L.offset -= L.tileWidth;
        }
        else if (L.offset < 0f)
        {
            L.offset += L.tileWidth;
        }

        // Calcula e aplica posições
        float ax = L.baseAX - L.offset;
        float bx = ax + L.tileWidth;
        float ay = L.baseAY;
        float by = L.baseBY;

        SetPosition(L.tileA, ax, ay, L);
        SetPosition(L.tileB, bx, by, L);
    }

    // -----------------------------------------
    // Define posição com opção de pixel snapping
    // -----------------------------------------
    void SetPosition(Transform t, float x, float y, Layer L)
    {
        if (!L.pixelSnap)
        {
            t.position = new Vector3(x, y, t.position.z);
            return;
        }

        float unitsPerPixel = 1f / Mathf.Max(1f, L.pixelsPerUnit);
        float snappedX = Mathf.Round(x / unitsPerPixel) * unitsPerPixel;
        float snappedY = Mathf.Round(y / unitsPerPixel) * unitsPerPixel;
        t.position = new Vector3(snappedX, snappedY, t.position.z);
    }
}
