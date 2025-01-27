using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTransitionManager : MonoBehaviour
{
    public static LevelTransitionManager Instance;

    public GameObject transitionPanel; // Il pannello di transizione
    public Text transitionText;        // Il testo da mostrare sul pannello
    public Text difficultyText;        // Testo della difficoltà
    public float transitionTime = 2f;  // Tempo di transizione

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Assicurati che il pannello sia nascosto all'avvio
        if (transitionPanel != null)
        {
            transitionPanel.SetActive(false);
        }
    }

    // Mostra il pannello di transizione
    public void ShowTransitionPanel(string levelName, int difficulty)
    {
        Debug.Log(levelName);

        if (transitionPanel != null && transitionText != null && difficultyText != null)
        {
            difficulty++;
            transitionPanel.SetActive(true);
            transitionText.text = $"Level {levelName} Starting!";
            difficultyText.text = $"Difficulty: {difficulty}";

            StartCoroutine(FadeAndLoadLevel(levelName, transitionTime));
        }
        else
        {
            Debug.LogError("[LevelTransitionManager] Riferimenti mancanti nel pannello di transizione.");
        }
    }

    // Coroutine per la dissolvenza e il caricamento del livello
    private IEnumerator FadeAndLoadLevel(string levelName, float delay)
    {
        yield return new WaitForSeconds(delay); // Aspetta il tempo di transizione

        // Notifica il GameController che la transizione è completata
        if (GameController.Instance != null)
        {
            GameController.Instance.OnTransitionComplete();
        }
        else
        {
            Debug.LogError("[LevelTransitionManager] GameController.Instance è null!");
        }

        HideTransitionPanel(); // Nascondi il pannello di transizione
    }


    // Nascondi il pannello di transizione
    public void HideTransitionPanel()
    {
        if (transitionPanel != null)
        {
            transitionPanel.SetActive(false);
        }
    }
}