using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Collider))]
public class ItemPickup : TangibleObject {

    [Title("Settings")]
    public InventoryItem item;
    [field: SerializeField] public int Count { get; set; }
    public bool canBePickedUp = true;

    [Title("Object References")]
    [SerializeField] [Tooltip("Object the item gravitates towards")] private GameObject gravityTarget;

    private Collider sphereCollider;
    private Rigidbody rb;

    // ------------------------------------------------------------------------------------------------------------

    #region Unity Functions

    private void Awake() {
        sphereCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == gravityTarget) {
            Inventory inv = other.GetComponentInParent<Inventory>();
            if(inv) {
                inv.AddItem(item, Count, null);
            }
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        if(gravityTarget) {
            transform.position = Vector3.MoveTowards(transform.position, gravityTarget.transform.position, 20f * Time.deltaTime);
        }
    }

    #endregion

    // ------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------

    #region Gravity Targets

    public void SetGravityTarget(GameObject newTarget) {
        if(gravityTarget)
            return;

        gravityTarget = newTarget;
        sphereCollider.isTrigger = true;
        if(rb) {
            rb.useGravity = false;
            rb.isKinematic = false;
        }
    }

    public void SetGravityTargetDelayed(GameObject newTarget, float delay, bool gravity = true) {
        if(gravityTarget)
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
            gravityTarget = newTarget;
        }
    }

    #endregion

}
