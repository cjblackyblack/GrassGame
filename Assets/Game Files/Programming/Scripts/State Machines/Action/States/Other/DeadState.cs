using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Other/Dead")]
public class DeadState : SmartState
{


	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);

		smartObject.OrientationMethod = OrientationMethod.TowardsMovement;
		smartObject.Target = null;
		smartObject.InputVector = Vector3.zero;
		smartObject.MovementVector = Vector3.zero;
		smartObject.StoredMovementVector = Vector3.zero;
		smartObject.Motor.BaseVelocity *= 0;
		smartObject.Controller.Button1Buffer = 0;
		smartObject.Controller.Button1Hold = false;
		smartObject.Controller.Button1ReleaseBuffer = 0;

		smartObject.Controller.Button2Buffer = 0;
		smartObject.Controller.Button2Hold = false;
		smartObject.Controller.Button2ReleaseBuffer = 0;

		smartObject.Controller.Button3Buffer = 0;
		smartObject.Controller.Button3Hold = false;
		smartObject.Controller.Button3ReleaseBuffer = 0;

		smartObject.Controller.Button4Buffer = 0;
		smartObject.Controller.Button4Hold = false;
		smartObject.Controller.Button4ReleaseBuffer = 0;

		smartObject.Controller.enabled = false;
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		CreateVFX(smartObject);
		CreateSFX(smartObject);

		if (smartObject.CurrentFrame >= MaxTime)
		{
			smartObject.CurrentFrame = -1;
			smartObject.CurrentFrame = -1;
		}
	}
}