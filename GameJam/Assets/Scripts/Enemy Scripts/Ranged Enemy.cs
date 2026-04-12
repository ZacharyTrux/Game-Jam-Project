using System;
using UnityEngine;

public class RangedEnemy : Enemy {

    public GameObject attackProjectilePrefab;
    protected override void Awake() {
        base.Awake();
    }

    protected override void Start(){
        base.Start();
        Type = EnemyType.Ranged;
        attackRange = 5f;
        attackCooldown = 0.5f;
        slotWeight = 2;
    }

    protected override void MoveTowards(Vector3 position) {
        if(State == EnemyState.Attacking){
            animator.SetBool("Walking", false);
            return;
        }

        animator.SetBool("Walking", true);
        transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);
        RotateTowards(position);
    }

    protected override void HandleAttacking() {
        if(target == null){
            ResetState();
            return;
        }

        float dist = Vector3.Distance(transform.position, target.transform.position);
        if(dist <= attackRange){
            RotateTowards(target.transform.position);
            if (dist < attackRange) { 
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

    protected override void HandlePossession() {
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
    }

    private void PerformAttack(){
        if(target == null) return;
        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion spawnRotation = Quaternion.Euler(0,0, angle);

        GameObject projectile = Instantiate(attackProjectilePrefab, transform.position, spawnRotation);
        projectile.GetComponent<RangedAttack>().ownerTag = gameObject.tag;
    }
}