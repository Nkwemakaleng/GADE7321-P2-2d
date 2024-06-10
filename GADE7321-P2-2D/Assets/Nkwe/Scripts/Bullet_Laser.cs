using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Laser : MonoBehaviour
{
    public float laserSpeed = 20f; // Speed of the laser
    public float laserDuration = 2f;// Duration the laser stays active
    public int damage = 20;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * laserSpeed; // Move the laser in the direction it's facing

        // Destroy the laser after a certain duration
        Destroy(gameObject, laserDuration);
    }

    void Update()
    {
        // Update the laser's rotation based on its velocity
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Damage the player upon collision with the laser
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Damage the enemy upon collision with the laser
            Destroy(collision.gameObject);
        }

        // Destroy the laser upon collision
        Destroy(gameObject);
    }
}