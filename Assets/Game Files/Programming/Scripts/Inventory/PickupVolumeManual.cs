using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PickupVolumeManual : MonoBehaviour {

    [SerializeField] private Inventory inv;
    public List<ItemPickup> itemsInRange;

    private void OnTriggerEnter(Collider other) {
        ItemPickup item = other.GetComponent<ItemPickup>();
        if(item && item.canBePickedUp && !itemsInRange.Contains(item))
            itemsInRange.Add(item);
    }

    private void OnTriggerExit(Collider other) {
        ItemPickup item = other.GetComponent<ItemPickup>();
        if(itemsInRange.Contains(item))
            itemsInRange.Remove(item);
        // TODO - remove items in list if they get destroyed
    }

    [Button]
    public void Pickup() {
        if(itemsInRange.Count == 0) // Empty list
            return;
        if(!itemsInRange[0]) { // Null item
            itemsInRange.RemoveAt(0);
            Pickup();
            return;
        }

        ItemPickup pickup = itemsInRange[0];
        inv.AddItem(pickup.item, pickup.Count, (leftoverCount) => {
            if(leftoverCount == 0) {
                Destroy(pickup.gameObject);
                itemsInRange.RemoveAt(0);
            } else {
                pickup.Count = leftoverCount;
            }
        });
    }

}
