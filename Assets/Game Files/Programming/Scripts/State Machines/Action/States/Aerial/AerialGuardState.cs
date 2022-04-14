using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Aerial/AerialGuard")]
public class AerialGuardState : SmartState
{
	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
	}
}