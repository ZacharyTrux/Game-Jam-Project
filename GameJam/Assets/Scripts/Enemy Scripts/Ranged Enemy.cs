using System;
using UnityEngine;

public class RangedEnemy : Enemy {

    public GameObject attackProjectilePrefab;
    public RangedEnemy() {
        Type = EnemyType.Ranged;
        attackRange = 5f;
        attackCooldown = 0.5f;
        slotWeight = 2;
    }

    protected override void MoveTowards(Vector3 position) {
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
            if (dist > attackRange) { 
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

    private void OrbitPlayer() {
        if (player == null) return;
        float angle = Time.time * 2f + (gameObject.GetInstanceID() * 0.5f);
        Vector3 orbitOffset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 2.5f;
        MoveTowards(player.transform.position + orbitOffset);
    }

    private void PerformAttack(){
        if(target == null) return;
        if(Vector3.Distance(transform.position, target.transform.position) <= attackRange){
            GameObject projectile =Instantiate(attackProjectilePrefab, gameObject.transform.position, gameObject.transform.rotation);
            projectile.GetComponent<RangedAttack>().ownerTag = gameObject.tag;
            //target.Health.TakeDamage(10f);
        }
        
    }
}