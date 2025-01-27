using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Santa1Controller : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocità di movimento
    private float screenLimitX; // Limite orizzontale dello schermo
    private Animator animator;
    private Rigidbody2D rb;
    void Start()
    {
        // Calcola i limiti della viewport
        Camera cam = Camera.main;
        screenLimitX = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D non trovato su Santa!");
        }
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Ottieni input orizzontale
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(moveInput * moveSpeed * Time.deltaTime, 0, 0);
        transform.position += move;


        // Mantieni Santa entro i limiti della viewport
        float clampedX = Mathf.Clamp(transform.position.x, -screenLimitX, screenLimitX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
       
        if (animator != null)
        {
            animator.SetBool("IsMoving", moveInput != 0); // Imposta `isMoving` a true solo quando Santa si muove
        }
    }
}