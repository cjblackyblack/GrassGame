using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Inventory : MonoBehaviour {

    [Title("Inventory")]
    [SerializeField] private InventorySlot[] slots;
    [field: SerializeField] public int SlotCount { get; set; }

    // ------------------------------------------------------------------------------------------------------------

    #region Inventory Management

    // -----------------------------------------------------------

    #region Adding

    /// <summary>
    /// Adds the given item to the first available slot.
    /// Stacks with existing item stacks, if applicable.
    /// Will fill other inventory slots if needed & possible.
    /// </summary>
    /// <param name="newItem">New item to be added</param>
    /// <param name="count">Amount of items to be added</param>
    /// <param name="OnComplete">Action with input (int overflowCount) to be called after adding is complete.
    ///     overflowCount = number of items leftover that could not be added</param>
    /// <returns>Whether or not at least 1 item was added to the inventory</returns>
    public bool AddItem(InventoryItem newItem, int count, Action<int> OnComplete) {
        if(count <= 0)
            throw new IndexOutOfRangeException();

        int remainingCount = count;
        for(int i = 0; i < slots.Length; i++) {
            // Create new stack || Fill existing (not full) stack
            if(slots[i].IsEmpty() || (slots[i].item.Equals(newItem) && !slots[i].IsFull())) {
                slots[i].item = newItem;
                int openSlotCount = newItem.MaxStackCount - slots[i].Count;
                int countToAdd = Mathf.Min(remainingCount, openSlotCount);
                slots[i].Count += countToAdd;
                remainingCount -= countToAdd;
            }
            // Check if complete
            if(remainingCount == 0) {
                OnComplete?.Invoke(0);
                return true;
            }
        }

        // Failed to add
        if(remainingCount == count) {
            return false;
        }
        // Items leftover
        else {
            OnComplete?.Invoke(remainingCount);
            return true;
        }
    }

    /// <summary>
    /// Adds the given item to the given slot. 
    /// Stacks with an existing item stack, if applicable.
    /// Will fail if the slot is filled with an item of a different type.
    /// </summary>
    /// <param name="newItem">New item to be added</param>
    /// <param name="count">Amount of items to be added</param>
    /// <param name="OnComplete">Action with input (int overflowCount) to be called after adding is complete.
    ///     overflowCount = number of items leftover that could not be added</param>
    /// <returns>Whether or not at least 1 item was added to the inventory</returns>
    public bool AddItem(InventoryItem newItem, int count, int slot, Action<int> OnComplete) {
        throw new NotImplementedException();
    }

    #endregion

    // -----------------------------------------------------------

    #region Swapping

    /// <summary>
    /// Swaps the contents of two inventory slots
    /// </summary>
    public void SwapSlots(InventorySlot other, int slot) {
        SwapSlots(other, slots[slot]);
    }

    /// <summary>
    /// Swaps the contents of two inventory slots
    /// </summary>
    public static void SwapSlots(InventorySlot slot1, InventorySlot slot2) {
        throw new NotImplementedException();
    }

    #endregion

    // -----------------------------------------------------------

    #region Removing & Deleting

    public void DeleteItem(int slot) {
        slots[slot].item = null;
        slots[slot].Count = 0;
    }

    #endregion

    // -----------------------------------------------------------

    #region Fetching Info

    public InventorySlot GetSlot(int slot) => slots[slot];

    public InventoryItem GetItem(int slot) => slots[slot].item;

    public int GetItemCount(int slot) => slots[slot].Count;

    public bool IsSlotEmpty(int slot) => slots[slot].IsEmpty();

    #endregion

    // -----------------------------------------------------------

    #region Debug Stuff

    [TitleGroup("Buttons")] [Button]
    private void ResetSlots() {
        slots = new InventorySlot[SlotCount];
    }

    #endregion

    // -----------------------------------------------------------

    #endregion

}

[Serializable]
public class InventorySlot {

    public InventoryItem item;
    [field: SerializeField] public int Count { get; set; }

    public bool IsEmpty() {
        return Count == 0;
    }

    public bool IsFull() {
        return Count >= item.MaxStackCount;
    }

}