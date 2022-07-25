using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void HighscorePanel()
    {
        SceneManager.LoadScene(3);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void SettingsPanel()
    {
        SceneManager.LoadScene(4);
    }

    public void StatisticsPanel()
    {
        SceneManager.LoadScene(5);
    }
}
