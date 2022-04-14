using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : BaseController
{
    public SmartObject SmartObject => GetComponent<SmartObject>();

	public AIBehaviour CurrentBehaviour;
    public float CurrentTime;

	private void Start()
	{
		CurrentTime = Random.Range(0, 1000);
	}
	private void FixedUpdate()
	{
        CurrentTime += 1;
	}

	public override void BeforeObjectUpdate()
	{
		if(this.enabled)
		CurrentBehaviour.UpdateBehaviour(this);
        //Input = new Vector2(CurrentBehaviour.ForwardCurve.Evaluate(Time.time), CurrentBehaviour.StrafeCurve.Evaluate(CurrentTime));
        SmartObject.SetInputDir(Input, true);
		DecrementBuffers();
	}

	public void ChangeBehaviour(AIBehaviour newBehaviour)
	{
		CurrentBehaviour = newBehaviour;
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
	}
}
