using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour, IFallingObject
{
    [SerializeField] private AudioSource bonusSound;
    [SerializeField] public float fallSpeed = 2f;

    void Update()
    {
        Fall(fallSpeed);
    }

    public void Fall(float speed)
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < -Camera.main.orthographicSize - 1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Santa"))
        {
            LifeManager lifeManager = FindObjectOfType<LifeManager>();
            if (lifeManager != null)
            {
                lifeManager.AddLife();
            }

            if (bonusSound != null && !bonusSound.isPlaying)
            {
                bonusSound.Play();
            }

            Destroy(gameObject);
        }
    }
    public void OnCollect()
    {
        // Eventuale comportamento personalizzato per la raccolta
        Debug.Log("Stella raccolta.");
    }

}