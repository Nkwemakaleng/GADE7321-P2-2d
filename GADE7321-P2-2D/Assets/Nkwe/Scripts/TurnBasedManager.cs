using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    [SerializeField] public GameObject[] players; // Array of player GameObjects
    private int currentPlayerIndex = 0; // Index of the current player

    // Start is called before the first frame update
    void Start()
    {
        // Initialize turn by starting with the first player
        StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for end of turn condition (e.g., player action)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
    }

    // Start the turn for the current player
    void StartTurn()
    {
        // Activate player controls
        players[currentPlayerIndex].GetComponent<PlayerController>().EnableControls();

        Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn");
    }

    // End the turn for the current player
    void EndTurn()
    {
        // Deactivate player controls
        players[currentPlayerIndex].GetComponent<PlayerController>().DisableControls();

        // Move to the next player
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;

        // Start the next player's turn
        StartTurn();
    }
}
