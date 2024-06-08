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

    private void Start()
    {
        points = new GameObject[numOfPoints];
        for (int i = 0; i < numOfPoints; i++)
        {
            points[i] = Instantiate(Point, bulletPoint.position, Quaternion.identity);
        }
        PowerSlider.value = Power;
        UpdatePowerValueText();
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
        GameObject newBullet = Instantiate(Bullet1, bulletPoint.position, Quaternion.identity);
        Rigidbody2D bulletRB = newBullet.GetComponent<Rigidbody2D>();
        if (bulletRB != null)
        {
            bulletRB.velocity = direction.normalized * Power;
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
      //  UpdateShieldValueText();
    }

    // Method to update the power value in the text box
    void UpdatePowerValueText()
    {
        powerValueText.text =  Power.ToString("0");
    }

   // void UpdateShieldValueText()
   // {
    //    ShieldValueText.text = Power.ToString("0") ;
   // }



}