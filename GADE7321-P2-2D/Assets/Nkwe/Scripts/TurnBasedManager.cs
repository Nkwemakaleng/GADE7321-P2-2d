using System;
using TMPro;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    public GameObject[] players; // Array of player GameObjects
    [SerializeField] private float turnDuration = 10f; // Duration of each turn in seconds
    public int currentPlayerIndex = 0; // Index of the current player
    private float remainingTurnTime; // Remaining time for the current turn
    public static event Action<GameObject> OnTurnChanged; // Event for turn change

    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text turnDurationText;

    // Start is called before the first frame update
    void Start()
    {
        remainingTurnTime = turnDuration;

        // Ensure all player controls are disabled first
        foreach (var player in players)
        {
            SetPlayerControl(player, false);
        }

        // Enable controls for the current player
        if (players[currentPlayerIndex] != null)
        {
            SetPlayerControl(players[currentPlayerIndex], true);
            Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn");
            turnText.text = "Player: " + players[currentPlayerIndex].name;

            OnTurnChanged?.Invoke(players[currentPlayerIndex]);
        }
        else
        {
            Debug.LogError("Player at index " + currentPlayerIndex + " is null.");
        }
    }

    // Update is called once per frame
    void Update()
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
            OnTurnChanged?.Invoke(players[currentPlayerIndex]);
           // OnTurnChanged?.Invoke(currentPlayerIndex, players[currentPlayerIndex]);
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

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;

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
    
    // Public method to get the current player index
    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }
}
