using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class Player1 : MonoBehaviour
{
    //player health

    public int maxHealth = 100; // Maximum health of the player
    public int currentHealth; // Current health of the player
    public TMP_Text healthText; // Reference to the TextMeshPro text for health display

    public float respawnCooldown = 5f; // Cooldown duration for respawn
    public GameObject respawnPoint; // Respawn point for automatic respawn
    public float respawnHeight = 10f; // Height for respawning if player falls
    public KeyCode respawnKey = KeyCode.R; // Key to trigger manual respawn
    public int respawnAmounts;
    public int respawnAmountsLeft;
    public TMP_Text respawnAmountText;


    private bool isRespawning = false; // Flag to track if the player is respawning
    //movement
    public float moveSpeed = 5f; // Speed of player movement
    public int maxMovement = 100; // Maximum movement distance
    public bool playersturn = true;
    private int remainingMovement; // Remaining movement distance
    Rigidbody2D rb;
    
    // Jetpack
    public bool jetpackEnabled = true;
    public float jetpackForce = 5f;
    public KeyCode jetpackKey = KeyCode.E; // Key to trigger jetpack1
    private BulletManager bulletManager;
    private TurnBasedManager turnManager;

    void Start()
    {
        
        remainingMovement = maxMovement;
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        respawnAmountsLeft = respawnAmounts;
        UpdateHealthUI();
        UpdateRespawnText();

    }

    void Update()
    {
        // Check if it's the player's turn
        if (playersturn)
        {
            // Check if the player has remaining movement
            if (remainingMovement > 0)
            {
                // Get input for movement
                float horizontalInput = Input.GetAxis("Horizontal");

                // Calculate movement vector
                Vector3 movement = new Vector3(horizontalInput, 0f, 0f) * moveSpeed * Time.deltaTime;

                // Move the player
                transform.Translate(movement);

                // Decrease remaining movement based on horizontal movement
                remainingMovement -= Mathf.Abs((int)(horizontalInput * moveSpeed * Time.deltaTime));
                
                // Jetpack movement
                if (jetpackEnabled && Input.GetKey(jetpackKey))
                {
                    rb.AddForce(Vector2.up * jetpackForce, ForceMode2D.Force);
                }
            }
            else
            {
                // Disable movement if there's no remaining movement
                enabled = false;
            }
        }
        UpdateHealthUI();
        if (Input.GetKeyDown(respawnKey))
        {
            // Trigger manual respawn when R key is pressed
            Reset();
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            BulletHit(BulletHit(bulletManager.bulletTypes[bulletManager.GetCurrentBulletIndex()].damage)); // Apply the damage from the bullet
            Destroy(other.gameObject); // Optionally destroy the bullet on hit); 
            Debug.Log("Hit");
        }
    }
    // Method to handle player being hit by a bullet
    public int BulletHit(int damage)
    {
        if (!isRespawning)
        {
            currentHealth -= damage; // Decrease player's health by the damage amount
            UpdateHealthUI(); // Update health UI
            if (currentHealth <= 0)
            {
                Die(); // Die if health reaches zero
            }
        }
        return currentHealth;
        
    }

    // Method to handle player death
    void Die()
    {
        // Trigger respawn
        if (!isRespawning)
        {
            StartCoroutine(RespawnCooldown());
        }
    }


    // Coroutine for respawn cooldown
    IEnumerator RespawnCooldown()
    {
        isRespawning = true;
        yield return new WaitForSeconds(respawnCooldown);

        if (currentHealth <= 0)
        {
            Reset();
        }

        isRespawning = false;
    }

    // Method to reset player's position and rotation
    void Reset()
    {
        if (!isRespawning)
        {
            if (currentHealth <= 0)
            {
                if (respawnAmountsLeft > 0 )
                {
                    // Automatic respawn at respawn point if player dies
                    transform.position = respawnPoint.transform.position;
                    transform.rotation = Quaternion.identity;
                    respawnAmountsLeft -= 1;
                    UpdateRespawnText();
                }
                else
                {
                    Debug.Log("Not enough Lives Left");
                    DestroyThisObject();// win logic in this case 
                }
            }
            else
            {
                if (respawnAmountsLeft > 0)
                {
                    // Manual respawn at higher altitude
                    Vector3 respawnPos = new Vector3(transform.position.x, respawnHeight, transform.position.z);
                    transform.position = respawnPos;
                    transform.rotation = Quaternion.identity;
                    respawnAmountsLeft -= 1;
                    UpdateRespawnText();
                }
                else
                {
                    Debug.Log("No enough respawns left for automatic respawn");
                    DestroyThisObject();
                }
                
                
            }
            currentHealth = maxHealth; // Reset player's health
            UpdateHealthUI(); // Update health UI
            UpdateRespawnText();
        }
    }

    // Method to update health UI
    void UpdateHealthUI()
    {
        healthText.text = "Health: " + currentHealth.ToString();

    }

    void UpdateRespawnText()
    {
        
        respawnAmountText.text = "Respawn " + this.gameObject.name + " :"+ Mathf.Max(respawnAmountsLeft, 0).ToString();;
    }

    void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }

}