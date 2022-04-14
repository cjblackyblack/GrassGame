using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
	public SmartObject SmartObject => GetComponent<SmartObject>();

	public Vector2 lookInput;

	public int ButtonLockBuffer;
	public int ButtonRecenterBuffer;

	public int ButtonLockReleaseBuffer;
	public int ButtonRecenterReleaseBuffer;

	public bool ButtonLockHold;
	public bool ButtonRecenterHold;

	//lol fix this
	public InputActionReference MoveInput;
	public InputActionReference LookInput;
	public InputActionReference Button1Press;
	public InputActionReference Button2Press;
	public InputActionReference Button3Press;
	public InputActionReference Button4Press;
	public InputActionReference Button1Release;
	public InputActionReference Button2Release;
	public InputActionReference Button3Release;
	public InputActionReference Button4Release;
	public InputActionReference PauseButton;

	private void Start()
	{
		PlayerManager.Instance.PlayerController = this;
		PlayerManager.Instance.PlayerObject = SmartObject;
	}

	public override void BeforeObjectUpdate()
	{
		//if (ButtonLockBuffer > 0 && (!ButtonLockHold || ButtonLockReleaseBuffer > 0))
		//{
		//	ButtonLockBuffer = 0;
		//	TargetingManager.Instance.ToggleLockOn();
		//}
		//else if (ButtonLockBuffer <= 1 && ButtonLockHold && ButtonLockReleaseBuffer == 0)
		//{
		//	ButtonLockBuffer = 0;
		//	TargetingManager.Instance.SwitchTarget(SmartObject.PossibleTargets);
		//}
		//if (ButtonRecenterBuffer > 0)
		//{
		//	ButtonRecenterBuffer = 0;
		//	CameraManager.Instance.ResetCamera();
		//}
	}
	public override void PollForTargets()
	{
		SmartObject.PossibleTargets = new Collider[16];
		PhysicsExtensions.OverlapColliderNonAlloc(SmartObject.TargetingCollider, SmartObject.PossibleTargets, TargetingManager.Instance.Targetable);


		foreach (Collider collider in SmartObject.PossibleTargets)
			if (collider != null)
			{ TargetingManager.Instance.AutoTarget(SmartObject.PossibleTargets); break; }
	}

	//CALLED ON PLAYERINPUT COMPONENT AS UNITYEVENT
	public void BufferMovement(InputAction.CallbackContext ctx)
	{
		Input = ctx.ReadValue<Vector2>();
	}

	public void BufferLook(InputAction.CallbackContext ctx)
	{
		lookInput = ctx.ReadValue<Vector2>();
	}

	public void BufferButton1(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			Button1Buffer = 6;
			Button1Hold = true;
		}
	}
	public void BufferRelease1(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			Button1ReleaseBuffer = 6;
			Button1Hold = false;
		}
	}
	public void BufferButton2(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			Button2Buffer = 6;
			Button2Hold = true;
		}
	}
	public void BufferRelease2(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			Button2ReleaseBuffer = 6;
			Button2Hold = false;
		}
	}
	public void BufferButton3(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			Button3Buffer = 6;
			Button3Hold = true;
		}
	}
	public void BufferRelease3(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			Button3ReleaseBuffer = 6;
			Button3Hold = false;
		}
	}
	public void BufferButton4(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			Button4Buffer = 6;
			Button4Hold = true;
		}
	}
	public void BufferRelease4(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			Button4ReleaseBuffer = 6;
			Button4Hold = false;
		}
	}

	public void BufferButtonLock(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			ButtonLockBuffer = 16;
			ButtonLockHold = true;
		}
	}
	public void BufferReleaseLock(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			ButtonLockReleaseBuffer = 10;
			ButtonLockHold = false;
		}
	}

	public void BufferButtonRecenter(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			ButtonRecenterBuffer = 6;
			ButtonRecenterHold = true;
		}
	}
	public void BufferReleaseRecenter(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			ButtonRecenterReleaseBuffer = 6;
			ButtonRecenterHold = false;
		}
	}

	public void PlayerPause(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{

		}
	}

	public void DecrementBuffers()
	{
		if (Button1Buffer > 0)
			Button1Buffer--;
				  
		if (Button1ReleaseBuffer > 0)
			Button1ReleaseBuffer--;

		if (Button2Buffer > 0)
			Button2Buffer--;
				  
		if (Button2ReleaseBuffer > 0)
			Button2ReleaseBuffer--;

		if (Button3Buffer > 0)
			Button3Buffer--;
				  
		if (Button3ReleaseBuffer > 0)
			Button3ReleaseBuffer--;

		if (Button4Buffer > 0)
			Button4Buffer--;

		if (Button4ReleaseBuffer > 0)
			Button4ReleaseBuffer--;

		if (ButtonLockBuffer > 0)
			ButtonLockBuffer--;

		if (ButtonLockReleaseBuffer > 0)
			ButtonLockReleaseBuffer--;

		if (ButtonRecenterBuffer > 0)
			ButtonRecenterBuffer--;

		if (ButtonRecenterReleaseBuffer > 0)
			ButtonRecenterReleaseBuffer--;
	}

	public void Update()
	{
		SmartObject.SetInputDir(Input);
	}

	private void FixedUpdate()
	{
		DecrementBuffers();
	}
}