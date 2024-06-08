using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject[] players; // Array of player GameObjects
    [SerializeField] private Slider zoomSlider; // UI Slider for zoom control
    [SerializeField] private float zoomSpeed = 1.0f; // Speed of zoom
    [SerializeField] private float minZoom = 5.0f; // Minimum zoom level
    [SerializeField] private float maxZoom = 20.0f; // Maximum zoom level
    [SerializeField] private float followSpeed = 5.0f; // Speed of following the player

    private Camera cam; // Reference to the camera component
    private int currentPlayerIndex = 0; // Index of the current player
    private GameObject currentTargetPlayer; // Current target player to follow
    private TurnBasedManager turnManager;


    private void Start()
    {
        //turnBasedManager.currentPlayerIndex
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component is missing.");
            return;
        }
        if (players == null || players.Length == 0)
        {
            Debug.LogError("Players array is not set or empty.");
            return;
        }
        TurnBasedManager.OnTurnChanged += UpdateTargetPlayer;
        // Get initial player list from TurnBasedManager
        TurnBasedManager turnManager = FindObjectOfType<TurnBasedManager>();
        if (turnManager != null)
        {
            players = turnManager.players;
            currentPlayerIndex = turnManager.GetCurrentPlayerIndex();
        }

        UpdateTargetPlayer(players[currentPlayerIndex]);
    }

    private void Update()
    {
        HandleZoom();
        FollowPlayer();

        // Switch player on specific conditions
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchToNextPlayer();
        }
    }

    // Handles camera zoom based on the slider value and mouse scroll wheel input
    private void HandleZoom()
    {
        if (zoomSlider == null)
        {
            Debug.LogError("Zoom slider is not set.");
            return;
        }

        float zoomValue = zoomSlider.value;
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        zoomValue -= zoomDelta;
        zoomValue = Mathf.Clamp01(zoomValue);
        zoomSlider.value = zoomValue;

        float zoom = Mathf.Lerp(minZoom, maxZoom, zoomValue);
        cam.orthographicSize = zoom;
    }

    // Smoothly follows the current player
    private void FollowPlayer()
    {
        if (players == null || players.Length == 0 || players[currentPlayerIndex] == null)
        {
            Debug.LogWarning("No valid player to follow.");
            return;
        }

        Vector3 targetPosition = new Vector3(players[currentPlayerIndex].transform.position.x, players[currentPlayerIndex].transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    // Sets the camera position to the current player
    private void SetCameraPosition()
    {
        if (players == null || players.Length == 0 || players[currentPlayerIndex] == null)
        {
            Debug.LogWarning("No valid player to set camera position.");
            return;
        }
        transform.position = new Vector3(players[currentPlayerIndex].transform.position.x, players[currentPlayerIndex].transform.position.y, transform.position.z);
    }

    // Switches to the next player in the list
    private void SwitchToNextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        SetCameraPosition();
    }
    private void UpdateTargetPlayer(GameObject newTargetPlayer)
    {
        currentTargetPlayer = newTargetPlayer;
        SetCameraPosition();
    }
    private void OnDestroy()
    {
        TurnBasedManager.OnTurnChanged -= UpdateTargetPlayer; // Unsubscribe from the turn change event
    }
    private void OnTurnChanged(int playerIndex, GameObject newPlayer)
    {
        currentPlayerIndex = playerIndex;
        SetCameraPosition();
    }
    
    

}
