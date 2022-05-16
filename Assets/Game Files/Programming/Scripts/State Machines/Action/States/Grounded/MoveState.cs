using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Move")]
public class MoveState : SmartState
{
    public float MaxStableMoveSpeed;
    public float StableMovementSharpness;

	public override void OnEnter(SmartObject smartObject)
	{
		if (smartObject.LocomotionStateMachine.PreviousLocomotionEnum == LocomotionStates.GroundedShoot && smartObject.ActionStateMachine.PreviousActionEnum == ActionStates.Move)
		{

		}
		else
		{
			smartObject.CurrentTime = -1;
			smartObject.CurrentFrame = -1;
		}
		if (AnimationTransitionTime != 0)
		{
			smartObject.Animator.CrossFadeInFixedTime(AnimationState, AnimationTransitionTime, 0, AnimationTransitionOffset);
		}
		else
		{
			smartObject.Animator.Play(AnimationState, 0, 0);
		}
		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
	}
	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		if ((smartObject.Controller.Button1Buffer > 0 || smartObject.Controller.Button2Buffer > 0))
		{
			if (smartObject.PreviousAttack != null && smartObject.PreviousAttackBuffer > 0)
			{
				for (int i = 0; i < (smartObject.PreviousAttack as AttackState).StateTransitions.Length; i++)
				{
					if ((smartObject.PreviousAttack as AttackState).StateTransitions[i].CanTransition(smartObject, smartObject.PreviousAttack))
					{
						smartObject.ActionStateMachine.ChangeActionState((smartObject.PreviousAttack as AttackState).StateTransitions[i].TransitionState);
						break;
					}
				}
			}
			else
			{
				smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);
			}
		}



		if (smartObject.Controller.Button3Buffer > 0)
		{
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Dodge);
		}

		if (smartObject.Controller.Button4Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);

		if (smartObject.InputVector == Vector3.zero)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);

        smartObject.MovementVector = smartObject.InputVector;
    }

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
        smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
    }
}