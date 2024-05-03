using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public GameObject[] players;
    public Slider zoomSlider;

    public float zoomSpeed = 1.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 20.0f;
    public float followSpeed = 5.0f;

    private Camera cam;
    private int currentPlayerIndex = 0;

    private void Start()
    {
        cam = GetComponent<Camera>();
        SetCameraPosition();
    }

    private void Update()
    {
        // Zoom In/Out
        float zoomValue = zoomSlider.value;
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        zoomValue -= zoomDelta;
        zoomValue = Mathf.Clamp01(zoomValue);
        zoomSlider.value = zoomValue;

        // Update camera zoom
        float zoom = Mathf.Lerp(minZoom, maxZoom, zoomValue);
        cam.orthographicSize = zoom;

        // Follow player smoothly
        if (players.Length > 0)
        {
            Vector3 targetPosition = new Vector3(players[currentPlayerIndex].transform.position.x, players[currentPlayerIndex].transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        // Switch player on specific conditions (e.g., player's turn)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextPlayer();
        }
    }

    private void SetCameraPosition()
    {
        if (players.Length == 0)
        {
            Debug.LogWarning("No players assigned to the camera controller.");
            return;
        }

        transform.position = new Vector3(players[currentPlayerIndex].transform.position.x, players[currentPlayerIndex].transform.position.y, transform.position.z);
    }

    private void NextPlayer()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Length)
        {
            currentPlayerIndex = 0; // Wrap around to the first player
        }
        SetCameraPosition();
    }
}
