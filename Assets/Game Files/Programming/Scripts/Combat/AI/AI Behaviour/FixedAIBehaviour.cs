using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehaviour/FixedBehaviour")]
public class FixedAIBehaviour : AIBehaviour
{

    public AnimationCurve ForwardCurve;
    public AnimationCurve StrafeCurve;

    public AnimationCurve Button1Curve;
    public AnimationCurve Button2Curve;
    public AnimationCurve Button3Curve;
    public AnimationCurve Button4Curve;

    public override void UpdateBehaviour(AIController aiController)
	{
		if(Button1Curve.Evaluate(aiController.CurrentTime) > 0)
		{
            aiController.Button1Buffer = 6;
            aiController.Button1Hold = true;

        }
        else if (aiController.Button1Hold == true)
		{
            aiController.Button1ReleaseBuffer = 6;
            aiController.Button1Hold = false;
        }

        if (Button2Curve.Evaluate(aiController.CurrentTime) > 0)
        {
            aiController.Button2Buffer = 6;
            aiController.Button2Hold = true;

        }
        else if (aiController.Button2Hold == true)
        {
            aiController.Button2ReleaseBuffer = 6;
            aiController.Button2Hold = false;
        }


        if (Button3Curve.Evaluate(aiController.CurrentTime) > 0)
        {
            aiController.Button3Buffer = 6;
            aiController.Button3Hold = true;

        }
        else if (aiController.Button3Hold == true)
        {
            aiController.Button3ReleaseBuffer = 6;
            aiController.Button3Hold = false;
        }


        if (Button4Curve.Evaluate(aiController.CurrentTime) > 0)
        {
            aiController.Button4Buffer = 6;
            aiController.Button4Hold = true;

        }
        else if (aiController.Button4Hold == true)
        {
            aiController.Button4ReleaseBuffer = 6;
            aiController.Button4Hold = false;
        }

        aiController.Input = new Vector2(StrafeCurve.Evaluate(aiController.CurrentTime), ForwardCurve.Evaluate(aiController.CurrentTime)).normalized;
    }


}