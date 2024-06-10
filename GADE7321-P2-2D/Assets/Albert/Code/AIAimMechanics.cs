using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class AIAimMechanics : MonoBehaviour
{
    public GameObject Bullet1;
    public Transform bulletPoint;
    public float initialPower = 10f;
    public float maxPower = 100f;
    public float moveThreshold = 10f;
    public GameObject TargetPrefab;
    public AiManager aiManager;
    public TextMeshProUGUI powerValueText;

    private Vector3 direction;
    private GameObject empty;
    private float currentPower;

    private void Start()
    {
        currentPower = initialPower;
        UpdatePowerValueText();
    }

    private void Update()
    {
        if (aiManager.aiTurn)
        {
            AIUpdate();
        }
        // Adjust the cannon direction to look at the target
        if (TargetPrefab != null)
        {
            Vector3 cannonPosition = transform.position;
            direction = TargetPrefab.transform.position - cannonPosition;
            transform.right = direction;
        }
    }

    public void AIUpdate()
    {
        GameObject playerObject = GameObject.Find("Player1");

        // Check if player object exists
        if (playerObject == null)
        {
            Debug.LogWarning("Player1 GameObject not found!");
            return;
        }

        Vector3 playerPosition = playerObject.transform.position;
        Vector3 aiPosition = transform.position;

        // Calculate direction towards the player
        direction = playerPosition - aiPosition;

        // Spawn empty if it doesn't exist
        if (empty == null)
        {
            empty = Instantiate(TargetPrefab, playerPosition, Quaternion.identity);
        }

        // Move the empty upwards
        Vector3 targetUpPosition = empty.transform.position + Vector3.up * 10f;
        empty.transform.position = Vector3.MoveTowards(empty.transform.position, targetUpPosition, 6f * Time.deltaTime);

        // Adjust power based on the distance
        float distance = Vector3.Distance(aiPosition, playerPosition);
        AdjustPower(distance);

        // Determine if the shot will hit the player
        if (WillHitPlayer(playerPosition))
        {
            Shoot();
        }
        else
        {
            // Check if the empty has moved away
            float emptyDistance = Vector3.Distance(empty.transform.position, playerPosition);
            if (emptyDistance > moveThreshold)
            {
                // Set shouldMove to true in AiManager
                aiManager.SetShouldMove(true);
            }
        }
    }

    public void Shoot()
    {
        GameObject newBullet = Instantiate(Bullet1, bulletPoint.position, Quaternion.identity);
        Rigidbody2D bulletRB = newBullet.GetComponent<Rigidbody2D>();
        if (bulletRB != null)
        {
            bulletRB.velocity = direction.normalized * currentPower;
        }
    }

    private void AdjustPower(float distance)
    {
        currentPower = Mathf.Clamp(distance / 10f, initialPower, maxPower); // Adjust power based on distance
        UpdatePowerValueText();
    }

    private bool WillHitPlayer(Vector3 playerPosition)
    {
        Vector3 bulletVelocity = direction.normalized * currentPower;
        Vector3 gravity = new Vector3(Physics2D.gravity.x, Physics2D.gravity.y, 0f);
        Vector3 predictedPosition = bulletPoint.position;

        while (Vector3.Distance(predictedPosition, playerPosition) > 0.5f)
        {
            predictedPosition += bulletVelocity * Time.fixedDeltaTime;
            bulletVelocity += gravity * Time.fixedDeltaTime;

            // If predicted position moves too far, return false
            if (Vector3.Distance(predictedPosition, aiManager.gameObject.transform.position) > moveThreshold)
            {
                return false;
            }
        }
        return true;
    }

    private void UpdatePowerValueText()
    {
        powerValueText.text = currentPower.ToString("0");
    }
}
