using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaypointManager : MonoBehaviour
{
    // Variables shown in the inspector
    
    [Header("Waypoint Manager Settings")]
    [Tooltip("Camera reference to do UI transformation with")] 
    public Camera Camera;
    
    [Tooltip("Control the gap between color bands")] 
    public float WaypointColorStep = 30f;
    
    [Header("UI Settings")]
    
    [Tooltip("Waypoint UI element opacity")]
    public float WaypointOpacity = .8f;
    
    [Tooltip("Waypoint UI element scale")]
    public float WaypointScale = .6f;
    
    [Tooltip("UI Prefab to Instantiate (Waypoint)")]
    public GameObject WaypointUIPrefab;
    
    [Header("Events")]
    [Space(5)]
    
    public UnityEvent<WaypointData> OnWaypointAdded = new UnityEvent<WaypointData>();
    public UnityEvent<WaypointData> OnWaypointRemoved = new UnityEvent<WaypointData>();
    
    // Fields
    
    public bool IsHidden { get; private set; }
    public static WaypointManager Instance { get; private set; }
    public List<WaypointData> Waypoints { get; private set; } = new List<WaypointData>();

    // Private variables
    
    private float _colorIndex;
    private CanvasGroup _canvasGroup;
    
    // Remove Waypoint by data structure
    public bool RemoveWaypoint(WaypointData data)
    {
        return RemoveWaypointsOccurrences(x => x.Equals(data));
    }
    
    // Remove Waypoint by ID
    public bool RemoveWaypoint(string id)
    {
        return RemoveWaypointsOccurrences(x => x.ID == id);
    }
    
    // Add a Waypoint
    public WaypointData AddWaypoint(Vector3 position, string id = "")
    {
        // Instantiate UI Waypoint
        GameObject waypointGameobject = Instantiate(WaypointUIPrefab, _canvasGroup.transform);
        Waypoint waypoint = waypointGameobject.GetComponent<Waypoint>();
        
        // Create WaypointData
        WaypointData waypointData = new WaypointData();
        
        waypointData.Position = position;
        waypointData.ID = id;
        waypointData.UIWaypoint = waypoint;
        
        // Add waypoint to the List
        Waypoints.Add(waypointData);
        
        // Assign data to the Waypoint object
        waypoint.WaypointData = waypointData;
        waypoint.WaypointColor = ScrollInColors();
        
        OnWaypointAdded?.Invoke(waypointData);

        return waypointData;
    }
    
    // Hide the canvas
    public void HideCanvas()
    {
        IsHidden = true;
        _canvasGroup.alpha = 0f;
    }
    
    // Show the canvas
    public void ShowCanvas()
    {
        IsHidden = false;
        _canvasGroup.alpha = 1f;
    }
    
    
    // Generate colors for waypoints
    public Color ScrollInColors()
    {
        float hue = (_colorIndex % 360) / 360f;
        float saturation = 1f;
        float value = 1f;

        Color selectedColor = Color.HSVToRGB(hue, saturation, value);
        _colorIndex += WaypointColorStep;
        return selectedColor;
    }
    
    private void Awake()
    {
        // Singleton behavior
        if (Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
        
        // Prevent as most as it can to not have a null Camera reference
        if (Camera == null)
        {
            Debug.LogWarning("Camera was not assigned to the WaypointManager. Assigning it to the first camera in the scene");
            Camera = FindFirstObjectByType<Camera>();
        }
        
        _canvasGroup = GetComponent<CanvasGroup>();
        _colorIndex = 1f;
    }

    // Remove all the waypoint occurrences found via Predicate
    private bool RemoveWaypointsOccurrences(System.Predicate<WaypointData> predicate)
    {
        
        List<WaypointData> list = Waypoints.FindAll(predicate);
        if (list.Count == 0) return false;
        
        // Destroy associated UIWaypoint objects
        foreach (WaypointData wp in list)
        {
            OnWaypointRemoved?.Invoke(wp);
            Destroy(wp.UIWaypoint.gameObject);
        }
        
        // Remove all occurrences of the specified waypoint
        Waypoints.RemoveAll(predicate);
        return true;
    }
}
