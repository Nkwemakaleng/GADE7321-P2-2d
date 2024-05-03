using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
   [SerializeField] public GameObject player;
  [SerializeField]  public GameObject player2;
   [SerializeField] public Slider zoomSlider;

    public float zoomSpeed = 1.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 20.0f;

    [SerializeField] private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        // Zoom In/Out
        float zoomValue = zoomSlider.value;
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        zoomValue -= zoomDelta;
        zoomValue = Mathf.Clamp(zoomValue, 0.0f, 1.0f);
        zoomSlider.value = zoomValue;

        // Update camera zoom
        float zoom = Mathf.Lerp(minZoom, maxZoom, zoomValue);
        cam.orthographicSize = zoom;

        // Follow player
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        }
    }
}

