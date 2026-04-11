using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
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

public class Enemy : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyState State { get; private set; }
    public EnemyType Type { get; private set; }
    public bool isPossessed;

    public LayerMask groundLayer;
    public float moveSpeed = 3.5f;
    public float attackRange = 10f;
    public float attackCooldown = 1f;
    public float health = 100f;

    private GameObject player;
    private GameObject target;
    private float lastAttackTime;
    private Animator animator;

    void Awake(){
       player = GameObject.FindGameObjectWithTag("Player");
       animator = GetComponent<Animator>();
    }

    void Start(){
        State = EnemyState.Targeting;
        Type = EnemyType.Melee;
        isPossessed = false;
        lastAttackTime = 0f;
    }

    void Update(){
        switch (State)
        {
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

    private void HandleTargeting(GameObject target){
        // Move towards the target and check for attack range
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, target.transform.position) <= attackRange){
            State = EnemyState.Attacking;
        }
    }

    private void HandleAttacking(){
        // need to implement
        Debug.Log("Attacking target");
        if(target == null){
            ResetState();
            return;
        }

        float dist = Vector3.Distance(transform.position, target.transform.position);
        if(dist <= attackRange){
            RotateTowardsTarget(target.transform.position);
            if(Time.time >= lastAttackTime + attackCooldown){
                PerformAttack();
                lastAttackTime = Time.time;
            }
            Debug.Log("Performing attack");
        }
        else{
            ResetState();
        }
    }

    private void HandlePossession(){
        target = FindNearestTarget();
        if(target != null){
            State = EnemyState.Attacking;
            return;
        }

        var currPoint = Waypoints.currentWaypoint;
        if(currPoint != null){ 
            MoveTowards(currPoint.transform.position);
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

    private void HandleDeath(){
        // Play death animation and disable enemy
        GameObject.Destroy(gameObject, 2f);
    }

    private void DecideTarget(){
        // Logic to find the nearest player or target
        target = FindNearestTarget();
        if (target != null) {
            HandleTargeting(target);
        }
    }
    

    private GameObject FindNearestTarget(){
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

    public void MakePossesed(){
        State = EnemyState.PlayerControlled;
        gameObject.tag = "Ally";
        isPossessed = true;
    }

    private void PerformAttack(){
        if(target == null) return;

        if(Type == EnemyType.Melee){
            if(Vector3.Distance(transform.position, target.transform.position) <= attackRange){
                animator.SetTrigger("Attack");
                Debug.Log("Melee attack hits " + target.name);
                //target.Health.TakeDamage(10f);
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

    private void OrbitPlayer(){
        if(player == null) return;

        Vector3 direction = (transform.position - player.transform.position).normalized;
        Vector3 orbitPosition = player.transform.position + direction * 1f; // 1f is orbit radius
        MoveTowards(orbitPosition);
    }

    private void ResetState(){
        State = isPossessed ? EnemyState.PlayerControlled : EnemyState.Targeting;
    }
}
