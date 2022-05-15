using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

// First X slots of the player's inventory is the hotbar, and the remaining slots are the main inventory
//  The two areas (hotbar/main inv) can be accessed as separate inventories, even though in code it is stored all as one
public class PlayerInventory : Inventory {

    [field: TitleGroup("Inventory"), SerializeField, OnValueChanged("OnUpdateInventorySize"), MinValue(0)]
    public int HotbarSize { get; private set; }

    // ---

    protected override void OnUpdateInventorySize() {
        FullInventorySize = SlotCount + HotbarSize;
        // TODO - update actual inventory to match
    }

    // ------------------------------------------------------------------------------------------------------------

    #region Fetching Info

    public InventorySlot GetMainSlot(int slot) {
        return GetSlot(slot + HotbarSize);
    }

    public InventoryItem GetMainItem(int slot) {
        return GetItem(slot + HotbarSize);
    }

    public int GetMainItemCount(int slot) {
        return GetItemCount(slot + HotbarSize);
    }

    public bool IsMainSlotEmpty(int slot) {
        return IsSlotEmpty(slot + HotbarSize);
    }

    // ---

    public InventorySlot GetHotbarSlot(int slot) {
        if(slot >= HotbarSize)
            throw new System.IndexOutOfRangeException();
        return GetSlot(slot);
    }

    public InventoryItem GetHotbarItem(int slot) {
        if(slot >= HotbarSize)
            throw new System.IndexOutOfRangeException();
        return GetItem(slot);
    }

    public int GetHotbarItemCount(int slot) {
        if(slot >= HotbarSize)
            throw new System.IndexOutOfRangeException();
        return GetItemCount(slot);
    }

    public bool IsHotbarSlotEmpty(int slot) {
        if(slot >= HotbarSize)
            throw new System.IndexOutOfRangeException();
        return IsSlotEmpty(slot);
    }

    #endregion

}
