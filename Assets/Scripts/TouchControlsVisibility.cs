using System;
using UnityEngine;

public class TouchControlsVisibility : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(true);
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.State == GameManager.GameState.GameOver)
            gameObject.SetActive(false);
    }
}
