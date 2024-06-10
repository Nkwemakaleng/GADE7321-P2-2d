using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class Aim_Mechanics : MonoBehaviour
{
    public GameObject Bullet1;
    public float Power = 10;
    public Slider PowerSlider;
    public Transform bulletPoint;

    public GameObject Point;
    GameObject[] points;
    public int numOfPoints = 10;
    public float spacing = 0.5f;
    public TextMeshProUGUI powerValueText;
    Vector3 direction;

    private BulletManager bulletManager;
    private void Start()
    {
        points = new GameObject[numOfPoints];
        for (int i = 0; i < numOfPoints; i++)
        {
            points[i] = Instantiate(Point, bulletPoint.position, Quaternion.identity);
        }
        bulletManager = FindObjectOfType<BulletManager>();
        PowerSlider.value = Power;
        UpdatePowerValueText();
        bulletManager.updateBulletText();
        TurnBasedManager.OnTurnChanged += OnTurnChanged;
    }

    void Update()
    {

        // Cannon rotation
        Vector3 cannonPosition = transform.position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)); // Set z to 10 (or any distance from the camera)
        direction = mousePosition - cannonPosition; // Update direction
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.right = direction;

        // Shoot
        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
        }

        for (int i = 0; i < numOfPoints; i++)
        {
            points[i].transform.position = PointPosition(i * spacing);
        }
    }


    public void Shoot()
    {
        
        GameObject currentBullet = bulletManager.GetCurrentBullet();
        bulletManager.updateBulletText();
        if (currentBullet != null)
        {
            if (bulletManager.GetCurrentAmount() >= bulletManager.bulletTypes[bulletManager.GetCurrentBulletIndex()].maxBullets)
            {
                GameObject newBullet = Instantiate(currentBullet, bulletPoint.position, Quaternion.identity);
                Rigidbody2D bulletRB = newBullet.GetComponent<Rigidbody2D>();
                if (bulletRB != null)
                {
                    bulletRB.velocity = direction.normalized * Power;
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

    Vector3 PointPosition(float t)
    {
        Vector3 gravity = new Vector3(Physics2D.gravity.x, Physics2D.gravity.y, 0f);
        Vector3 position = bulletPoint.position + direction.normalized * Power * t + 0.5f * gravity * (t * t);
        return position;
    }
// Method to change power with the slider
    public void ChangePower()
    {
        Power = PowerSlider.value;
        UpdatePowerValueText();
        Debug.Log("Power changed to: " + Power + " for player: " + gameObject.name);
    }

    // Method to update the power value in the text box
    void UpdatePowerValueText()
    {
        powerValueText.text = "Power: " + Power.ToString("0");
    }
    private void OnDestroy()
    {
        TurnBasedManager.OnTurnChanged -= OnTurnChanged;
    }

    private void OnTurnChanged(int players, GameObject newPlayer)
    {
        if (newPlayer == gameObject)
        {
            PowerSlider.value = Power; // Sync slider with power value
            UpdatePowerValueText();
            //PowerSlider.interactable = true; // Enable slider for the active player
            Debug.Log("Turn changed to: " + gameObject.name + " with power: " + Power);
        }
        else
        {
            //PowerSlider.interactable = false; // Disable slider for inactive players
        }
    }
}