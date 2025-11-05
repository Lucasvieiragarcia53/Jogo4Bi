using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public float vidaMaxima = 1000f;   // Vida total do boss
    private float vidaAtual;

    [Header("Interface (UI)")]
    public Slider barraDeVida;         // Slider (barra de HP)
    public Image barraDeVidaImagem;    // Alternativa: usar imagem em vez do Slider
    public Gradient corDaBarra;        // Gradiente de cor da barra (verde → vermelho)

    void Start()
    {
        vidaAtual = vidaMaxima;

        if (barraDeVida != null)
        {
            barraDeVida.maxValue = vidaMaxima;
            barraDeVida.value = vidaMaxima;
        }

        AtualizarCorDaBarra();
    }

    public void ReceberDano(float dano)
    {
        vidaAtual -= dano;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);


        AtualizarBarra();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void AtualizarBarra()
    {
        if (barraDeVida != null)
        {
            barraDeVida.value = vidaAtual;
        }

        AtualizarCorDaBarra();
    }

    void AtualizarCorDaBarra()
    {
        if (barraDeVidaImagem != null)
        {
            barraDeVidaImagem.color = corDaBarra.Evaluate(vidaAtual / vidaMaxima);
        }
    }

    void Morrer()
    {

        Debug.Log("O chefão foi derrotado!");
        Destroy(gameObject);
    }
}
