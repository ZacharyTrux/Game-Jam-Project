using UnityEngine;
using UnityEngine.InputSystem;

public class Waypoints : MonoBehaviour{
    // Set in inspector
    public GameObject locationWaypoint;
    public GameObject enemyWaypoint;
    public LayerMask enemyLayer;
    public float waypointDuration = 5f;

    public static GameObject currentWaypoint { get; private set; }

    // Update is called once per frame
    void Update(){
        if(Mouse.current.rightButton.wasPressedThisFrame) // Replace this with waypoint keybind
        {
            ClickDetector();
        }
    }

    void ClickDetector(){
        if(currentWaypoint != null){ // ensure only one waypoint at a time
            Destroy(currentWaypoint);
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, enemyLayer);

        if (hit.collider != null){
            GameObject waypoint = Instantiate(enemyWaypoint, hit.point, Quaternion.identity);
            waypoint.transform.SetParent(hit.collider.transform);
            currentWaypoint = waypoint;
        } 
        else{
            Vector3 screenPos = Mouse.current.position.ReadValue();
            screenPos.z = 10f;
            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(screenPos);
            spawnPos.z = 0;
            GameObject waypoint = Instantiate(locationWaypoint, spawnPos, Quaternion.identity);
            currentWaypoint = waypoint;
        }

        Destroy(currentWaypoint, waypointDuration); // destroy waypoint after given duration
    }
}
