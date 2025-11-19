using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VoltaraoMenu : MonoBehaviour
{
    public Button voltar;
  public void Voltar()
  {
    SceneManager.LoadScene("Menu");
  }
}
