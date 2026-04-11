using UnityEngine;
using UnityEngine.InputSystem;

public class Waypoints : MonoBehaviour
{
    // Waypoint Sprites
    public GameObject locationWaypoint;
    public GameObject enemyWaypoint;

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = 10f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            SpawnLocationWaypoint(worldPos);
        }
        
    }

    public void SpawnLocationWaypoint(Vector3 spawnPoint)
    {
        spawnPoint.z = 0;
        GameObject newPoint = Instantiate(locationWaypoint, spawnPoint, Quaternion.identity);
        Destroy(newPoint, 5f);
    }

}
