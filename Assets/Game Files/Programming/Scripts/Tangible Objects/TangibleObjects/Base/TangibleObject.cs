using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class TangibleObject : MonoBehaviour, IChemistryObject, ITimeScaleable
{
    public MeshRenderer MeshRenderer => GetComponentInChildren<MeshRenderer>();


    public AudioSource AudioSource => GetComponent<AudioSource>();

    [PropertyOrder(-99)]
    [TitleGroup("Stats")]
    public Stats Stats;
    [TitleGroup("Properties"), PropertyOrder(-20)]
    public BaseObjectProperties BaseObjectProperties;
    [TitleGroup("Variables")]
    [OnValueChanged("SetTimeScale")]
    [FoldoutGroup("Variables/Time")]
    public float LocalTimeScale;
    public List<Collider> Hurtboxes;

    public Collider[] Hitboxes = new Collider[8];
    public Collider[] TargetPosiitions;
    public virtual void Start()
	{
        //Hurtboxes = new List<Collider>();
	}

    public virtual void TakeDamage(ref DamageInstance damageInstance)//WE GOT HIT FILTERED FROM A HITBOX BEHAVIOUR GO HERE
    {
        if (damageInstance.hitStun > 0)
        {

        }
        else
        {

        }
    }

    //Mainly here for non state machine objects, if the object has a state machine, put its behaviour there!!!
    public virtual bool HitConfirmReaction(PhysicalObjectTangibility hitTangibility, DamageInstance damageInstance)//WE HIT SOMETHING FILTERED THROUGH A HITBOX BEHAVIOUR GO HERE
    {
        switch (hitTangibility)
        {
            case PhysicalObjectTangibility.Normal:
                {
                    
                }
                break;
            case PhysicalObjectTangibility.Armor:
                {
                    if(FlagsExtensions.HasFlag(damageInstance.breakthroughType, BreakthroughType.ArmorPierce))
					{

					}
                }
                break;
            case PhysicalObjectTangibility.Guard:
                {
                    if (FlagsExtensions.HasFlag(damageInstance.breakthroughType, BreakthroughType.GuardPierce))
                    {

                    }
                }
                break;
            case PhysicalObjectTangibility.Invincible:
                {

                }
                break;
            case PhysicalObjectTangibility.Intangible:
                {

                }
                break;
        }
        return true;//change to false later and only set to true when we want it better to be safe than sorry
    }

    public void OnChemistryInteraction()
	{

	}

	public void SubscribeTime()
	{

	}

	public void UnsubscribeTime()
	{

	}

	public virtual void SetTimeScale(float speed)
	{
        LocalTimeScale = speed;
	}

    public IEnumerator WaitDestroyObject()
    {
        //GameEventManager.current.OnObjectDestroy(this);
        yield return new WaitForFixedUpdate();
        Destroy(this.gameObject);
    }
}