using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Laser : MonoBehaviour
{
    public float laserLength = 10f; // Length of the laser beam
    public float laserWidth = 0.1f; // Width of the laser beam
    public float laserDamage = 10f; // Damage inflicted by the laser beam per second
    public LayerMask damageableLayers; // Layers that can be damaged by the laser

    private LineRenderer lineRenderer;
    private Vector2 laserDirection;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;

        // Set laser direction based on the object's rotation
        laserDirection = transform.right;

        // Start the laser beam
        StartCoroutine(FireLaser());
    }

    IEnumerator FireLaser()
    {
        while (true)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, laserDirection, laserLength, damageableLayers);

            // Set the position of the line renderer
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);

            if (hits.Length > 0)
            {
                // If the laser hits something, set the end position to the hit point
                lineRenderer.SetPosition(1, hits[0].point);

                // Damage the hit object
                DamageableObject damageableObject = hits[0].collider.GetComponent<DamageableObject>();
                if (damageableObject != null)
                {
                    damageableObject.TakeDamage(laserDamage * Time.deltaTime);
                }
            }
            else
            {
                // If the laser doesn't hit anything, extend it to its maximum length
                Vector2 endPosition = transform.position + laserDirection * laserLength;
                lineRenderer.SetPosition(1, endPosition);
            }

            // Wait for the next frame
            yield return null;
        }
    }
}
