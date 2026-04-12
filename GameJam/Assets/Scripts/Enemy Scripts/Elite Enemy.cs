using System;
using UnityEngine;

public class EliteEnemy : Enemy {

    protected override void Awake(){
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        Type = EnemyType.Elite;
        health = 200f;
        attackRange = 1.5f;
        attackCooldown = 4f;
    }

    protected override void MoveTowards(Vector3 position) {
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
                animator.SetBool("Walking", true);
            }
            else{
                animator.SetBool("Walking", false);
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