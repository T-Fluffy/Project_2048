using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonActions2048 : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void LoadMenu(){
        SceneManager.LoadScene("StartUI");
    }
}
