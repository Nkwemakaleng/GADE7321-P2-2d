using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Shotgun : MonoBehaviour
{
    public int numPellets = 5; // Number of pellets in the shotgun spread
    public float spreadAngle = 30f; // Angle of spread for the shotgun pellets
    public float bulletSpeed = 10f; // Speed of each pellet

    void Start()
    {
        // Start a coroutine to spread the shotgun pellets
        StartCoroutine(SpreadPellets());
    }

    IEnumerator SpreadPellets()
    {
        for (int i = 0; i < numPellets; i++)
        {
            // Calculate the rotation for each pellet
            float randomAngle = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + randomAngle);

            // Create and shoot the pellet
            GameObject pellet = Instantiate(gameObject, transform.position, rotation);
            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
            rb.velocity = pellet.transform.right * bulletSpeed;

            // Destroy the pellet after a short delay
            Destroy(pellet, 2f);

            // Wait a short delay before spawning the next pellet
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Destroy the player upon collision with the shotgun pellets
            Destroy(collision.gameObject);
        }

        // Destroy the shotgun pellet upon collision
        Destroy(gameObject);
    }
}

