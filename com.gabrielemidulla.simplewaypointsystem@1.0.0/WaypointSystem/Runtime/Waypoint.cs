using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour
{
    // Variables shown in the inspector
    
    public WaypointData WaypointData;
    public Color WaypointColor;

    // Private variables
    
    private RectTransform _rectTransform;
    private RawImage _rawImage;
    private TMP_Text _tmpText;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rawImage = GetComponent<RawImage>();
        _tmpText = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (WaypointManager.Instance.Camera == null) return;
        if (WaypointManager.Instance.IsHidden) return;
        
        // Set waypoint color
        WaypointColor.a = WaypointManager.Instance.WaypointOpacity;
        _rawImage.color = WaypointColor;
        
        // Set waypoint scale
        float scale = WaypointManager.Instance.WaypointScale;
        _rectTransform.localScale = new Vector3(scale, scale, scale);

        Transform cameraTransform = WaypointManager.Instance.Camera.transform;
        
        // Set distance text
        int distance = (int)Vector3.Distance(WaypointData.Position, cameraTransform.transform.position);
        _tmpText.text = distance + "m";
        
        // Check if the waypoint is behind camera
        float dot = Vector3.Dot( (WaypointData.Position - cameraTransform.position).normalized, cameraTransform.forward );
        if (dot <= 0) return; // prevent from acting weird
        
        // Convert world position to viewport position
        Vector3 viewportPosition = WaypointManager.Instance.Camera.WorldToViewportPoint(WaypointData.Position);

        // Clamp the viewport position to the screen size
        viewportPosition.x = Mathf.Clamp01(viewportPosition.x);
        viewportPosition.y = Mathf.Clamp01(viewportPosition.y);

        // Set anchored position using the clamped viewport position
        _rectTransform.anchorMin = viewportPosition;
        _rectTransform.anchorMax = viewportPosition;
    }
}