using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string firstLevelName = "Level1"; // Nome del primo livello
    public string gameOverSceneName = "GameOver"; // Nome della scena Game Over
    private string lastLoadedScene = "";

    // Riferimenti agli oggetti persistenti
    public GameObject gameControllerPrefab;
    public GameObject lifeManagerPrefab;
    private static LevelManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Rendi il LevelManager persistente
        }
        else
        {
            Destroy(gameObject); // Distruggi duplicati
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[LevelManager] Scena caricata: {scene.name}");

        lastLoadedScene = scene.name;

        // Usa l'istanza del GameController
        if (GameController.Instance != null)
        {
            Debug.Log("GameController trovato correttamente tramite Instance.");
            Debug.Log("Avvio automatico in " + scene.name);
            GameController.Instance.StartGame();
        }
        else
        {
            Debug.LogError("[LevelManager] ERRORE: GameController.Instance è null!");
            StartCoroutine(WaitForGameController());
        }
    }
    private IEnumerator WaitForGameController()
    {
        Debug.Log("In attesa del GameController...");

        // Aspetta che il GameController sia disponibile
        while (GameController.Instance == null)
        {
            yield return null; // Aspetta il frame successivo
        }

        Debug.Log("GameController trovato. Avvio lo spawn.");
        GameController.Instance.StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Caricamento del primo livello.");
        SceneManager.LoadScene(firstLevelName);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            Debug.Log("listener distrutto");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    public void LoadGameOver()
    {
        // Carica la scena Game Over
        UIManager.Instance.ShowGameOverPanel();
    }
    
    
    public void QuitGame()
    {
        // Esci dal gioco
        Debug.Log("Uscita dal gioco.");
        Application.Quit();
    }
}