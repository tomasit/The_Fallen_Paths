using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private bool _enable = true;
    public static bool isPaused = false;
    public GameObject PauseUI;
    public GameObject optionPanel;
    public GameObject pausePanel;
    public int gameMainMScene;

    public void Enable(bool state)
    {
        _enable = state;
    }

    private void Update()
    {
        if (!_enable) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
                optionPanel.gameObject.SetActive(false);
                pausePanel.gameObject.SetActive(true);

            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        PauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void StartGame()
    {
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(gameMainMScene);
    }

    public void QuitGame()
    {
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Application.Quit();
    }
}
