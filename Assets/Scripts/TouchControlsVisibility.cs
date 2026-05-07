using System;
using UnityEngine;

public class TouchControlsVisibility : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(true);
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;
    }

    private void GameManager_OnGameUnpaused(object sender, EventArgs e)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.State == GameManager.GameState.GameOver)
            gameObject.SetActive(false);
    }
}
