using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Manager : MonoBehaviour
{
    public GameObject coalPrefab;
    public float initialSpeedMultiplier = 1.5f;  // Moltiplicatore iniziale della velocità
    public float speedMultiplierIncrease = 0.2f; // Incremento per ogni difficoltà
    private float currentSpeedMultiplier;       // Moltiplicatore attuale della velocità
    public float spawnHeight = 5f;
    private float screenLimitX;
    private Coroutine spawnCoroutine;
    private GameController gameController;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Level3")
        {
            Debug.Log("[Level3Manager] Non siamo nel livello 3. Disattivo il manager.");
            enabled = false; // Disabilita questo script
            return;
        }

        gameController = GameController.Instance;
        if (gameController == null)
        {
            Debug.LogError("GameController non trovato!");
        }

        // Calcola i limiti dello schermo
        Camera cam = Camera.main;
        screenLimitX = cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x;

        // Inizia lo spawn degli oggetti speciali
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnObjects());
        }

        // Iscriviti all'evento di difficoltà del GameController
        GameController.OnDifficultyIncreased += HandleDifficultyIncrease;
        GameController.OnGameEnd += CheckForWin;
    }

    private void OnDestroy()
    {
        // Disiscriviti dall'evento per evitare errori
        GameController.OnDifficultyIncreased -= HandleDifficultyIncrease;
        GameController.OnGameEnd -= CheckForWin;
    }
    private void CheckForWin()
    {
        // Verifica se il giocatore ha completato il livello 3 e ha ancora vite
        if (LifeManager.instance.GetLife() > 0)
        {
            Debug.Log("[Level3Manager] Livello 3 completato con successo. Mostro il pannello di vincita.");
            UIManager.Instance.ShowWinPanel();
        }
    }
    private void HandleDifficultyIncrease(int newDifficulty)
    {
        Debug.Log($"[Level3Manager] Difficoltà aumentata a {newDifficulty}. Nuovo moltiplicatore di velocità: {currentSpeedMultiplier}");
    }
    private IEnumerator SpawnObjects()
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
            yield return new WaitForSeconds(gameController.spawnInterval);
        }
    }

    private void SpawnSingleObject()
    {
        float randomX = Random.Range(-screenLimitX, screenLimitX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0f);
        GameObject obj = Instantiate(coalPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Oggetto speciale spawnato a: " + spawnPosition);
    }

    public void StopSpawningObjects()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    
}