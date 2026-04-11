using UnityEngine;

public enum EnemyState{
    Targeting_Player,
    Targeting_Pet,
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

public class EnemyScript : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    EnemyState State;
    EnemyType Type;
    

    void Start(){
        State = EnemyState.Targeting_Player;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
