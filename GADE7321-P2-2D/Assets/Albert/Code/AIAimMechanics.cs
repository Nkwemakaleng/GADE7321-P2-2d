using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AIAimMechanics : MonoBehaviour
{
   
    public Transform bulletPoint;
    public float initialPower = 10f;
    public float maxPower = 100f;
    public float powerIncrement = 1f;
    public GameObject TargetPrefab;
    public AiManager aiManager;
    public TextMeshProUGUI powerValueText;

    private Vector3 direction;
    private Transform target;
    private float currentPower;
    private bool shouldIncreasePower = false;

    [SerializeField] private BulletManager bulletManager;
    private void Start()
    {
        currentPower = initialPower;
        UpdatePowerValueText();
        TargetMovement.MovementCycleComplete += OnMovementCycleComplete;
    }

    private void OnDestroy()
    {
        TargetMovement.MovementCycleComplete -= OnMovementCycleComplete;
    }

    private void Update()
    {
        if (aiManager.aiTurn)
        {
            AIUpdate();
        }
        // Adjust the cannon direction to look at the target
        if (target != null)
        {
            Vector3 cannonPosition = transform.position;
            direction = target.position - cannonPosition;
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

        // Ensure the target is on the player object
        if (target == null)
        {
            target = Instantiate(TargetPrefab, playerObject.transform).transform;
        }

        // Adjust power based on the distance and determine if the shot will hit the player
        AdjustPower();

        if (WillHitPlayer(playerObject.transform.position))
        {
            Shoot();
            Debug.Log("Shot!");
            aiManager.aiTurn = false;
        }
        else if (shouldIncreasePower)
        {
            currentPower += powerIncrement; // Increment power if the shot will not hit
            UpdatePowerValueText();
            shouldIncreasePower = false; // Reset the flag
        }
    }

    public void Shoot()
    {
        GameObject currentBullet = bulletManager.GetCurrentBullet();
        bulletManager.updateBulletText();
        if (currentBullet != null)
        {
            if (bulletManager.GetCurrentAmount() > 0 )//bulletManager.bulletTypes[bulletManager.GetCurrentBulletIndex()].maxBullets)
            {
                GameObject newBullet = Instantiate(currentBullet, bulletPoint.position, Quaternion.identity);
                Rigidbody2D bulletRB = newBullet.GetComponent<Rigidbody2D>();
                if (bulletRB != null)
                {
                    bulletRB.velocity = direction.normalized * currentPower;
                }
                bulletManager.ReduceBulletCount();
                bulletManager.updateBulletText();
            }
            else
            {
                Debug.Log("Out of " + bulletManager.bulletTypes[bulletManager.GetCurrentBulletIndex()].bulletName.text);
            }

        }
        else
        {
            Debug.LogWarning("No bullet Selected!");
        }
    }

    private void AdjustPower()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        currentPower = Mathf.Clamp(distance / 10f, initialPower, maxPower);
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
            if (Vector3.Distance(predictedPosition, aiManager.gameObject.transform.position) > 100f) // Adjust threshold as needed
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

    private void OnMovementCycleComplete()
    {
        shouldIncreasePower = true;
    }
}
