using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static System.TimeZoneInfo;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Text timerText;
    public Text scoreText;
    public Text livesText;

    // Pannelli UI
    public GameObject gameOverPanel;
    public GameObject winPanel;

    // Testi UI
    public Text gameOverText;
    public Text gameOver1Text;
    public TextMeshPro winText;

    // Pulsanti UI
    public Button pauseButton;
    public Button restartButton;

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
    }

    void Start()
    {
        InitializePanels();
        GameController.OnScoreChanged += UpdateScore;
    }

    // Inizializzazione dei pannelli
    private void InitializePanels()
    {
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(winPanel, false);

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartLevel);
        }
        else
        {
            Debug.LogError("Riferimento mancante per il pulsante Restart.");
        }
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            if (gameOverText != null) gameOverText.gameObject.SetActive(true);
            if (gameOver1Text != null) gameOver1Text.gameObject.SetActive(true);
            SetPanelActive(gameOverPanel, true);
            pauseButton.gameObject.SetActive(false);
            restartButton?.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Debug.LogError("[UIManager] Riferimento mancante per il pannello di game over.");
        }
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverText.gameObject.SetActive(false);
            gameOver1Text.gameObject.SetActive(false);
            SetPanelActive(gameOverPanel, false);
        }
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            if (winText != null) winText.gameObject.SetActive(true);
            SetPanelActive(winPanel, true);
        }
        else
        {
            Debug.LogError("[UIManager] Riferimento mancante per il pannello di vincita.");
        }
    }

    public void HideWinPanel()
    {
        if (winPanel != null)
        {
            winText.gameObject.SetActive(false);
            SetPanelActive(winPanel, false);
        }
    }

    // Aggiorna l'UI
    public void UpdateTimer(float time)
    {
        if (timerText != null)
        {
            timerText.text = $"Tempo: {Mathf.Ceil(time)}";
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {lives}";
        }
    }

    // Gestione pulsanti
    public void RestartLevel()
    {
        HideGameOverPanel();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Metodo di utilità per attivare/disattivare i pannelli
    private void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
        else
        {
            Debug.LogError("[UIManager] Pannello non assegnato.");
        }
    }

    private void OnDestroy()
    {
        GameController.OnScoreChanged -= UpdateScore;
    }
}