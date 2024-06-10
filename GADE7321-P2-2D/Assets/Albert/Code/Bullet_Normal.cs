using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : MonoBehaviour
{
    Rigidbody2D rb;

    public int damage = 20;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player1 player = collision.gameObject.GetComponent<Player1>();
            if (player != null)
            {
                player.BulletHit(damage); // Apply damage to the player
            }
            // Destroy the bullet 
            Destroy(this.gameObject, 5f) ;
        }
    }



}