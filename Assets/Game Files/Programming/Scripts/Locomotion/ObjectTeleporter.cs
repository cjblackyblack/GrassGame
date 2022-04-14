using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


    public class ObjectTeleporter : MonoBehaviour
    {
        public ObjectTeleporter TeleportTo;

        public UnityAction<SmartObject> OnCharacterTeleport;

        public bool isBeingTeleportedTo { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!isBeingTeleportedTo)
            {
            SmartObject cc = other.GetComponent<SmartObject>();
                if (cc)
                {
                    cc.Motor.SetPositionAndRotation(TeleportTo.transform.GetChild(0).GetChild(0).position, TeleportTo.transform.rotation);

                    if (OnCharacterTeleport != null)
                    {
                        OnCharacterTeleport(cc);
                    }
                    TeleportTo.isBeingTeleportedTo = true;
                }
            }

            isBeingTeleportedTo = false;
        }
    }
