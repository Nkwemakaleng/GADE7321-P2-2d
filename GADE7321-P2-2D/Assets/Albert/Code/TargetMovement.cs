using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public delegate void OnMovementCycleComplete();
    public static event OnMovementCycleComplete MovementCycleComplete;

    private AiManager aiManager;
    private Vector3 initialPosition;
    private bool movingUp = false;

    private void Start()
    {
        aiManager = FindObjectOfType<AiManager>();
        initialPosition = transform.localPosition; // Relative to the player
    }

    private void Update()
    {
        if (aiManager.aiTurn && !movingUp)
        {
            StartCoroutine(MoveUpwards());
        }
    }

    private IEnumerator MoveUpwards()
    {
        movingUp = true;
        float elapsedTime = 0f;
        while (elapsedTime < 3f)
        {
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initialPosition; // Reset position after moving up
        movingUp = false;

        // Notify listeners that the movement cycle is complete
        MovementCycleComplete?.Invoke();
    }
}
