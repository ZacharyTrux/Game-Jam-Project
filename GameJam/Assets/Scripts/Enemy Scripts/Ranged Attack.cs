using System.Xml;
using UnityEngine;

public class RangedAttack : MonoBehaviour {
    public float speed = 0.5f;
    public string ownerTag;
    public float damage = 20f;
    void Start(){
        Destroy(gameObject, 5f); 
    }

    void Update(){
        
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(ownerTag)) return;

        bool shouldHit = false;
        if (ownerTag == "Ally" || ownerTag == "Player") {
            if (collision.CompareTag("Enemy")) shouldHit = true;
        } 
        else if (ownerTag == "Enemy") {
            if (collision.CompareTag("Ally") || collision.CompareTag("Player")) shouldHit = true;
        }

        if (shouldHit) {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); 
        }
    }
} 