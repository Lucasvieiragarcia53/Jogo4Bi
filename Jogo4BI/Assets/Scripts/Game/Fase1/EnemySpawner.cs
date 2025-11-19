using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // Prefab do inimigo (arraste no Inspector)
    [SerializeField] private Transform player; // Referência ao Transform do player
    [SerializeField] private BeatManager beatManager; // Referência ao BeatManager
    [SerializeField] private float spawnDistance = 2f; // Distância à frente do player para spawnar
    [SerializeField] private int beatsBetweenSpawns = 4; // Quantos beats entre spawns (ajuste para ritmo)

    private float lastBeat; // Último beat em que spawnou
    private int beatCounter = 0; // Contador de beats

    private void Start()
    {
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
                SpawnEnemy();
                beatCounter = 0; // Resetar contador
            }
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || player == null) return;

        // Calcular posição de spawn: à frente do player
        Vector3 spawnPosition = player.position + player.right * spawnDistance; // 'right' assume player virado para direita; ajuste se necessário

        // Instanciar o inimigo
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}