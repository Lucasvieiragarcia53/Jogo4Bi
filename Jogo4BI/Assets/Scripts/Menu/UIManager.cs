using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class UIManager : MonoBehaviour
{
    [Header("Botões do Menu")]
    public Button start;
    public Button credits;
    public Button options;
    public Button exit;

    // Funções dos botões
    public void Jogar()
    {
        SceneManager.LoadScene("Fase 1");
    }

    public void Opcoes()
    {
        SceneManager.LoadScene("Opções");
    }

    public void Creditos()
    {
        SceneManager.LoadScene("Crédito"); 
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
    }
}
