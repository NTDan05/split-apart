using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void UpdateTimer(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int wholeSeconds = Mathf.FloorToInt(seconds % 60);
        int fracSeconds = Mathf.FloorToInt(seconds * 100) % 100;
        timerText.text = $"Time: {minutes:D2}:{wholeSeconds:D2}.{fracSeconds:D2}";
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
