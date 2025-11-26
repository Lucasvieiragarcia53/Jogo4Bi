using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Placar : MonoBehaviour
{
    public int vidas = 5;
    public TextMeshProUGUI vidastxt;

    public GameObject player;
    private Player playerScript;

    void Start()
    {
        // Atualizar a UI com as vidas iniciais
        AtualizarVidas(vidas);
    }

    public void AtualizarVidas(int vida)
    {
        vidastxt.text = "Vidas: " + vida.ToString();
    }

    // Novo: Método para perder uma vida 
    public void PerderVida()
    {
        vidas--; // Reduz uma vida
        AtualizarVidas(vidas); // Atualiza a UI

        // Verificar se o player morreu
        if (vidas <= 0)
        {
            SceneManager.LoadScene("Game Over"); 
        }
    }
}