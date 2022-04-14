using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    public Vector3 _parentOffset = new Vector3(0f, 0.01f, 0f);
    public LayerMask _layerMask;

    void LateUpdate()
    {

        Ray ray = new Ray(transform.parent.position, -Vector3.up);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _layerMask))
        {
            // Position
            transform.position = hitInfo.point + _parentOffset;

            // Rotate to same angle as ground

            transform.up = hitInfo.normal;
            transform.Rotate(transform.parent.eulerAngles);
        }
        else
        {
            // If raycast not hitting (air beneath feet), position it far away
            transform.position = new Vector3(0f, 1000f, 0f);
        }
    }
}