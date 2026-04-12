using UnityEngine;
using UnityEngine.InputSystem;

public class PushBackAttack : MonoBehaviour{
    public float pushBackForce = 10f;
    public float speed = 5f;
    public float lifetime = 0.5f;
    private Vector3 moveDirection;

    void Awake(){
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mousePos.z = 0; 

        moveDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle+180f);


        // Cleanup
        Destroy(gameObject, lifetime); 
    }

    // Update is called once per frame
    void Update(){
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Enemy")){
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null){
                enemy.isStunned = true;
                enemy.stunTimer = 2f;
            }
        }
    }
}
