using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : PhysicalObject
{
    public TangibleObject SourceObject;
    public ProjectileStateMachine StateMachine => GetComponent<ProjectileStateMachine>();

    public bool CanTrack;
    float trackingTimer;
    public int MaxCollisions;
    public int MaxHitConfirms;

    public bool AlignToGravity;
    public bool DestroyOnDamage;
    public GameObject HurtFX;
    public override void Start()
    {
        //base.Start();
        Initialize();
        gameObject.name = BaseObjectProperties.Name;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanTrack && trackingTimer != 0)
        {
            if (CurrentFrame > trackingTimer)
                CanTrack = true;
            if (CurrentFrame > trackingTimer - 60)
                CanBounce = true;
        }
        StateMachine.OnUpdate();
    }

    private void FixedUpdate()
    {

        CurrentTime += LocalTimeScale;
        if (CurrentTime - CurrentFrame >= 1)
        {

            CurrentFrame = (int)CurrentTime;
            StateMachine.OnFixedUpdate();
            if (AlignToGravity)
                AlignModelToGravity();


        }
        for (int i = 0; i < Hitboxes.Length; i++)
        {
            {
                Hitboxes[i].GetComponent<Hitbox>().PreviousPos = Hitboxes[i].transform.position;
            }
        }
    }
    

	public override void TakeDamage(ref DamageInstance damageInstance)
	{
        // transform.up = damageInstance.origin.transform.up;
        transform.forward = damageInstance.origin.transform.forward;
        if(Target)
            transform.forward = (damageInstance.origin.transform.forward  + (damageInstance.origin.transform.up * 0.5f));
        RBody.AddForce(((damageInstance.knockbackDirection.x * damageInstance.origin.transform.right) + (damageInstance.knockbackDirection.y * damageInstance.origin.transform.up) + (damageInstance.knockbackDirection.z * damageInstance.origin.transform.forward)) * damageInstance.knockbackStrength * RBody.mass, ForceMode.VelocityChange);
        CanTrack = false;
        CanBounce = false;
        trackingTimer = CurrentFrame + (damageInstance.hitStun * 2);
        if(HurtFX != null)
            Instantiate(HurtFX, transform.position, Quaternion.identity);
        if (DestroyOnDamage)
            Destroy(this.gameObject);
    }

    private void Initialize()
	{
        //Direction = transform.forward;
	}

    public void AlignModelToGravity()
    {
            // Rotate from current up to invert gravity
            Vector3 smoothedGravityDir = Vector3.Slerp(MeshRenderer.transform.rotation * Vector3.up, -Gravity.normalized, 1 - Mathf.Exp(-25 * Time.deltaTime));
            MeshRenderer.transform.rotation = Quaternion.FromToRotation(MeshRenderer.transform.rotation * Vector3.up, smoothedGravityDir) * MeshRenderer.transform.rotation;
	}
}