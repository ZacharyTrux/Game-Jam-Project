using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

public enum EnemyState{
    Targeting,
    Attacking,
    PlayerControlled,
    Dying
}

public enum EnemyType{
    Melee,
    Ranged,
    Brute,
    Elite,
    Boss
}

public abstract class Enemy : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyState State { get; set; }
    public EnemyType Type { get; set; }
    public bool isPossessed;
    public bool canBePossessed;

    public LayerMask groundLayer;
    public float moveSpeed = 3.5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int slotWeight = 1;
    public float health = 100f;

    protected GameObject player;
    protected GameObject target;
    protected float lastAttackTime;
    protected Animator animator;
    protected Vector3 groupOffset;

    protected virtual void Awake(){
       player = GameObject.FindGameObjectWithTag("Player");
       animator = GetComponent<Animator>();
    }

    protected virtual void Start(){
        groupOffset = new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f), 0);
        State = EnemyState.Targeting;
        //Type = EnemyType.Melee;
        isPossessed = false;
        lastAttackTime = 0f;
    }

    protected virtual void Update(){
        switch (State){
            case EnemyState.Targeting:
                DecideTarget();
                break;

            case EnemyState.Attacking:
                HandleAttacking();
                break;

            case EnemyState.PlayerControlled:
                HandlePossession();
                break;

            case EnemyState.Dying:
                HandleDeath();
                break;
        }
    }

    protected virtual void DecideTarget(){
        target = FindNearestTarget();
        if(target != null){
            HandleTargeting(target.transform.position);
        }
    }

    protected virtual void HandleTargeting(Vector3 targetPosition){
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, target.transform.position) <= attackRange){
            State = EnemyState.Attacking;
        }
    }

    protected abstract void MoveTowards(Vector3 position);
    protected abstract void HandleAttacking();

    public virtual void TakeDamage(float damage){
        health -= damage;
        if(health <= 0f && State != EnemyState.Dying){
            State = EnemyState.Dying;
        }
    }

    public virtual void MakePossesed(){
        State = EnemyState.PlayerControlled;
        gameObject.tag = "Ally";
        isPossessed = true;
        target = null;
        GetComponentInChildren<SpriteRenderer>().color = Color.green;
    }

    protected virtual GameObject FindNearestTarget(){
        List<string> targetTags = new();
        if(State == EnemyState.PlayerControlled){
            targetTags.Add("Enemy");
        } 
        else{
            targetTags.Add("Player");
            targetTags.Add("Ally");
        }

        GameObject nearestTarget = null;
        float minDistance = 15f; 
        Vector3 currPos = transform.position;

        foreach(string targetTag in targetTags){ // grab valid targets
            foreach (GameObject target in GameObject.FindGameObjectsWithTag(targetTag)){ // loop through each object on the field with that tag
                float dist = Vector3.Distance(currPos, target.transform.position); // calculate distance to target
                if (dist < minDistance){ // closer target found track new target
                    nearestTarget = target;
                    minDistance = dist;
                }
            }
        }
        return nearestTarget;
    }

    protected virtual void HandleDeath(){
        // Play death animation or effects here
        Destroy(gameObject);
    }

    protected virtual void HandlePossession(){
        var currPoint = Waypoints.currentWaypoint;
        if(currPoint != null){ 
            MoveTowards(currPoint.transform.position);
            target = FindNearestTarget();
            if(target != null){
                State = EnemyState.Attacking;
                return;
            }
        }
        else{
            OrbitPlayer();
        }

        // get mouse position
        // Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        // Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        // mousePos.z = 0; 

        // move towards mouse position
        //transform.position = Vector3.Lerp(transform.position, mousePos, moveSpeed * Time.deltaTime);        
    }

    private void OrbitPlayer(){
        if(player == null) return;

        float angle = Time.time * 2f + (gameObject.GetInstanceID() * 0.5f);

        // 2. Calculate the X and Y offsets using Sine and Cosine
        float x = Mathf.Cos(angle) * 2.5f;
        float y = Mathf.Sin(angle) * 2.5f;

        // 3. Create the target position relative to the player
        Vector3 orbitTarget = player.transform.position + new Vector3(x, y, 0);

        // 4. Move towards that dynamic rotating point
        MoveTowards(orbitTarget);
    }

    protected void RotateTowards(Vector3 position){
        if(target == null) return;

        Vector2 direction = position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected void ResetState(){
        State = isPossessed ? EnemyState.PlayerControlled : EnemyState.Targeting;
    }

    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Attack") && State != EnemyState.Dying){
            GameObject attacker = collision.transform.root.gameObject;
            Debug.Log("Hit by: " + attacker.name);
            if(gameObject.CompareTag("Ally")){
                if (attacker.CompareTag("Enemy")){
                    health -= 20f;
                }
            }
            else if(gameObject.CompareTag("Enemy")){
                if(attacker.CompareTag("Player") || attacker.CompareTag("Ally")){
                    health -= 20f;
                }
            }
            if(health <= 0f){
                State = EnemyState.Dying;
                // Play death animation or effects here
            }
        }
    }

/*
    private void HandleTargeting(GameObject target){
        // Move towards the target and check for attack range
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, target.transform.position) <= attackRange){
            State = EnemyState.Attacking;
        }
    }

    private void HandleAttacking(){
        if(target == null){
            ResetState();
            return;
        }

        float dist = Vector3.Distance(transform.position, target.transform.position);
        if(dist <= attackRange){
            RotateTowardsTarget(target.transform.position);
            if (dist > 0.5f) { 
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, (moveSpeed * 0.5f) * Time.deltaTime);
            }
            if(Time.time >= lastAttackTime + attackCooldown){
                PerformAttack();
                lastAttackTime = Time.time;
            }
        }
        else{
            ResetState();
        }
    }

    

    
    

    

    
        // else if(Type == EnemyType.Ranged){
        //     // Instantiate projectile towards target
        //     Debug.Log("Ranged attack towards " + target.name);
        //     // GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        //     // projectile.GetComponent<Projectile>().Initialize(target.transform);
        // }
        // else if(Type == EnemyType.Brute){
        //     // Stronger melee attack with knockback
        //     if(Vector3.Distance(transform.position, target.transform.position) <= attackRange){
        //         Debug.Log("Brute attack hits " + target.name);
        //         // target.GetComponent<Health>()?.TakeDamage(20f);
        //         // Apply knockback logic here
        //     }
        // }
    }

    public void MoveTowards(Vector3 position){
        transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);
        RotateTowardsTarget(position);
    }

    private void RotateTowardsTarget(Vector3 position){
        if(target == null) return;

        Vector2 direction = position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    

    

    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Attack") && State != EnemyState.Dying){
            GameObject attacker = collision.transform.root.gameObject;
            Debug.Log("Hit by: " + attacker.name);
            if(gameObject.CompareTag("Ally")){
                if (attacker.CompareTag("Enemy")){
                    health -= 20f;
                }
            }
            else if(gameObject.CompareTag("Enemy")){
                if(attacker.CompareTag("Player") || attacker.CompareTag("Ally")){
                    health -= 20f;
                }
            }
            if(health <= 0f){
                State = EnemyState.Dying;
                // Play death animation or effects here
            }
        }
    }
    */
}
