using System.Collections.Generic;
using UnityEngine;

public class AttackSlotManager : MonoBehaviour{
    public static AttackSlotManager Instance {get; private set;}
    public float slotRadius = 1.2f;
    public int slotsPerTarget = 8;
    private Dictionary<GameObject, List<SlotReservation>> reservations = new();

    void Awake(){
        if(Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector3 ReserveSlot(GameObject target, Enemy enemy){
        if(target == null) return Vector3.zero;

        // Ensure the target has a slot list
        if(!reservations.ContainsKey(target))
            reservations[target] = new List<SlotReservation>();

        List<SlotReservation> slots = reservations[target]; // was missing — slots was never assigned

        // Release any existing slot this enemy holds around this target
        ReleaseSlot(target, enemy);

        // Find the first free slot index and reserve it
        for(int i = 0; i < slotsPerTarget; i++){
            if(!SlotTaken(slots, i)){
                slots.Add(new SlotReservation(i, enemy)); // was misspelled "SlotReseration"
                return SlotPosition(target.transform.position, i);
            }
        }

        // All slots full — nudge slightly so enemies don't perfectly overlap
        return target.transform.position + (Vector3)(Random.insideUnitCircle.normalized * slotRadius);
    }

    public Vector3 GetReservedSlotPosition(GameObject target, Enemy enemy){
        if(target == null) return Vector3.zero;
        if(!reservations.ContainsKey(target)) return target.transform.position;

        foreach(SlotReservation r in reservations[target]){ // was "reservation[target]" (missing 's')
            if(r.owner == enemy)
                return SlotPosition(target.transform.position, r.slotIndex);
        }

        // No reservation found for this enemy
        return target.transform.position; // was inside the loop — only checked the first element
    }

    public void ReleaseSlot(GameObject target, Enemy enemy){
        if(target == null || !reservations.ContainsKey(target)) return;
        reservations[target].RemoveAll(r => r.owner == enemy);
    }

    public void ReleaseAllSlots(Enemy enemy){
        foreach(var kvp in reservations){
            kvp.Value.RemoveAll(r => r.owner == enemy);
        }
    }

    private Vector3 SlotPosition(Vector3 center, int slotIndex){
        float angle = slotIndex * (360f / slotsPerTarget) * Mathf.Deg2Rad;
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * slotRadius; // was Mathf.cos (lowercase c)
    }

    private bool SlotTaken(List<SlotReservation> slots, int index){
        foreach(SlotReservation r in slots){
            if(r.slotIndex == index) return true;
        }
        return false; // was inside the loop — exited after checking only the first slot
    }

    private class SlotReservation{
        public int slotIndex;
        public Enemy owner;
        public SlotReservation(int index, Enemy owner) { slotIndex = index; this.owner = owner; }
    }
}