using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// O que código faz:
/// Controla um parallax infinito e contínuo para múltiplas camadas 2D.
/// OBS: apesar do nome "3Layers", o exemplo abaixo está configurado com 2 camadas


/// É só adicionar outra instância de Layer (ex.: distantLayer) para ter 3

public class InfiniteParalaxLayers : MonoBehaviour
{
    // ------------------------------
    // Estrutura de uma camada (Layer)
    // ------------------------------
    [System.Serializable] // Permite que a classe apareça no
   
public class Layer
    {
        public Transform tileA; // Referência ao primeiro tile (geralmente
     
public Transform tileB; // Referência ao segundo tile (mesma sprite

public float speed = 2f; // Velocidade de deslocamento da camada
        public bool pixelSnap = false; // Se true, ativa arredondamento para grade
     
public float pixelsPerUnit = 100f;// Pixels Por Unidade (deve casar com o PPU

// Calculadas/armazenadas em tempo de execução:
[HideInInspector] public float tileWidth; // Largura do sprite em unidades do
   
[HideInInspector] public float baseAX; // Posição X inicial do tile A
    
[HideInInspector] public float baseAY; // Posição Y inicial do tile A
        
[HideInInspector] public float baseBX; // Posição X inicial do tile B
        [HideInInspector] public float baseBY; // Posição Y inicial do tile B
        [HideInInspector] public float offset; // Deslocamento cíclico acumulado
    
}
    public Layer fastLayer = new Layer { speed = 2.0f }; // Camada “próxima” ao
  
public Layer slowLayer = new Layer { speed = 0.8f }; // Camada “distante”
  
void Start()
    {
        // Inicializa cada camada configurando largura, posições base e offset
        InitLayer(fastLayer);
        InitLayer(slowLayer);
    }
    void LateUpdate() // Executa após a câmera: ajuda a reduzir tearing visual
    {
        // Atualiza o deslocamento contínuo de cada camada

        UpdateLayer(fastLayer);
        UpdateLayer(slowLayer);
    }
    void InitLayer(Layer L)
    {
        // Verifica se referências aos tiles foram atribuídas
        if (L.tileA == null || L.tileB == null)
        {
            Debug.LogWarning("[Parallax] Tile A/B não atribuídos.");
            return;
        }
        // Tenta obter a largura do sprite pelo SpriteRenderer do A;
        // se não encontrar, tenta pelo B; se ainda não, usa distância entre A e B.
        SpriteRenderer sr = L.tileA.GetComponent<SpriteRenderer>();
        if (sr == null) sr = L.tileA.GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            sr = L.tileB.GetComponent<SpriteRenderer>();
            if (sr == null) sr = L.tileB.GetComponentInChildren<SpriteRenderer>();
        }
        if (sr != null)
        {
            // bounds.size.x retorna a largura em unidades de mundo (já considera escala
          
        L.tileWidth = sr.bounds.size.x;
        }
        else
        {
            // Fallback: usa a distância atual entre os dois tiles
            L.tileWidth = Mathf.Abs(L.tileB.position.x - L.tileA.position.x);
        }
        // Garante que o tile B fique exatamente 1 largura à direita do A
        L.tileB.position = new Vector3(
        L.tileA.position.x + L.tileWidth,
        L.tileB.position.y,
        L.tileB.position.z
        );
        // Salva as posições base (X e Y) de A e B
        // Assim podemos recomputar posições determinísticas a cada frame
        L.baseAX = L.tileA.position.x;
        L.baseAY = L.tileA.position.y;
        L.baseBX = L.tileB.position.x;
        L.baseBY = L.tileB.position.y;

        // Offset começa em zero — ponto crucial para o loop contínuo sem “saltos”
        L.offset = 0f;
    }
    // --------------------------------------------------------------
    // Atualiza a camada aplicando offset cíclico contínuo (sem jumps)
    // --------------------------------------------------------------
    void UpdateLayer(Layer L)
    {
        // Validação de segurança: só atualiza se os tiles existem e a largura é
     
    if (L.tileA == null || L.tileB == null || L.tileWidth <= 0f) return;
        // 1) Avança (ou recua) o offset com base na velocidade e no tempo do frame
        // speed > 0 => cenário anda para a ESQUERDA
        // speed < 0 => cenário anda para a DIREITA
        L.offset += L.speed * Time.deltaTime;
        // 2) Mantém o offset sempre dentro de [0, tileWidth)
        // - Quando passa de tileWidth, “volta” subtraindo tileWidth
        // - Quando fica negativo, “avança” somando tileWidth
        if (L.offset >= L.tileWidth)
        {
            L.offset -= L.tileWidth;
        }
        else if (L.offset < 0f)
        {
            L.offset += L.tileWidth;
        }
        // 3) Calcula as novas posições X determinísticas a partir das bases:
        // A.x = baseA - offset
        // B.x = A.x + tileWidth
        float ax = L.baseAX - L.offset;
        float bx = ax + L.tileWidth;
        // Mantém os Y originais das bases (não movemos em Y)
        float ay = L.baseAY;
        float by = L.baseBY;
        // 4) Aplica as posições nos transforms (com opção de pixel snapping)
        SetPosition(L.tileA, ax, ay, L);
        SetPosition(L.tileB, bx, by, L);
    }
    // -----------------------------------------
    // Define posição com opção de pixel snapping
    // -----------------------------------------
    void SetPosition(Transform t, float x, float y, Layer L)
    {

        // Se não for pixel art, aplica posição diretamente (com Z original)
        if (!L.pixelSnap)
        {
            t.position = new Vector3(x, y, t.position.z);
            return;
        }
        // Para pixel art: arredonda X e Y para a grade 1/PPU
        // Isso evita “linhas” finas entre tiles devido a sub-pixel rendering
        float unitsPerPixel = 1f / Mathf.Max(1f, L.pixelsPerUnit);
        float snappedX = Mathf.Round(x / unitsPerPixel) * unitsPerPixel;
        float snappedY = Mathf.Round(y / unitsPerPixel) * unitsPerPixel;
        // Aplica a posição “snapada” mantendo o Z atual
        t.position = new Vector3(snappedX, snappedY, t.position.z);
    }
}