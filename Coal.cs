using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coal : MonoBehaviour, IFallingObject
{
    [SerializeField] private AudioSource coalSound;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D coalCollider;
    [SerializeField] private float fallSpeed = 2f; // Velocità di caduta
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coalCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Santa"))
        {
            LifeManager lifeManager = FindObjectOfType<LifeManager>();
            if (lifeManager != null)
            {
                lifeManager.RemoveLife();
            }

            if (coalSound != null && !coalSound.isPlaying)
            {
                coalSound.Play();
            }
            else
            {
                Debug.LogError("coalSound non assegnato!");
            }

            MakeCoalInvisible();
            Destroy(gameObject, coalSound.clip.length);
        }
    }

    void Update()
    {
        Fall(fallSpeed);
    }
    public void Fall(float speed)
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }
    /*public void IncreaseFallSpeed(float multiplier)
    {
        fallSpeed *= multiplier;
    }*/

    void MakeCoalInvisible()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (coalCollider != null)
            coalCollider.enabled = false;
    }
    public void OnCollect()
    {
        // Eventuale comportamento personalizzato per la raccolta
        Debug.Log("Carbone raccolto.");
    }
}
