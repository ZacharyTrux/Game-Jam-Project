using UnityEngine;
using UnityEngine.InputSystem;

public class Waypoints : MonoBehaviour
{
    // Set in inspector
    public GameObject locationWaypoint;
    public GameObject enemyWaypoint;
    public LayerMask enemyLayer;

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Replace this with waypoint keybind
        {
            ClickDetector();
        }
        
    }

    void ClickDetector()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, enemyLayer);
        if (hit.collider != null)
        {
            GameObject waypoint = Instantiate(enemyWaypoint, hit.point, Quaternion.identity);
            Destroy(waypoint, 5f);
        } 
        else
        {
            GameObject waypoint = Instantiate(locationWaypoint, hit.point, Quaternion.identity);
            Destroy(waypoint, 5f);
        }
    }
}
