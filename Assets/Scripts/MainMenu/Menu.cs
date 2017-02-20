using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    public Canvas MainCanvas;
    public Canvas OptionsCanvas;

    void Awake()
    {
        if (OptionsCanvas == null)
            return;
        OptionsCanvas.enabled = false;
    }

    public void OptionsOn()
    {
        OptionsCanvas.enabled = true;
        MainCanvas.enabled = false;
    }

    public void ReturnOn()
    {
        OptionsCanvas.enabled = false;
        MainCanvas.enabled = true;
    }

    public void LoadOn()
    {
        SceneManager.LoadScene("GameUI");
    }
    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ReturnToAudio()
    {
        SceneManager.LoadScene("Audio");
    }
    public void PauseQuest()
    {
        SceneManager.LoadScene("Quest");
    }
    public void PauseInv()
    {
        SceneManager.LoadScene("Inventory");
    }
    public void PauseSK()
    {
        SceneManager.LoadScene("Skill_Tree");
    }
    public void PauseMap()
    {
        SceneManager.LoadScene("Map");
    }
    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelMenuSelection");
    }
    public void WIPSelect()
    {
        SceneManager.LoadScene("WIP");
    }

    public void GoToShowcase()
    {
        SceneManager.LoadScene("Showcase");
    }
    public void MenuProgress()
    {
        SceneManager.LoadScene("Progress");
    }
}
