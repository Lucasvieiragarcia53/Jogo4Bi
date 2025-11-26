using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public float vidaMaxima = 1000f; 
    private float vidaAtual;

    [Header("Interface (UI)")]
    public Slider barraDeVida;         
    public Image barraDeVidaImagem;    
    public Gradient corDaBarra;        
    public Bullet bullet;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();

            if (bullet != null)
            {
                float dano = bullet.dano;

                vidaAtual -= dano;
                vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);

                AtualizarBarra();

                if (vidaAtual <= 0)
                {
                    Morrer();
                }
            }
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
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }
}
