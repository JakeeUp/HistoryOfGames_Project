using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuButtons : MonoBehaviour
{
    public void OnePlayerMode()
    {
        SaveScript.Player1Mode = true;

        SceneManager.LoadScene(1);
    }
    public void ControlsMenu()
    {
        SceneManager.LoadScene(2);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ExitGame()
    {
        Application.Quit();
    } 
}
