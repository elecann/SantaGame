using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour, ILifeManager
{
    public static LifeManager instance; // Singleton

    private int lives; // Numero di vite correnti

    void Awake()
    {
        // Configurazione Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantieni il LifeManager tra le scene
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetInitialLives(int initialLives)
    {
        lives = initialLives;
        UIManager.Instance.UpdateLives(lives);
    }
    void Start()
    {
        SetInitialLives(0);  // Imposta il numero iniziale di vite a 3
    }

    // Restituisce il numero corrente di vite
    public int GetLife()
    {
        return lives;
    }

    // Aggiunge un numero specifico di vite
    public void AddLife()
    {
        lives++;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateLives(lives);
        }
        else
        {
            Debug.LogError("UIManager.Instance è null!");
        }
        Debug.Log("Vite aggiunte. Vite attuali: " + lives);
    }

    // Rimuove un numero specifico di vite
    public void RemoveLife()
    {
        lives--;

        if (lives <= 0)
        {
            lives = 0;
            Debug.Log("Vite esaurite. Game Over!");
            GameController.Instance.EndGame(); // Notifica il GameController
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateLives(lives); // Aggiorna la UI
        }
        else
        {
            Debug.LogError("UIManager.Instance è null!");
        }
        Debug.Log("Vite rimosse. Vite rimanenti: " + lives);
    }
}
