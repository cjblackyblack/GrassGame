using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGunVisibility : MonoBehaviour
{
    public LocomotionStateMachine ParentMachine;// => GetComponentInParent<LocomotionStateMachine>();
    public MeshRenderer mesh;// => GetComponent<MeshRenderer>();
    public bool reverse;
    // Update is called once per frame
    void Update()
    {

        if (ParentMachine.CurrentLocomotionEnum == LocomotionStates.AerialShoot || ParentMachine.CurrentLocomotionEnum == LocomotionStates.GroundedShoot)
        {
            mesh.enabled = reverse;
        }
        else
            mesh.enabled = !reverse;
    }
}