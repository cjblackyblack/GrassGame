using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour {

    [Title("Settings")]
    [SerializeField] private InventoryItem item;
    [field: SerializeField] public int Count { get; set; }

    [Title("Object References")]
    [SerializeField] [Tooltip("Object the item gravitates towards")] private GameObject target;

    private Collider sphereCollider;
    private Rigidbody rb;

    // ------------------------------------------------------------------------------------------------------------

    private void Awake() {
        sphereCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == target) {
            Inventory inv = other.GetComponentInParent<Inventory>();
            if(inv) {
                inv.AddItem(item, Count, null);
            }
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        if(target) {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 20f * Time.deltaTime);
        }
    }

    // ------------------------------------------------------------------------------------------------------------

    public void SetTarget(GameObject newTarget) {
        if(target)
            return;

        target = newTarget;
        sphereCollider.isTrigger = true;
        if(rb) {
            rb.useGravity = false;
            rb.isKinematic = false;
        }
    }

    public void SetTargetDelayed(GameObject newTarget, float delay, bool gravity = true) {
        if(target)
            return;

        sphereCollider.isTrigger = true;
        if(rb) {
            rb.useGravity = gravity;
            rb.isKinematic = false;
        }
        StartCoroutine(Set());

        IEnumerator Set() {
            yield return new WaitForSeconds(delay);
            rb.useGravity = false;
            target = newTarget;
        }
    }

}
