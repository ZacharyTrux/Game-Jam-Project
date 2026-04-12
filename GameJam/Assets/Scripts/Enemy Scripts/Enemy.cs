using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
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
    public bool isStunned = false;
    public float stunTimer = 0;

    protected GameObject player;
    protected GameObject target;
    protected float lastAttackTime;
    protected Animator animator;
    protected Vector3 groupOffset;

    // The world-space position this enemy has reserved next to its target
    private Vector3 reservedSlotPosition;
    private GameObject lastSlotTarget;

    protected virtual void Awake(){
       player = GameObject.FindGameObjectWithTag("Player");
       animator = GetComponent<Animator>();
    }

    protected virtual void Start(){
        groupOffset = new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f), 0);
        State = EnemyState.Targeting;
        isPossessed = false;
        lastAttackTime = 0f;
    }

    protected virtual void Update(){
        HandleStunTimer();
        if(isStunned) return;
    
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
            // Reserve a slot when we first pick a target, or if the target changed
            if(target != lastSlotTarget){
                ReserveSlotAroundTarget(target);
            }
            HandleTargeting(target.transform.position);
        }
    }

    protected virtual void HandleTargeting(Vector3 targetPosition){
        // Move towards the reserved slot, not the target's exact center
        MoveTowards(reservedSlotPosition);

        // Enter attack state when close enough to the reserved slot
        if(Vector3.Distance(transform.position, reservedSlotPosition) <= 0.25f){
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

        // Release attack slot when becoming possessed
        ReleaseSlot();
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
        float minDistance = 50f; 
        Vector3 currPos = transform.position;

        foreach(string targetTag in targetTags){
            foreach(GameObject t in GameObject.FindGameObjectsWithTag(targetTag)){
                float dist = Vector3.Distance(currPos, t.transform.position);
                if(dist < minDistance){
                    nearestTarget = t;
                    minDistance = dist;
                }
            }
        }
        return nearestTarget;
    }

    protected virtual void HandleDeath(){
        XPManager.Instance?.AddKill();
        SoundManager.Instance?.PlayEnemyDeath();
        SoundManager.Instance?.PlayKill();
        EnemySpawnManager.Instance.enemyCount -= 1;

        ReleaseSlot();
        if(isPossessed) MindControl.Instance?.ReleaseEnemy(this);

        Destroy(gameObject);
    }

    protected virtual void HandlePossession(){
        var currPoint = Waypoints.currentWaypoint;
        if(currPoint != null){ 
            MoveTowards(currPoint.transform.position);
            target = FindNearestTarget();
            if(target != null){
                if(target != lastSlotTarget) ReserveSlotAroundTarget(target);
                State = EnemyState.Attacking;
                return;
            }
        }
        else{
            ReleaseSlot(); // not targeting anyone, free the slot
            OrbitPlayer();
        }
    }

    protected void OrbitPlayer(){
        if(player == null) return;
        float angle = Time.time * 2f + (gameObject.GetInstanceID() * 0.5f);
        float x = Mathf.Cos(angle) * 2.5f;
        float y = Mathf.Sin(angle) * 2.5f;
        MoveTowards(player.transform.position + new Vector3(x, y, 0));
    }

    protected void RotateTowards(Vector3 position){
        Vector2 direction = position - transform.position;
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        if(direction.x > 0) sprite.flipX = false;
        else if(direction.x < 0) sprite.flipX = true;
    }

    protected void ResetState(){
        // Release slot when abandoning a target
        ReleaseSlot();
        State = isPossessed ? EnemyState.PlayerControlled : EnemyState.Targeting;
        animator.SetBool("Walking", false);
    }


    private void ReserveSlotAroundTarget(GameObject newTarget){
        // Release old slot first so we don't hold two at once
        if(lastSlotTarget != null && lastSlotTarget != newTarget)
            AttackSlotManager.Instance?.ReleaseSlot(lastSlotTarget, this);

        lastSlotTarget = newTarget;
        reservedSlotPosition = AttackSlotManager.Instance != null
            ? AttackSlotManager.Instance.ReserveSlot(newTarget, this)
            : newTarget.transform.position;
    }

    private void ReleaseSlot(){
        if(lastSlotTarget != null)
            AttackSlotManager.Instance?.ReleaseSlot(lastSlotTarget, this);
        lastSlotTarget = null;
    }

    protected Vector3 GetAttackStandPosition(){
        if(target == null) return transform.position;
        return reservedSlotPosition;
    }


    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Attack") && State != EnemyState.Dying){
            GameObject attacker = collision.transform.root.gameObject;
            if(gameObject.CompareTag("Ally")){
                if(attacker.CompareTag("Enemy")) health -= 20f;
            }
            else if(gameObject.CompareTag("Enemy")){
                if(attacker.CompareTag("Player") || attacker.CompareTag("Ally")) health -= 20f;
            }
            if(health <= 0f) State = EnemyState.Dying;
        }        
    }

    private void HandleStunTimer(){
        if(!isStunned) return;
        stunTimer -= Time.deltaTime;
        if(stunTimer <= 0) isStunned = false;
    }
}