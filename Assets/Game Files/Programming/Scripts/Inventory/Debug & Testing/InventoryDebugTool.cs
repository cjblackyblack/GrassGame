using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InventoryDebugTool : MonoBehaviour {

    [TitleGroup("Add Item")]
    public Inventory invAdd;
    public InventoryItem itemAdd;
    public int countAdd;

    [Button]
    public void AddItem() {
        invAdd.AddItem(itemAdd, countAdd, null);
    }

    // ------------------------------------------------------------------------------------------------------------

    [TitleGroup("Spawn Item")]
    public GameObject itemSpawn;
    public int countSpawn;
    public int stackSizeSpawn;
    public float delay;
    public GameObject target;

    [Button]
    public void SpawnItem() {
        for(int i = 0; i < countSpawn; i++) {
            GameObject newItem = Instantiate(itemSpawn);
            newItem.transform.position = transform.position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            newItem.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-3, 3), Random.Range(3, 8), Random.Range(-3, 3));
            newItem.GetComponent<ItemPickup>().Count = stackSizeSpawn;
        }
    }

    [Button]
    public void SpawnItemAutoGravity() {
        for(int i = 0; i < countSpawn; i++) {
            GameObject newItem = Instantiate(itemSpawn);
            newItem.transform.position = transform.position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            newItem.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-3, 3), Random.Range(3, 8), Random.Range(-3, 3));
            newItem.GetComponent<ItemPickup>().Count = stackSizeSpawn;
            newItem.GetComponent<ItemPickup>().SetGravityTargetDelayed(target, delay);
        }
    }


}
