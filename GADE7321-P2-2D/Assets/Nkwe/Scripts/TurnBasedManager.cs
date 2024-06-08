using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    [SerializeField] private GameObject[] players; // Array of player GameObjects
    [SerializeField] private float turnDuration = 10f; // Duration of each turn in seconds
    private int currentPlayerIndex = 0; // Index of the current player
    private float remainingTurnTime; // Remaining time for the current turn
    public static event Action<GameObject> OnTurnChanged; // Event for turn change

    // Start is called before the first frame update
    void Start()
    {
        if (players == null || players.Length == 0)
        {
            Debug.LogError("Players array is not set or empty.");
            return;
        }

        // Initialize turn by starting with the first player
        StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the remaining turn time
        remainingTurnTime -= Time.deltaTime;
        Debug.Log("Time " + remainingTurnTime);
        if (remainingTurnTime <= 0)
        {
            EndTurn();
        }

        // Check for end of turn condition (e.g., player action)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
    }

    // Start the turn for the current player
    private void StartTurn()
    {
        // Reset the turn timer
        remainingTurnTime = turnDuration;

        // Ensure the player exists before enabling controls
        if (players[currentPlayerIndex] != null)
        {
            SetPlayerControl(players[currentPlayerIndex], true);
            Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn");
            
            // Notify subscribers about the turn change
            OnTurnChanged?.Invoke(players[currentPlayerIndex]);
        }
        else
        {
            Debug.LogError("Player at index " + currentPlayerIndex + " is null.");
        }
    }

    // End the turn for the current player
   public void EndTurn()
    {
        
        // Ensure the player exists before disabling controls
        if (players[currentPlayerIndex] != null)
        {
            SetPlayerControl(players[currentPlayerIndex], false);
        }
        else
        {
            Debug.LogError("Player at index " + currentPlayerIndex + " is null.");
        }

        // Move to the next player
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;

        // Start the next player's turn
        StartTurn();
    }

    // Method to enable or disable player controls
    private void SetPlayerControl(GameObject player, bool isEnabled)
    {
        if (player == null)
        {
            Debug.LogError("Player object is null.");
            return;
        }

        var player1Component = player.GetComponent<Player1>();
        var aimMechanicsComponent = player.GetComponentInChildren<Aim_Mechanics>();
       
        if (player1Component != null)
        {
            player1Component.enabled = isEnabled;
        }
        else
        {
            Debug.LogError("Player1 component is missing on player " + player.name);
        }

        if (aimMechanicsComponent != null)
        {
            aimMechanicsComponent.enabled = isEnabled;
        }
        else
        {
            Debug.LogError("Aim_Mechanics component is missing on player " + player.name);
        }
    }
}
