using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLedge : MonoBehaviour
{
    // Ladder segment

    // Points to move to when reaching one of the extremities and moving off of the ladder
    public bool CanClimbMove;
    public Transform BottomReleasePoint;
    public Transform TopReleasePoint;

    public AutoLedge TopAttach;
    public AutoLedge BottomAttach;

    // Gets the position of the bottom point of the ladder segment
    public Vector3 BottomAnchorPoint
    {
        get
        {
            return BottomReleasePoint.position;
        }
    }

    // Gets the position of the top point of the ladder segment
    public Vector3 TopAnchorPoint
    {
        get
        {
            return TopReleasePoint.position;
        }
    }

    public Vector3 ClosestPointOnLadderSegment(Vector3 fromPoint, out float onSegmentState)
    {
        Vector3 segment = TopAnchorPoint - BottomAnchorPoint;
        Vector3 segmentPoint1ToPoint = fromPoint - BottomAnchorPoint;
        float pointProjectionLength = Vector3.Dot(segmentPoint1ToPoint, segment.normalized);

        // When higher than bottom point
        if (pointProjectionLength > 0)
        {
            // If we are not higher than top point
            if (pointProjectionLength <= segment.magnitude)
            {
                onSegmentState = 0;
                return BottomAnchorPoint + (segment.normalized * pointProjectionLength);
            }
            // If we are higher than top point
            else
            {
                onSegmentState = pointProjectionLength - segment.magnitude;
                return TopAnchorPoint;
            }
        }
        // When lower than bottom point
        else
        {
            onSegmentState = pointProjectionLength;
            return BottomAnchorPoint;
        }
    }


    public float GetNormalizedPosition(Vector3 fromPoint, out float normalizedPoint)
	{
        Vector3 segment = TopAnchorPoint - BottomAnchorPoint;
        Vector3 segmentPoint1ToPoint = fromPoint - BottomAnchorPoint;
        normalizedPoint = Vector3.Distance(BottomAnchorPoint, fromPoint);
        return normalizedPoint / Vector3.Distance(BottomAnchorPoint, TopAnchorPoint);
	}

    public Vector3 GetPositionFromFloat(float distance)
	{
        return Vector3.Lerp(BottomAnchorPoint, TopAnchorPoint, distance);
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(BottomAnchorPoint, TopAnchorPoint);
        Gizmos.DrawLine(BottomAnchorPoint + Vector3.up * 0.005f, TopAnchorPoint + Vector3.up * 0.005f);
        Gizmos.DrawLine(BottomAnchorPoint + Vector3.down * 0.005f, TopAnchorPoint + Vector3.down * 0.005f);
        Gizmos.DrawLine(BottomAnchorPoint + Vector3.left * 0.005f, TopAnchorPoint + Vector3.left * 0.005f);
        Gizmos.DrawLine(BottomAnchorPoint + Vector3.right * 0.005f, TopAnchorPoint + Vector3.right * 0.005f);
    }
}
