using System;
using UnityEngine;

public class MeleeEnemy : Enemy {
    protected override void Awake() {
        base.Awake();
    }

    protected override void Start(){
        base.Start();
        Type = EnemyType.Melee;
        slotWeight = 1;
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

    private void PerformAttack(){
        if(target == null) return;
        if(Vector3.Distance(transform.position, target.transform.position) <= attackRange){
            animator.SetBool("Walking", false);
            animator.SetTrigger("Attack");
        }
        
    }
}