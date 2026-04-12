using System;
using UnityEngine;

public class Boss : Enemy {

    public GameObject attackProjectilePrefab;
    public Boss() {
        Type = EnemyType.Boss;
        health = 1000f;
        attackRange = 10f;
        attackCooldown = 2f;
        slotWeight = 1000000;
        moveSpeed = 1f;
    }

    protected override void MoveTowards(Vector3 position)
    { 
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


    private void PerformAttack(){
        if(target == null) return;
        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion spawnRotation = Quaternion.Euler(0,0, angle);

        GameObject projectile = Instantiate(attackProjectilePrefab, transform.position, spawnRotation);
        projectile.GetComponent<RangedAttack>().ownerTag = gameObject.tag;
    }
}