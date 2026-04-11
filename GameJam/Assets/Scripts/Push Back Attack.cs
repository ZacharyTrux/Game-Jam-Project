using UnityEngine;

public class PushBackAttack : MonoBehaviour
{
    public float pushBackForce = 10f;
    public float speed = 5f;
    public float lifetime = 0.5f;

    private Vector3 moveDirection;

    void Start(){
        moveDirection = Player.Instance.gameObject.transform.right;
        Destroy(gameObject, lifetime); 
    }

    // Update is called once per frame
    void Update(){
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
}
