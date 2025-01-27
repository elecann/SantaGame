using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gift : MonoBehaviour, IFallingObject
{
    [SerializeField] AudioSource giftSound;

    [SerializeField] private SpriteRenderer spriteRenderer; // Renderer del regalo
    [SerializeField] private Collider2D giftCollider;       // Collider del regalo
    [SerializeField] private float fallSpeed = 2f; // Velocità iniziale di caduta

    private GameController gameController;  // Riferimento al GameController

    void Start()
    {
        // Trova il GameController all'inizio
        gameController = GameController.Instance;  // Usa il singleton del GameController
        if (gameController == null)
        {
            Debug.LogError("GameController non trovato!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Santa"))  // Assicurati che Santa abbia il tag "Santa"
        {
            if (giftSound != null && !giftSound.isPlaying)
            {
                gameController.AddScore(); // Aggiunge il punteggio
                giftSound.Play();
            }
            // Notifica il GameController che il regalo è stato raccolto
            gameController.OnGiftCollected();

            // Rendi il regalo invisibile e inattivo
            MakeGiftInvisible();

            // Distruggi l'oggetto dopo la durata del suono
            Destroy(gameObject, giftSound.clip.length);
        }
    }
    void Update()
    {
        // Movimento verso il basso
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    public void Fall(float newSpeed)
    {
        fallSpeed = newSpeed; // Imposta la nuova velocità di caduta
    }

    void MakeGiftInvisible()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false; // Disattiva il renderer per nascondere il regalo

        if (giftCollider != null)
            giftCollider.enabled = false;  // Disattiva il collider per evitare altre collisioni
    }


    public void OnCollect()
    {
        Debug.Log("regalo raccolto");
    }
}