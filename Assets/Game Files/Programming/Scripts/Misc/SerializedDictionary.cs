using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedDictionary<Key, Value> : Dictionary<Key, Value>, ISerializationCallbackReceiver {
    [SerializeField, HideInInspector]
    private List<KeyValue> serializedPairs = new List<KeyValue>();

    [System.Serializable]
    public struct KeyValue {
        [SerializeField]
        public Key key;
        [SerializeField]
        public Value value;
    }

    public void OnAfterDeserialize() {
        Clear();
        foreach(KeyValue pair in serializedPairs) {
            Add(pair.key, pair.value);
        }
    }

    public void OnBeforeSerialize() {
        serializedPairs.Clear();
        foreach(KeyValuePair<Key, Value> pair in this) {
            serializedPairs.Add(new KeyValue() { key = pair.Key, value = pair.Value });
        }
    }
}