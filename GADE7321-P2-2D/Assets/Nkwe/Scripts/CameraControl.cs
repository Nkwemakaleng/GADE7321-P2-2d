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
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component is missing.");
            return;
        }

        turnManager = FindObjectOfType<TurnBasedManager>();
        if (turnManager == null)
        {
            Debug.LogError("TurnBasedManager not found in the scene.");
            return;
        }

        players = turnManager.players;
        if (players == null || players.Length == 0)
        {
            Debug.LogError("Players array is not set or empty.");
            return;
        }

        currentPlayerIndex = turnManager.GetCurrentPlayerIndex();
        TurnBasedManager.OnTurnChanged += UpdateTargetPlayer;

        if (players.Length > 0 && players[currentPlayerIndex] != null)
        {
            UpdateTargetPlayer(currentPlayerIndex, players[currentPlayerIndex]);
        }
        else
        {
            Debug.LogError("Initial target player is null.");
        }
    }

    private void Update()
    {
        HandleZoom();
        FollowPlayer();

        // Switch player on specific conditions
        if (Input.GetKeyDown(KeyCode.Q) || turnManager.GetRemainingTime() <= 0)
        {
            SwitchToNextPlayer();
        }
    }

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

    private void FollowPlayer()
    {
        if (currentTargetPlayer == null)
        {
            Debug.LogWarning("No valid player to follow.");
            return;
        }

        Vector3 targetPosition = new Vector3(currentTargetPlayer.transform.position.x, currentTargetPlayer.transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void SetCameraPosition()
    {
        if (currentTargetPlayer == null)
        {
            Debug.LogWarning("No valid player to set camera position.");
            return;
        }
        transform.position = new Vector3(currentTargetPlayer.transform.position.x, currentTargetPlayer.transform.position.y, transform.position.z);
    }

    private void SwitchToNextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        UpdateTargetPlayer(currentPlayerIndex, players[currentPlayerIndex]);
    }

    private void UpdateTargetPlayer(int targetPlayerIndex, GameObject newTargetPlayer)
    {
        currentPlayerIndex = targetPlayerIndex;
        currentTargetPlayer = newTargetPlayer;
        SetCameraPosition();
    }

    private void OnDestroy()
    {
        TurnBasedManager.OnTurnChanged -= UpdateTargetPlayer;
    }
}