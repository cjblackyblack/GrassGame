using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/ComboState")]
public class ComboState : SmartState
{
	public SmartState LightAttack;
	public SmartState HeavyAttack;
	public override void OnEnter(SmartObject smartObject)
	{
		if(smartObject.Controller.Button1Buffer > 0 && (smartObject.Controller.Button1Buffer > smartObject.Controller.Button2Buffer))
		{
			smartObject.ActionStateMachine.ChangeActionState(LightAttack);
		}
		else if (smartObject.Controller.Button2Buffer > 0 && (smartObject.Controller.Button2Buffer > smartObject.Controller.Button1Buffer))
		{
			smartObject.ActionStateMachine.ChangeActionState(HeavyAttack);
		}
		else if ((smartObject.Controller.Button1Buffer > 0 && smartObject.Controller.Button2Buffer > 0) && (smartObject.Controller.Button1Buffer == smartObject.Controller.Button2Buffer))
		{
			smartObject.ActionStateMachine.ChangeActionState(HeavyAttack);
		}
		else
		{
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
		}
	}
}