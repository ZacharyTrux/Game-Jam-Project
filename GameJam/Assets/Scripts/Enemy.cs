using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.AI;

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
    public bool isPossessed { get; set; }

    public NavMeshAgent agent;
    public LayerMask groundLayer;
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    private GameObject player;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<GameObject>();
        State = EnemyState.Targeting;
        Type = EnemyType.Melee;
        isPossessed = false;

        agent = GetComponent<NavMeshAgent>();
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
        agent.isStopped = false;
        agent.SetDestination(target.transform.position);

        if(Vector3.Distance(transform.position, target.transform.position) <= attackRange){
            State = EnemyState.Attacking;
        }
    }

    private void HandleAttacking(){
        // need to implement
    }

    private void HandlePossession(){
        agent.isStopped = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f, groundLayer)){
            Vector3 destination = hit.point;
            agent.SetDestination(destination);
        }
        // Allow player to control the enemy's movement and attacks
    }

    private void HandleDeath(){
        // Play death animation and disable enemy
        GameObject.Destroy(gameObject, 2f);
    }

    private void DecideTarget(){
        if (isPossessed){
            State = EnemyState.PlayerControlled;
        }
        else{
            // Logic to find the nearest player or target
            GameObject target = FindNearestTarget();
            if (target != null) {
                HandleTargeting(target);
            }
        }
    }

    private GameObject FindNearestTarget(){
        string[] targetTags = {"Player", "Ally"};
        GameObject nearestTarget = null;
        float minDistance = Mathf.Infinity;
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
    }
}
