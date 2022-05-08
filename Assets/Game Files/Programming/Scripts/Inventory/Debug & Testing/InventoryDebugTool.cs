using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InventoryDebugTool : MonoBehaviour {

    public Inventory inv;
    public InventoryItem item;
    public int count;

    [Button]
    public void Test() {
        inv.AddItem(item, count, null);
    }

}
