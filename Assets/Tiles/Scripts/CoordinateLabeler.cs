using System;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class CoordinateLabeler : MonoBehaviour
{
    // Serialized fields
    [SerializeField]
    private Color defaultColor = Color.white;

    [SerializeField]
    private Color blockedColor = Color.gray;

    // References
    private TextMeshPro label;

    // private Waypoint waypoint;

    private void Awake()
    {
        label = GetComponent<TextMeshPro>();
        // waypoint = GetComponentInParent<Waypoint>();

        // Immediately set the color based on the current placeability of the waypoint
        ColorCoordinates();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            DisplayCoordinates();
            UpdateObjectName();
        }
        else
        {
            // ColorCoordinates is not required in every Update if you want instant updates only on placeability changes.
        }
    }

    private void DisplayCoordinates()
    {
        if (transform.parent == null)
        {
            Debug.LogError("Parent transform is missing.");
            return;
        }

        var coordinates = CalculateCoordinates();
        label.text = $"{coordinates.x},{coordinates.y}";
    }

    private Vector2Int CalculateCoordinates()
    {
        Vector2Int coordinates;
        var parentPosition = transform.parent.position;

#if UNITY_EDITOR
        var snapSettings = UnityEditor.EditorSnapSettings.move;
        coordinates = new Vector2Int(
            Mathf.RoundToInt(parentPosition.x / snapSettings.x),
            Mathf.RoundToInt(parentPosition.z / snapSettings.z)
        );
#else
        coordinates = new Vector2Int(
            Mathf.RoundToInt(parentPosition.x),
            Mathf.RoundToInt(parentPosition.z)
        );
#endif
        return coordinates;
    }

    public void ColorCoordinates()
    {
        // Immediately update the color when this method is called
        // label.color = waypoint.IsPlaceable ? defaultColor : blockedColor;
    }

    private void UpdateObjectName()
    {
        var coordinates = CalculateCoordinates();
        transform.parent.name = coordinates.ToString();
    }

    // Method to call whenever the placeability of a waypoint changes
    public void OnPlaceabilityChanged()
    {
        // Update the color immediately when placeability changes
        ColorCoordinates();
    }
}
