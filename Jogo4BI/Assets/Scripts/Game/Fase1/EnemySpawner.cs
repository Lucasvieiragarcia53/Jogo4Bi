using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // Prefab do inimigo (arraste no Inspector)
    [SerializeField] private GameObject boxPrefab; // Prefab da caixa (arraste no Inspector) - nova prefab
    [SerializeField] private Transform player; // Referência ao Transform do player
    [SerializeField] private BeatManager beatManager; // Referência ao BeatManager
    [SerializeField] private float spawnDistance = 2f; // Distância à frente do player para spawnar
    [SerializeField] private float groundY = -3.734066f; // Altura do chão (ajuste no Inspector se necessário)
    [SerializeField] private float boxOffset = 0.5f; // Offset para a caixa aparecer acima do chão (ajuste no Inspector)
    [SerializeField] private int beatsBetweenSpawns = 4; // Quantos beats entre spawns (ajuste para ritmo)

    private float lastBeat; // Último beat em que spawnou
    private int beatCounter = 0; // Contador de beats
    private bool spawnEnemyNext = true; // Alterna entre inimigo e caixa (true = inimigo, false = caixa)

    private void Start()
    {
        lastBeat = beatManager.GetCurrentBeat();

        if (beatManager == null)
        {
            beatManager = FindObjectOfType<BeatManager>();
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform; // Assumindo tag "Player"
        }
    }

    private void Update()
    {
        // Calcular o beat atual (usando o mesmo cálculo do BeatManager)
        float currentBeat = beatManager.GetCurrentBeat();

        // Verificar se passou um beat inteiro
        if (Mathf.Floor(currentBeat) > Mathf.Floor(lastBeat))
        {
            beatCounter++;
            lastBeat = currentBeat;

            // Spawnar a cada 'beatsBetweenSpawns' beats
            if (beatCounter >= beatsBetweenSpawns)
            {
                SpawnObject(); // Renomeado para SpawnObject, pois pode ser inimigo ou caixa
                beatCounter = 0; // Resetar contador
            }
        }
    }

    private void SpawnObject()
    {
        if (player == null) return;

        // Decidir o que spawnar (inimigo ou caixa) e definir isBox
        bool isBox = !spawnEnemyNext; // Se spawnEnemyNext for true, spawna inimigo (isBox = false); senão, caixa (isBox = true)
        GameObject prefabToSpawn;

        if (spawnEnemyNext)
        {
            prefabToSpawn = enemyPrefab;
            spawnEnemyNext = false; // Próximo será caixa
        }
        else
        {
            prefabToSpawn = boxPrefab;
            spawnEnemyNext = true; // Próximo será inimigo
        }

        // Agora calcular a posição de spawn (usando isBox, que já foi definida)
        float spawnX = player.position.x + (player.localScale.x > 0 ? spawnDistance : -spawnDistance); // Direção baseada na escala do player
        float spawnY = groundY + (isBox ? boxOffset : 0f); // Adicionar offset apenas para a caixa
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, player.position.z);

        if (prefabToSpawn == null) return;

        // Instanciar o objeto
        GameObject newObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // Se for uma caixa, adicionar movimento horizontal
        if (isBox)
        {
            newObject.AddComponent<BoxMover>();
        }
    }
}
