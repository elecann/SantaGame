using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class GameController : MonoBehaviour
{
    public static GameController Instance; // Singleton

    [Header("Game Settings")]
    public GameObject giftPrefab;
    public Transform santa;
    public float spawnHeight = 5f;
    public float spawnInterval = 1f;
    public float gameDuration = 30f;

    [Header("UI Settings")]
    public Button restartButton;
    public Button pauseButton;
    public GameObject bonusLifePrefab;

    [Header("Spawn Interval Settings")]
    public float spawnIntervalDecrement = 0.05f;
    public float minSpawnInterval = 0.5f;

    [Header("Transition Settings")]
    public float transitionDelay = 2f;
    private int currentLevelIndex = 1;
    public int difficulty = 2;

    private float timer;
    private float currentSpawnInterval;
    private float screenLimitX;
    private int score;

    private Coroutine giftSpawnerCoroutine;
    private Coroutine starSpawnerCoroutine;
    private bool isGameRunning;
    private bool isPaused;
    private int giftsCollected = 0;
    private GameController gameController;
    [Header("Difficulty Settings")]
    public float initialCoalSpawnInterval = 1f; // Intervallo iniziale di spawn dei carboni
    public float coalSpawnIntervalDecrement = 0.1f; // Quanto diminuire l'intervallo per ogni difficoltà
    public float minCoalSpawnInterval = 0.5f; // Intervallo minimo di spawn

    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnDifficultyIncreased;
    public static event Action OnGameEnd;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Calcola i limiti orizzontali dello schermo
        float screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        screenLimitX = screenHalfWidth;
    }

    void Start()
    {
        Debug.Log("[GameController] Start method called.");
        InitializeSceneObjects();
        restartButton.gameObject.SetActive(false);
        currentSpawnInterval = spawnInterval;
        StartCoroutine(WaitForGameController());
        StartGame();

    }

    private IEnumerator WaitForGameController()
    {
        while (GameController.Instance == null)
        {
            yield return null; // Aspetta il frame successivo
        }
        gameController = GameController.Instance;
        Debug.Log("[Gift] GameController assegnato correttamente.");
    }

    void InitializeSceneObjects()
    {
        if (santa == null)
        {
            santa = GameObject.FindWithTag("Santa").transform;

            if (santa == null)
            {
                Debug.LogError("Santa non trovato! Assicurati che ci sia un oggetto con il tag 'Santa' nella scena.");
            }
        }
    }

    public void OnGiftCollected()
    {
        giftsCollected++;

        // Ogni 5 regali, aggiungi una vita
        if (giftsCollected >= 5)
        {
            giftsCollected = 0; // Resetta il contatore
            LifeManager.instance.AddLife(); // Aggiungi una vita
        }
    }

    public void Update()
    {
        if (isGameRunning)
        {
            timer -= Time.deltaTime;
            UIManager.Instance.UpdateTimer(timer);
            

            if (timer <= 0)
            {
                Debug.Log("[GameController] Timer scaduto. Controllo le vite...");
                if (LifeManager.instance.GetLife() <= 0)
                {
                    Debug.Log("[GameController] Fine del gioco: vite esaurite.");
                    EndGame();
                }
                else
                {
                    Debug.Log("[GameController] Caricamento del livello successivo...");
                    LoadNextLevel();
                }
            }

            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnIntervalDecrement * Time.deltaTime);
        }
    }
    public void StartGame()
    {
        Debug.Log("Gioco avviato.");
        isGameRunning = true;
        timer = gameDuration;
        score = 0;

        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateTimer(timer);

        StopAllCoroutines();
        Debug.Log("Avvio dello spawn dei regali.");
        giftSpawnerCoroutine = StartCoroutine(SpawnGifts());
        starSpawnerCoroutine = StartCoroutine(SpawnStars());
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    private IEnumerator SpawnGifts()
    {
        while (isGameRunning)
        {
            while (isPaused)
            {
                yield return null;
            }

            SpawnObject(giftPrefab);
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private IEnumerator SpawnStars()
    {
        while (isGameRunning)
        {
            while (isPaused)
            {
                yield return null;
            }

            if (LifeManager.instance.GetLife() >= 5)
            {
                SpawnObject(bonusLifePrefab);
            }

            yield return new WaitForSeconds(5f);
        }
    }

    public void SpawnObject(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab non assegnato!");
            return;
        }

        Vector3 spawnPosition = GameUtilities.GetRandomSpawnPosition(screenLimitX, spawnHeight);
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    public void AddScore()
    {
        score++;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(score);
        }

        if (OnScoreChanged != null)
        {
            OnScoreChanged.Invoke(score);
        }
        else
        {
            Debug.LogWarning("[GameController] Nessun listener per OnScoreChanged.");
        }
    }

    public bool IsGameRunning()
    {
        return isGameRunning;
    }

    public void IncreaseDifficulty()
    {
        difficulty=+1;
        Debug.Log("Difficoltà aumentata: " + difficulty);

        // Riduci l'intervallo di spawn dei carboni
        initialCoalSpawnInterval = Mathf.Max(minCoalSpawnInterval, initialCoalSpawnInterval - coalSpawnIntervalDecrement);

        Debug.Log($"[GameController] Nuovo intervallo di spawn carboni: {initialCoalSpawnInterval}");
        // Notifica i listener dell'aumento della difficoltà
        if (OnDifficultyIncreased != null) // Controlla se l'evento è null
        {
            OnDifficultyIncreased.Invoke(difficulty);
        }
        else
        {
            Debug.LogWarning("[GameController] Nessun listener per OnDifficultyIncreased.");
        }
    }

    public void LoadNextLevel()
    {
        isGameRunning = false;
        isPaused = true;
        StopAllCoroutines();
        currentLevelIndex++;
        // Mostra il pannello di transizione
        if (LevelTransitionManager.Instance != null)
        {
            LevelTransitionManager.Instance.ShowTransitionPanel("Level" + (currentLevelIndex + 1), difficulty);
           
        }
        else
        {
            Debug.LogError("[GameController] LevelTransitionManager.Instance è null!");
        }
    }

    public void OnTransitionComplete()
    {
        // Carica il livello successivo
        string nextLevelName = "Level" + currentLevelIndex;
        if (Application.CanStreamedLevelBeLoaded(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
            StartCoroutine(StartGameAfterDelay(0.1f));
        }
        else
        {
            Debug.LogError($"[GameController] Il livello {nextLevelName} non esiste o non è nei Build Settings!");
            UIManager.Instance.ShowWinPanel(); // Mostra il pannello di vincita se non ci sono più livelli
        }
    }
    private IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Aspetta che la scena sia completamente caricata
        StartGame(); // Riavvia il gioco
    }
    public void EndGame()
    {
        isGameRunning = false;
        StopAllCoroutines();

        // Mostra il pannello di game over
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }
        else
        {
            Debug.LogError("UIManager.Instance è null!");
        }

        Debug.Log("Gioco terminato.");
    }
}
