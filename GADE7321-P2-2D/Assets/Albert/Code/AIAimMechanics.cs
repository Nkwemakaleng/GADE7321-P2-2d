using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIAimMechanics : MonoBehaviour
{
    public GameObject Bullet1;
    public float Power = 10;
    public Slider PowerSlider;
    public Transform bulletPoint;
    public GameObject Point;
    public int numOfPoints = 10;
    public float spacing = 0.5f;
    public TextMeshProUGUI powerValueText;

    private GameObject[] points;
    private Vector3 direction;
    private AiManager aiManager;
    private Vector3 playerPosition;

    private void Start()
    {
        points = new GameObject[numOfPoints];
        for (int i = 0; i < numOfPoints; i++)
        {
            points[i] = Instantiate(Point, bulletPoint.position, Quaternion.identity);
        }
        PowerSlider.value = Power;
        UpdatePowerValueText();

        aiManager = GetComponent<AiManager>();
    }

    private void Update()
    {
        if (aiManager.aiTurn)
        {
            AIUpdate();
        }

        for (int i = 0; i < numOfPoints; i++)
        {
            points[i].transform.position = PointPosition(i * spacing);
        }
    }

    public void AIUpdate()
    {
        Vector3 aiPosition = transform.position;
        GameObject playerObject = GameObject.Find("Player1");

        // Check if player object exists
        if (playerObject == null)
        {
            Debug.LogWarning("Player1 GameObject not found!");
            return;
        }

        playerPosition = playerObject.transform.position;

        // Calculate direction towards the player
        direction = playerPosition - aiPosition;

        // Adjust direction for the arc
        float distance = Vector3.Distance(aiPosition, playerPosition);
        float timeToHit = Mathf.Sqrt((2 * distance) / Physics2D.gravity.magnitude);

        // Calculate the aim point based on the player's velocity
        Vector3 aimPoint = playerObject.transform.position + (Vector3)(playerObject.GetComponent<Rigidbody2D>().velocity * timeToHit);

        // Update direction based on the aim point
        direction = aimPoint - bulletPoint.position;

        // Rotate the AI towards the predicted position
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.right = direction.normalized;

        // Adjust power based on the distance
        AdjustPower(distance);

        // Determine if the shot will hit the player
        if (WillHitPlayer(playerPosition))
        {
            Shoot();
        }
    }


    public void Shoot()
    {
        GameObject newBullet = Instantiate(Bullet1, bulletPoint.position, Quaternion.identity);
        Rigidbody2D bulletRB = newBullet.GetComponent<Rigidbody2D>();
        if (bulletRB != null)
        {
            bulletRB.velocity = direction.normalized * Power;
        }
    }

    private Vector3 PointPosition(float t)
    {
        Vector3 gravity = new Vector3(Physics2D.gravity.x, Physics2D.gravity.y, 0f);
        Vector3 position = bulletPoint.position + direction.normalized * Power * t + 0.5f * gravity * (t * t);
        return position;
    }

    private void AdjustPower(float distance)
    {
        Power = Mathf.Clamp(distance / 10, 5, 20); // Adjust power based on distance
        PowerSlider.value = Power;
        UpdatePowerValueText();
    }

    private bool WillHitPlayer(Vector3 playerPosition)
    {
        Vector2 bulletVelocity = direction.normalized * Power;
        Vector2 gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        Vector2 predictedPosition = bulletPoint.position;

        for (int i = 0; i < numOfPoints; i++)
        {
            predictedPosition += bulletVelocity * Time.fixedDeltaTime;
            bulletVelocity += gravity * Time.fixedDeltaTime;

            if (Vector2.Distance(predictedPosition, playerPosition) < 0.5f)
            {
                return true;
            }
        }
        return false;
    }

    public void ChangePower()
    {
        Power = PowerSlider.value;
        UpdatePowerValueText();
    }

    private void UpdatePowerValueText()
    {
        powerValueText.text = Power.ToString("0");
    }
}
