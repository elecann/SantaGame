using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    public GameObject coalPrefab;           // Prefab del carbone
    public float spawnHeight = 5f;          // Altezza di spawn dei carboni
    public float initialCoalSpawnInterval = 3f;    // Intervallo iniziale di spawn
    public float spawnIntervalDecrement = 0.2f;    // Quanto diminuire l'intervallo per ogni difficoltà
    public float minSpawnInterval = 0.5f;          // Intervallo minimo
    public int initialCoalCount = 1;               // Numero iniziale di carboni spawnati contemporaneamente
    public int coalCountIncreaseRate = 1;          // Aumento del numero di carboni per livello di difficoltà

    private GameController gameController;  // Riferimento al GameController
    private float screenLimitX;             // Limite orizzontale dello schermo
    private Coroutine coalSpawnCoroutine;
    private float currentCoalSpawnInterval; // Intervallo attuale
    private int currentCoalCount;           // Numero attuale di carboni spawnati contemporaneamente

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Level2")
        {
            Debug.Log("[Level2Manager] Non siamo nel livello 2. Disattivo il manager.");
            enabled = false; // Disabilita questo script
            return;
        }
        Debug.Log("[Level2Manager] Start method called.");
        gameController = GameController.Instance;
       
        if (GameController.Instance == null)
        {
            Debug.LogError("GameController non trovato nel Livello 2!");
        }
        else
        {
            Debug.Log("GameController trovato correttamente nel Livello 2.");
        }
        // Calcola i limiti dello schermo
        Camera cam = Camera.main;
        screenLimitX = cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x;

        // Inizia lo spawn dei carboni
        if (coalSpawnCoroutine == null)
        {
            coalSpawnCoroutine = StartCoroutine(SpawnCoal());
        }
        if (coalPrefab == null)
        {
            Debug.LogError("coalPrefab non assegnato!");
        }
        else
        {
            Debug.Log("coalPrefab assegnato correttamente.");
        }
        // Iscriviti all'evento di difficoltà del GameController
        GameController.OnDifficultyIncreased += HandleDifficultyIncrease;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level2")
        {
            Debug.Log("Livello 2 caricato. Avvio del gioco...");
            GameController.Instance.StartGame();
        }
    }

    private void OnDestroy()
    {
        // Disiscriviti dagli eventi per evitare errori
        GameController.OnDifficultyIncreased -= HandleDifficultyIncrease;
    }

    private void HandleDifficultyIncrease(int newDifficulty)
    {
        Debug.Log($"[Level2Manager] Difficoltà aumentata a {newDifficulty}. " );
    }

    private IEnumerator SpawnCoal()
    {
        while (gameController.IsGameRunning())
        {
            if (coalPrefab != null)
            {
                SpawnSingleObject();
            }
            else
            {
                Debug.LogError("specialObjectPrefab non assegnato!");
            }
            // Aspetta il nuovo intervallo di spawn
            yield return new WaitForSeconds(gameController.initialCoalSpawnInterval);
        }
    }

    private void SpawnSingleObject()
    {
        float randomX = Random.Range(-screenLimitX, screenLimitX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0f);
        GameObject coal = Instantiate(coalPrefab, spawnPosition, Quaternion.identity);
      
    }

    public void StopSpawningObjectl()
    {
        if (coalSpawnCoroutine != null)
        {
            StopCoroutine(coalSpawnCoroutine);
            coalSpawnCoroutine = null;
        }
    }
}