using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;
[DefaultExecutionOrder(10000)]
public class RagdollController : MonoBehaviour
{
	private SmartObject smartObject => GetComponentInParent<SmartObject>();
	public List<Collider> Colliders;
	public Vector3 ColliderSize;
	public Vector3 RagdollAnchor;
	public Vector3 CharacterAnchor;
	[Range(0, 1)]
	public float lerp;
	public float VelScale;
	public float GravScale;
	public float TwistScale;
	float ragdollTime;
	public float maxTwistTime;

	[Button("EnableRagdoll")]
	public void EnableRagdoll()
	{

		ragdollTime = 0;



		for (int i = 0; i < Colliders.Count; i++)
		{

			Colliders[0].GetComponent<CharacterJoint>().autoConfigureConnectedAnchor = true;
			Colliders[i].isTrigger = false;
			Colliders[i].GetComponent<Rigidbody>().isKinematic = false;
			smartObject.Animator.enabled = false;
			smartObject.Motor.SetCapsuleDimensions(ColliderSize.x, ColliderSize.y, ColliderSize.z);
			Colliders[0].GetComponent<CharacterJoint>().autoConfigureConnectedAnchor = false;
			Colliders[i].attachedRigidbody.velocity *= 0;
			Colliders[i].attachedRigidbody.angularVelocity *= 0;
			Colliders[i].attachedRigidbody.ResetInertiaTensor();
			Colliders[i].attachedRigidbody.ResetCenterOfMass();

		}


	}

	[Button("DisableRagdoll")]
	public void DisableRagdoll()
	{
		for (int i = 0; i < Colliders.Count; i++)
		{
			Colliders[i].isTrigger = true;
			Colliders[i].GetComponent<Rigidbody>().isKinematic = true;
			smartObject.Animator.enabled = true;
			smartObject.Motor.SetCapsuleDimensions(smartObject.CharacterRadius, smartObject.CharacterHeight, smartObject.CharacterCenter.y);
			//smartObject.Motor.SetPosition(Colliders[0].transform.position, true);
			Colliders[0].GetComponent<CharacterJoint>().connectedAnchor = CharacterAnchor;
			Colliders[0].GetComponent<CharacterJoint>().autoConfigureConnectedAnchor = true;
			Colliders[i].attachedRigidbody.Sleep();
		}
	}

	public void FixedUpdate()
	{

		if (!Colliders[0].isTrigger)
		{
			ragdollTime += Time.deltaTime;
			for (int i = 0; i < Colliders.Count; i++)
			{
				Colliders[i].attachedRigidbody.AddForce(((smartObject.Motor.Velocity * VelScale) + (smartObject.Gravity * GravScale)) * (Colliders[i].attachedRigidbody.mass));
				if (ragdollTime < maxTwistTime)
				{
					Colliders[i].attachedRigidbody.AddTorque(Colliders[0].transform.right * (TwistScale * (ragdollTime / maxTwistTime)), ForceMode.Impulse);
				}
			}

			if (Mathf.Abs(Colliders[0].GetComponent<CharacterJoint>().connectedAnchor.sqrMagnitude - RagdollAnchor.sqrMagnitude) > 0.1f)
			{
				Debug.Log("scootin");
				Colliders[0].GetComponent<CharacterJoint>().connectedAnchor = Vector3.Lerp(Colliders[0].GetComponent<CharacterJoint>().connectedAnchor, RagdollAnchor, lerp);
			}
		}
	}
	//
}
