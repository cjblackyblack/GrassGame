using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "0 - New Item", menuName = "Item")]
public class InventoryItem : ScriptableObject, IComparable<InventoryItem>, IEquatable<InventoryItem> {

    [field: Title("Identifier"), SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public uint ID { get; private set; }

    // ---

    [field: Title("Information"), SerializeField]
    public ItemType Type { get; private set; }
    [field: SerializeField]
    public int MaxStackCount { get; private set; } = 1;
    [field: SerializeField]
    public Sprite Icon { get; private set; }
    public SerializedDictionary<string, string> metadata;

    // ------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------

    int IComparable<InventoryItem>.CompareTo(InventoryItem other) {
        throw new NotImplementedException();
    }

    public bool Equals(InventoryItem other) {
        return other && other.ID == ID;
    }

}

/*public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        GUI.enabled = false;
        if(string.IsNullOrEmpty(property.stringValue)) {
            property.stringValue = System.Guid.NewGuid().ToString();
        }
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif*/ // Use [ScriptableObjectId] attribute
