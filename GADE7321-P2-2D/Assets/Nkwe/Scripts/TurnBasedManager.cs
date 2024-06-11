using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class TurnBasedManager : MonoBehaviour
{
    public GameObject[] players; // Assign AiManager GameObjects in the Inspector
    [SerializeField] private float turnDuration = 10f; // Duration of each turn in seconds
    public int currentPlayerIndex = 0; // Index of the current player
    private float remainingTurnTime; // Remaining time for the current turn
    public static event Action<int, GameObject> OnTurnChanged; // Event for turn change

    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text turnDurationText;

    public static TurnBasedManager Instance { get; private set; }
    private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    private void Start()
    {
        
            remainingTurnTime = turnDuration;

        // Ensure all player controls are disabled first
        foreach (var player in players)
        {
            SetPlayerControl(player, false);
        }
        currentPlayerIndex = Random.Range(0, players.Length);
        // Enable controls for the current player
        if (players[currentPlayerIndex] != null)
        {
            StartTurn(); // Starts the first player's turn
        }
        else
        {
            Debug.LogError("Player at index " + currentPlayerIndex + " is null.");
        }
    }

    private void Update()
    {
        // Update the remaining turn time
        remainingTurnTime -= Time.deltaTime;
        turnDurationText.text = "Time: " + Mathf.Max(remainingTurnTime, 0).ToString("F2"); // Update UI

        if (remainingTurnTime <= 0 || Input.GetKeyDown(KeyCode.Q))
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

            // Update UI
            turnText.text = "Player: " + players[currentPlayerIndex].name;

            // Notify subscribers about the turn change
            OnTurnChanged?.Invoke(currentPlayerIndex, players[currentPlayerIndex]);
        }
        else
        {
            Debug.LogError("Player at index " + currentPlayerIndex + " is null.");
        }
    }

    // End the turn for the current player
    public void EndTurn()
    {
        if (players[currentPlayerIndex] != null)
        {
            SetPlayerControl(players[currentPlayerIndex], false);
        }

        // Move to the next player's turn
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        StartTurn();
        NotifyTurnChanged();
    }

    // Method to enable or disable player controls
    private void SetPlayerControl(GameObject player, bool isEnabled)
    {
        if (player == null)
        {
            Debug.LogError("Player object is null.");
            return;
        }
        // Try to find both Aim_Mechanics and AIAimMechanics components
        var playerComponent = player.GetComponent<Player1>();
        var aimMechanicsComponent = player.GetComponentInChildren<Aim_Mechanics>();
        var aiManager = player.GetComponent<AiManager>();
        var aiAimMechanicsComponent = player.GetComponentInChildren<AIAimMechanics>();

        if (player.name == "Player1")
        {
            if (playerComponent != null)
            {
                playerComponent.enabled = isEnabled;
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
        else
        {
                    // Enable/disable Aim_Mechanics if found
                    if (aiManager != null)
                    {
                        aiManager.enabled = isEnabled;
                        
                    }
            
                    // Enable/disable AIAimMechanics if found
                    if (aiAimMechanicsComponent != null)
                    {
                        aiAimMechanicsComponent.enabled = isEnabled;
                    }
            
                    // Log errors if both components are missing
                    if (aimMechanicsComponent == null && aiAimMechanicsComponent == null)
                    {
                        Debug.LogError("Aim_Mechanics or AIAimMechanics component is missing on player " + player.name);
                    }
        }
         /*else
        {
            Debug.Log("Player 2 Not found");
        }*/

    }

    // Public method to get the current player index
    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }

    public float GetRemainingTime()
    {
        return remainingTurnTime;
    }

    public void OnEndTurnButtonClicked()
    {
        EndTurn();
    }

    private void NotifyTurnChanged()
    {
        OnTurnChanged?.Invoke(currentPlayerIndex, players[currentPlayerIndex]);
    }
}
