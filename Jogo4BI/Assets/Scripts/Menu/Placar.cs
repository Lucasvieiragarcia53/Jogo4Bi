using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Placar : MonoBehaviour
{
    public int vidas = 0;
    public TextMeshProUGUI vidastxt;

    public GameObject player;
    private Player playerScript;

    // ✅ Atualiza as vidas (chamado pelo Player)
    public void AtualizarVidas(int vida)
    {
        vidastxt.text = "Vidas: " + vida.ToString();
    }

   
}
