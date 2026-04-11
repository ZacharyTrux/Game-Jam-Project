using UnityEngine.UI;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;



public class Player : MonoBehaviour {
    // public, inspection, variables
    public float iFrames = 3f;
    public float speed = 5f;
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    public GameObject expoPrefab;
    public Transform bulletSpawnPoint;
    public Shield shield;
    public Slider sliderHealth;
    public UI ui;
    public AudioClip shootingSound;
    public AudioClip missileFireSound;
    public AudioClip damage;
    public AudioClip explosionAudio;


    // private variables
    private Inputs input;    
    private const float Y_LIMIT = 4.6f;
    private const float X_LIMIT = 8.2f;
    private float health;
    private bool isInvincible = false;
    private int homingShotsRemaining = 0;
    private SpriteRenderer sprite;
    private AudioSource audioSrc;

    
    // Grab and set components needed
    void Start(){
        health = 1.0f; // initiate health to the max health
        audioSrc = GetComponent<AudioSource>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update(){
        sliderHealth.value = health; // update health every frame

        // Player Shooting bullets/missile
        if (Inputs.Instance.input.Shoot.WasPressedThisFrame()){
            if (homingShotsRemaining > 0) {
                Instantiate(missilePrefab, bulletSpawnPoint.position, UnityEngine.Quaternion.identity);
                homingShotsRemaining--;
            } else {
                Instantiate(bulletPrefab, bulletSpawnPoint.position, UnityEngine.Quaternion.identity);
            }
            audioSrc.clip = shootingSound;
            audioSrc.Play();
        }

        // Player Movement
        var vertMove = Inputs.Instance.input.MoveVertically.ReadValue<float>(); // move vertically based off input
        var horizontalMove = Inputs.Instance.input.MoveHortizontally.ReadValue<float>(); // move horizontally based off input
        this.transform.Translate(UnityEngine.Vector3.up * speed * Time.deltaTime * vertMove);
        this.transform.Translate(UnityEngine.Vector3.right * speed * Time.deltaTime * horizontalMove); // performs the updated movements
        CheckBounds(); // ensure player does not go off screen
    }

    private void CheckBounds(){ // boundary checks
        if (this.transform.position.y > Y_LIMIT)
        {
            this.transform.position = new UnityEngine.Vector3(transform.position.x, Y_LIMIT);
        }
        else if (this.transform.position.y < -Y_LIMIT)
        {
            this.transform.position = new UnityEngine.Vector3(transform.position.x, -Y_LIMIT);
        }
        else if (this.transform.position.x > X_LIMIT)
        {
            this.transform.position = new UnityEngine.Vector3(X_LIMIT, transform.position.y);
        }
        else if (this.transform.position.x < -X_LIMIT)
        {
            this.transform.position = new UnityEngine.Vector3(-X_LIMIT, transform.position.y);
        }
    }

    // update health and check for game over
    public void DamageFromEnemy(){
        if(isInvincible) return;
        health -= 0.25f;
        StartCoroutine(IFramesEnabled()); // enable Iframes on player

        if(health <= 0){
            sliderHealth.value = health;
            GameOver();
            ui.ShowGameOver();
        }
        audioSrc.clip = damage;
        audioSrc.Play();
    }


    // NOTE:
    // IEnumerator allows for pausing execution of controls and waiting till a given action or amount of time has passed to continue further
    // Player can continue movement and other actions while waiting for invincibility to turn off
    private System.Collections.IEnumerator IFramesEnabled(){ // go through IFrames
        isInvincible = true; // ensure player can not be hurt
        for(int i = 0; i < 4; i++){
            sprite.color = new Color(1,1,1, 0.2f); // lower alpha of player
            yield return new WaitForSeconds(iFrames / 8); 
            sprite.color = new Color(1,1,1, 1); // increase again to mimic flashing
            yield return new WaitForSeconds(iFrames / 8);
        }

        isInvincible = false; // player is damagable again
    }

    private void GameOver(){
        Explosion();
        Destroy(gameObject); // destroy the player
    }

    public bool IsInvincible(){
        return isInvincible;
    }

    private void Explosion(){
        AudioSource.PlayClipAtPoint(explosionAudio, transform.position);
        var expoObj = Instantiate(expoPrefab, transform.position, UnityEngine.Quaternion.identity); // creates explosion of enemy object
        Destroy(expoObj, expoObj.GetComponent<ParticleSystem>().main.duration); // delete explosion after it goes off
    }

}