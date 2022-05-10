using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupVolumeGravity : MonoBehaviour {

    [SerializeField] private GameObject parent;

    private void OnTriggerEnter(Collider other) {
        ItemPickup pickup = other.GetComponent<ItemPickup>();
        if(pickup) {
            pickup.SetGravityTarget(parent);
        }
    }

}
