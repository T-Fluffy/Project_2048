using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonActions : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
