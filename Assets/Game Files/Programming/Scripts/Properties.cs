using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

#region Gameplay
#region Interfaces
[SerializeField]
public interface ITimeScaleable
{
    void SubscribeTime();
    void UnsubscribeTime();
    void SetTimeScale(float speed);
}

[SerializeField]
public interface IChemistryObject
{
    void OnChemistryInteraction();
}

public interface IInteractable
{
    void OnInteract();
}
#endregion

#region StateEnums

public enum OrientationMethod
{
    TowardsCamera,
    TowardsMovement,
}

public enum BonusOrientationMethod
{
    None,
    TowardsGravity,
    TowardsGroundSlopeAndGravity,
}

/// <summary>
/// Used to determine what kind of velocity to apply. 
/// All states belong in one of the LocomotionState enum values
/// Changing the locomotion state always updates the SmartObject's SmartState Dictionary
/// </summary>
public enum LocomotionStates
{ 
    Grounded, 
    Aerial,
    GroundedShoot, //variant states for holding a gun out like DMC / Bayonetta
    AerialShoot,
    Climbing,
    Flying, 
    Swimming, 
    Submerged, 
    Vehicle 
}

/// <summary>
/// Used as key for the objects current action.
/// Current LocomotionState and Current Equipped Item greatly affect the dictionary value
/// </summary>
public enum ActionStates
{ 
    Idle, 
    Move, 
    Jump, 
    Dodge, 
    Attack,
    Blocked,
    Guard, 
    Hurt, 
    Other 
}
//          Locomotion Translations
//                        IDLE      MOVE        JUMP        DOGE        ATTACK      BLOCKED  GUARD       HURT        OTHER
//			Grounded    { Idle,     Move,       Jump,       Dodge,      Attack,     Blocked, Guard,      Hurt,       Other }
//          Crouched    { Crouch,   CrouchMove, CrouchJump, Slide,      LowAttack,  Blocked, LowGuard,   CrouchHurt, Other }
//			Aerial		{ Fall,		Drift,		AirDodge,	Glide,		AirAttack,	Blocked, AirGuard,	AirHurt,	Other }
//			Flying		{ FlyIdle,	FlyMove,	AirDodge,	FlyDash,	AirAttack,	Blocked, AirGuard,	FlyHurt,	Other }
//			Climbing	{ ClimbIdle, ClimbMove,	ClimbJump,	ClimbDash,	NONE,		Blocked, NONE,		AirHurt,	Other }
//	 		Swimming	{ SwimIdle,	SwimMove,	SwimJump,	SwimDash,	NONE,		Blocked, NONE,		SwimHurt,	Other }
//			Submerged	{ SubIdle,	SubMove,	SubDodge,	SubDash,	SubAttack,	Blocked, SubGuard,	SubHurt,	Other }

public enum ClimbingState
{
    Anchoring,
    Climbing,
    DeAnchoring
}
#endregion

#region Targeting
/// <summary>
/// Makes sure attacks don't hit teammates, unless they want to
/// 
/// </summary>

public enum TargetingState
{
    None,
    Auto,
    Locked
}

[System.Flags]
public enum TargetingDistance
{
    Close,
    Medium,
    Long
}

public enum Alliance
{
    Ally        = 1,
    Enemy       = 2,
    Neutral     = 4,
}

[System.Flags]
public enum ActionTarget 
{
    Ally        = 1,
    Enemy       = 2,
    Neutral     = 4,
    Self        = 8
}


public enum PersonalityType
{
    Friendly,   //targets any ally first
    Mean,       //targets any enemy first
    Brave,      //targets highest HP
    Smart,      //targets highest MP
    Lazy,       //targets low HP
    Dumb,       //targets closest
    Vengeful,   //targets highest recent damage offender
    Cowardly,   //targets lowest recent damage offender
}

public enum ValueType
{
    HP,
    PercentHP,
    MaxHP,
    MP,
    PercentMP,
    MaxMP
}

[System.Flags]
public enum Comparators
{
    GreaterThan = 1,
    LessThan = 2,
    EqualTo = 4
}
#endregion

#region Chemistry
public enum ChemistryObjectMaterial
{
    Organic,
    Wood,
    Rock,
    Metal,
    Water
}

public enum ChemistryObjectState
{
    Neutral,
    Burnt,      //SmartObjects go to Fire State
    Shocked,    //SmartObjects go to Shocked State
    Frozen,     //SmartObjects go to Frozen State
    Wet
}
#endregion

#region Combat
public enum PhysicalObjectTangibility 
{ 
    Normal, 
    Armor,
    Guard, 
    Invincible, 
    Intangible 
}

[System.Flags]
public enum ActionProperties
{
    Attack      = 1,
    Dodge       = 2,
    Fire        = 4,
    Elect       = 8,
    Ice         = 16,
    Water       = 32,
    Air         = 64,
    Cure        = 128,      
    MPCure      = 256,      
    Confuse     = 512,      //Randomize Input Dir, Enable Friendly Fire (stacks) //Also changes Alliances Temp
    Poison      = 1024,     //Damage Tick (stacks)
    Silence     = 2048,     //No Spells
    Blind       = 4096,     //Miss attacks (stacks)
    Sleep       = 8192,     //Sleep State
    Petrify     = 16384,    //Stuck in place state //was kinda hoping to use this one as a surprise puzzle mechanic
    Slow        = 32768,    //affects global time flow or just slows objects down
    Death       = 65536,    //death magic
    ClearMind   = 131072,
    Antidote    = 262144,
    EchoHerb    = 524288,
    EyeDrop     = 1048576,
    Awake       = 2097152,
    GoldNeedle  = 4194304,
    Haste       = 8388608,
    Life        = 16777216
}

[System.Flags]
public enum GroupFlag
{
    _0 = 1,
    _1 = 2,
    _2 = 4,
    _3 = 8,
    _4 = 16,
    _5 = 32,
    _6 = 64,
    _7 = 128,
   // _8 = 256,
   // _9 = 512,    
   // _10 = 1024,  
   // _11 = 2048,  
   // _12 = 4096,  
   // _13 = 8192,  
   // _14 = 16384, 
   // _15 = 32768
}

[System.Flags]
public enum BreakthroughType { None, ArmorPierce, GuardPierce } //Using flag for (if  player attack breaks past armor but does not ignore superarmor, enemy is guarding and their stance is not broken but the player is not recoiled

public enum KnockbackType { Knockback, Launch, Flyback, Knockdown }

public enum HitstopType { Global, Participants, ReceiverOnly}

public enum TransitionType {Jump, Land, Light, Heavy, LightCharge, HeavyCharge, Auto}


[System.Serializable]
public class StateTransition 
{
    [HideInInspector]
    public int MaxTime;
    [MinMaxSlider("0", "@MaxTime", true)]
    public Vector2Int TransitionWindow;
    public TransitionType TransitionType;
    public SmartState TransitionState;

    public bool CanTransition(SmartObject smartObject, SmartState lateCombo = null)
	{
		switch (TransitionType) 
        {
            case TransitionType.Jump:
                {
                    if (smartObject.Controller.Button4Buffer > 0 && smartObject.CurrentFrame >= TransitionWindow.x && smartObject.CurrentFrame <= TransitionWindow.y)
                        return true;
                    return false;
                }
            case TransitionType.Land:
                {
                    if (smartObject.CurrentFrame >= TransitionWindow.x && smartObject.CurrentFrame <= TransitionWindow.y)
                        smartObject.LocomotionStateMachine.LandState = TransitionState;
                    return false;
                }
            case TransitionType.Light:
				{
                    if (lateCombo == null)
                    {
                        if (smartObject.Controller.Button1Buffer > 0 && smartObject.CurrentFrame >= TransitionWindow.x && smartObject.CurrentFrame <= TransitionWindow.y)
                            return true;
                    }
                    else if (smartObject.Controller.Button1Buffer > 0 && TransitionWindow.y == lateCombo.MaxTime)
                        return true;
                    return false;
				}
            case TransitionType.Heavy:
                {
                    if (lateCombo == null)
                    {
                        if (smartObject.Controller.Button2Buffer > 0 && smartObject.CurrentFrame >= TransitionWindow.x && smartObject.CurrentFrame <= TransitionWindow.y)
                            return true;
                    }
                    else if (smartObject.Controller.Button2Buffer > 0 && TransitionWindow.y == lateCombo.MaxTime)
                        return true;
                    return false;
                }
            case TransitionType.LightCharge:
                {
                    if (smartObject.Controller.Button1Buffer > 0 && smartObject.Controller.Button1Hold && smartObject.Controller.Button1Buffer == 0 && smartObject.CurrentFrame == TransitionWindow.y)
                        return true;
                    return false;
                }
            case TransitionType.HeavyCharge:
                {
                    if (smartObject.Controller.Button2Buffer > 0 && smartObject.Controller.Button2Hold && smartObject.Controller.Button2Buffer == 0 && smartObject.CurrentFrame == TransitionWindow.y)
                        return true;
                    return false;
                }
            case TransitionType.Auto:
                {
                    if (smartObject.CurrentFrame >= TransitionWindow.x && smartObject.CurrentFrame <= TransitionWindow.y)
                        return true;
                    return false;
                }

        }
        return false;
	}
}


[System.Serializable]
public struct DamageInstance
{
    [ReadOnly]
    public TangibleObject origin;
    public int ID;
    [ReadOnly]
    public StatusEffect[] statusEffects;
    [ReadOnly]
    public float damage;
    [ReadOnly]
    public bool flatDamage;
    [ReadOnly]
    public bool ignoreProtections;
    [ReadOnly]
    public bool useMagic;
    [ReadOnly]
    public bool unstoppable; //continue attack even if we should have been parried? Used for combo attacks with specific openings
    [ReadOnly]
    public int hitStun;
    [ReadOnly]
    public float hitStopTime;
    [ReadOnly]
    public HitstopType hitstopType;
    [ReadOnly]
    public BreakthroughType breakthroughType;
    [ReadOnly]
    public KnockbackType knockbackType;
    [ReadOnly]
    public float knockbackStrength;
    [ReadOnly]
    public Vector3 knockbackDirection;


    [ReadOnly]
    public bool AttackIndividualBoxes;

    public DamageInstance(TangibleObject _origin, int _ID, StatusEffect[] _statusEffects, bool _unstoppable,
                            float _damage, int _hitStun,
                            float _hitStopTime, HitstopType _hitstopType ,BreakthroughType _breakthroughType,
                            KnockbackType _knockbackType , float _knockbackStrength, Vector3 _knockbackDirection, 
                            bool _flatDamage, bool _ignoreProtections, bool _useMagic, bool _singleHurtbox)
    {
        origin = _origin;
        ID = _ID;
        statusEffects = _statusEffects;
        unstoppable = _unstoppable;
        damage = _damage;
        hitStun = _hitStun;

        hitStopTime = _hitStopTime;
        hitstopType = _hitstopType;
        breakthroughType = _breakthroughType;

        knockbackType = _knockbackType;
        knockbackStrength = _knockbackStrength;
        knockbackDirection = _knockbackDirection;

        flatDamage = _flatDamage;
        ignoreProtections = _ignoreProtections;
        useMagic = _useMagic;

        AttackIndividualBoxes = _singleHurtbox;
    }
}
[System.Serializable]
public class MotionCurve 
{
    public AnimationCurve ForwardCurve;
    public AnimationCurve LateralCurve;
    public AnimationCurve VerticalCurve;
    public AnimationCurve YRotationCurve;
    public AnimationCurve GravityModCurve;
    public AnimationCurve FreeMoveCurve;
    public AnimationCurve RotationTrackingCurve;

    public AnimationCurve DistanceTrackingCurve;
    public AnimationCurve DistanceRangeCurve;


    public int TurnAroundTime; //CHECK IF CAN JUST USE TRACKING CURVE
    public float TrackingResistance;

    public Vector3 GetFixedTotalCurve(SmartObject smartObject)
	{
        return (smartObject.Motor.CharacterForward * ForwardCurve.Evaluate(smartObject.CurrentFrame))
         + (smartObject.Motor.CharacterRight * LateralCurve.Evaluate(smartObject.CurrentFrame))
         + (smartObject.Motor.CharacterUp * VerticalCurve.Evaluate(smartObject.CurrentFrame))
         + (smartObject.InputVector.normalized * FreeMoveCurve.Evaluate(smartObject.CurrentFrame));
    }

    public Vector3 GetFixedTotalCurve(SmartObject smartObject, bool UseStoreForFreeMove)
    {
        return (smartObject.Motor.CharacterForward * ForwardCurve.Evaluate(smartObject.CurrentFrame) * smartObject.Controller.Input.y)
         + (smartObject.Motor.CharacterRight * LateralCurve.Evaluate(smartObject.CurrentFrame) * smartObject.Controller.Input.x)
         + (smartObject.Motor.CharacterUp * VerticalCurve.Evaluate(smartObject.CurrentFrame))
         + ( UseStoreForFreeMove ? (smartObject.MovementVector.normalized * FreeMoveCurve.Evaluate(smartObject.CurrentFrame)) : (smartObject.InputVector.normalized * FreeMoveCurve.Evaluate(smartObject.CurrentFrame)));
    }

    public Vector3 GetFixedForwardCurve(SmartObject smartObject)
    {
        return (smartObject.Motor.CharacterForward * ForwardCurve.Evaluate(smartObject.CurrentFrame))
         + (smartObject.Motor.CharacterRight * LateralCurve.Evaluate(smartObject.CurrentFrame))
     //  + (smartObject.Motor.CharacterUp * VerticalCurve.Evaluate(smartObject.CurrentFrame))
         + (smartObject.InputVector.normalized * FreeMoveCurve.Evaluate(smartObject.CurrentFrame));
    }

    public Vector3 GetFixedTotalCurve(PhysicalObject physicalObject)
	{
        return (physicalObject.transform.forward * ForwardCurve.Evaluate(physicalObject.CurrentFrame));
        // + (physicalObject.transform.right * LateralCurve.Evaluate(physicalObject.CurrentFrame))
        // + (physicalObject.transform.up * VerticalCurve.Evaluate(physicalObject.CurrentFrame));
    }

    public Vector3 GetFixedTotalCurve(ProjectileObject projectileObject)
    {
        return (projectileObject.transform.forward * ForwardCurve.Evaluate(projectileObject.CurrentFrame));
        // + (physicalObject.transform.right * LateralCurve.Evaluate(physicalObject.CurrentFrame))
        // + (physicalObject.transform.up * VerticalCurve.Evaluate(physicalObject.CurrentFrame));
    }



    public float GravityMod(SmartObject smartObject)
	{
        return smartObject.GravityModifier = GravityModCurve.Evaluate(smartObject.CurrentFrame);
    }

    public Vector3 Rotation(SmartObject smartObject)
    {
        return Quaternion.AngleAxis(YRotationCurve.Evaluate(smartObject.CurrentFrame), smartObject.Motor.CharacterUp) * smartObject.Motor.CharacterForward;
    }

    public void Rotation(ProjectileObject projectileObject)
	{

        projectileObject.transform.rotation *=  Quaternion.Euler(new Vector3(VerticalCurve.Evaluate(projectileObject.CurrentFrame), LateralCurve.Evaluate(projectileObject.CurrentFrame), YRotationCurve.Evaluate(projectileObject.CurrentFrame)));
        //return ((VerticalCurve.Evaluate(projectileObject.CurrentFrame) * projectileObject.transform.right) * (LateralCurve.Evaluate(projectileObject.CurrentFrame) * projectileObject.transform.up) * (YRotationCurve.Evaluate(projectileObject.CurrentFrame) * projectileObject.transform.forward));

        //projectileObject.transform.rotation = Quaternion.AngleAxis(VerticalCurve.Evaluate(projectileObject.CurrentFrame), projectileObject.transform.right);

        //projectileObject.transform.rotation = Quaternion.AngleAxis(VerticalCurve.Evaluate(projectileObject.CurrentFrame), projectileObject.transform.right); Quaternion.AngleAxis(LateralCurve.Evaluate(projectileObject.CurrentFrame), projectileObject.transform.up);

        //projectileObject.transform.rotation = Quaternion.AngleAxis(VerticalCurve.Evaluate(projectileObject.CurrentFrame), projectileObject.transform.right); Quaternion.AngleAxis(YRotationCurve.Evaluate(projectileObject.CurrentFrame), projectileObject.transform.forward);

        //projectileObject.transform.rotation *= Quaternion.AngleAxis(VerticalCurve.Evaluate(projectileObject.CurrentFrame), projectileObject.transform.right);

        //projectileObject.transform.rotation *= Quaternion.AngleAxis(LateralCurve.Evaluate(projectileObject.CurrentFrame), projectileObject.transform.up);



        //return Quaternion.LookRotation(new Vector3(VerticalCurve.Evaluate(projectileObject.CurrentFrame), LateralCurve.Evaluate(projectileObject.CurrentFrame), YRotationCurve.Evaluate(projectileObject.CurrentFrame)), projectileObject.transform.up);










        //Vector3.Cross(projectileObject.Direction, Vector3.right);
        //return new Vector3(LateralCurve.Evaluate(projectileObject.CurrentFrame), VerticalCurve.Evaluate(projectileObject.CurrentFrame), YRotationCurve.Evaluate(projectileObject.CurrentFrame));
        //return new Vector3(VerticalCurve.Evaluate(projectileObject.CurrentFrame), LateralCurve.Evaluate(projectileObject.CurrentFrame) + projectileObject.transform.eulerAngles.y, 0f); //NEED ROLL CURVE OR SOMETHIN ForwardCurve.Evaluate(projectileObject.CurrentFrame) + projectileObject.transform.eulerAngles.z);
    }

    public Quaternion TurnAroundRotation(SmartObject smartObject,ref Vector3 currentVelocity ,bool overrideVelocity = false)
    {
        
        smartObject.MovementVector = smartObject.InputVector.normalized;
        Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, (smartObject.MovementVector.normalized), 1 - Mathf.Exp(-100 * Time.fixedDeltaTime)).normalized;
        if (overrideVelocity)
        {

            currentVelocity = Quaternion.AngleAxis(Vector3.SignedAngle(smartObject.Motor.CharacterForward, smartObject.MovementVector.normalized, smartObject.Motor.CharacterUp), smartObject.Motor.CharacterUp) * currentVelocity;
        }
        return Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);
    }


    public Quaternion TrackingRotation(SmartObject smartObject, int rotationSpeed)//Target is assumed here
    {
        if (!smartObject.Target)
            return Quaternion.identity;

        Vector3 rotationVector = Vector3.ProjectOnPlane((smartObject.Target.transform.position - smartObject.transform.position).normalized, smartObject.Motor.CharacterUp).normalized;
        Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, (rotationVector), 1 - Mathf.Exp(-rotationSpeed * Time.fixedDeltaTime)).normalized;
        return Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);
    }

    public Quaternion TrackingRotation(ProjectileObject projectileObject)//Target is assumed here
    {
        if (!projectileObject.Target)
            return Quaternion.identity;

        Vector3 rotationVector = (projectileObject.Target.transform.position - projectileObject.transform.position).normalized;
        Vector3 smoothedLookInputDirection = Vector3.Slerp(projectileObject.transform.forward, (rotationVector), 1 - Mathf.Exp(-RotationTrackingCurve.Evaluate(projectileObject.CurrentFrame) * Time.fixedDeltaTime)).normalized;
        return Quaternion.LookRotation(smoothedLookInputDirection, projectileObject.transform.up);
    }



    public Vector3 TrackingVelocity(PhysicalObject physicalObject)
    {
        if (!physicalObject.Target)
            return Vector3.zero;

        if (DistanceRangeCurve.Evaluate(physicalObject.CurrentFrame) == 0)
            return Vector3.zero;

        float distance = Vector3.Distance(physicalObject.Target.transform.position, physicalObject.transform.position);

        if (distance < 1f)//we are already in range
            return Vector3.zero;

        //Debug.Log(Mathf.Clamp01(distance / (DistanceRangeCurve.Evaluate(physicalObject.CurrentFrame))));
        return ((Mathf.Clamp01(distance/(DistanceRangeCurve.Evaluate(physicalObject.CurrentFrame)))  * DistanceTrackingCurve.Evaluate(physicalObject.CurrentFrame)) * (physicalObject.Target.transform.position - physicalObject.transform.position).normalized);
	}

    public Vector3 TrackingVelocity(ProjectileObject projectileObject)
    {
        if (!projectileObject.Target)
            return Vector3.zero;

        if (DistanceRangeCurve.Evaluate(projectileObject.CurrentFrame) == 0)
            return Vector3.zero;

        float distance = Vector3.Distance(projectileObject.Target.transform.position, projectileObject.transform.position);

        //Debug.Log(Mathf.Clamp01(distance / (DistanceRangeCurve.Evaluate(physicalObject.CurrentFrame))));
        return ((Mathf.Clamp01(distance / (DistanceRangeCurve.Evaluate(projectileObject.CurrentFrame))) * DistanceTrackingCurve.Evaluate(projectileObject.CurrentFrame)) * (projectileObject.Target.transform.position - projectileObject.transform.position).normalized);
    }

    public Vector3 TrackingVelocity(PhysicalObject physicalObject, int time)
    {
        if (!physicalObject.Target)
            return Vector3.zero;

        if (DistanceRangeCurve.Evaluate(physicalObject.CurrentFrame) == 0)
            return Vector3.zero;

        float distance = Vector3.Distance(physicalObject.Target.transform.position, physicalObject.transform.position);
        if (distance < 1f)//we are already in range
            return Vector3.zero;

        //Debug.Log(Mathf.Clamp01(distance / (DistanceRangeCurve.Evaluate(time))));
        return ((Mathf.Clamp01(distance / (DistanceRangeCurve.Evaluate(time))) * DistanceTrackingCurve.Evaluate(time)) * (physicalObject.Target.transform.position - physicalObject.transform.position).normalized);
    }

}
[System.Serializable]
public class TangibilityFrames
{
    public bool Hurtbox;
    public GroupFlag BoxGroup;
    public PhysicalObjectTangibility ActiveTangibility;
    public AnimationCurve ActiveFrames; // if ActiveFrames == 1 ? ActiveTang : DefaultTang //should change to additive maybe? box.Iframes += Eval()
}

[System.Serializable]
public class HitboxData
{
    [HideInInspector]
    public int MaxTime;
    public ActionTarget ActionTarget;
    private int[] Values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    public bool RefreshID;
    public bool ShareID;
    public bool AttackBoxesIndividually;
    public bool ShareIncomingHitboxes;
    [ValueDropdown("Values")]
    public int Hitbox;
    public GroupFlag HitboxGroup;

    [MinMaxSlider("0", "@MaxTime", true)]
    public Vector2Int ActiveFrames;
    public StatusEffect[] StatusEffects;

    public float Damage;
    public bool FlatDamage;
    public bool IgnoreProtections;
    public bool UseMagic;
    public bool Unstoppable;
    public int Hitstun;
    public float HitStopTime;
    public HitstopType HitstopType;

    public BreakthroughType BreakthroughType;

    public KnockbackType KnockbackType;
    public float KnockbackStrength;
    public Vector3 KnockbackDirection;

    public HitboxProcess[] HitboxProcesses;

}

[System.Serializable]
public class Hitstop
{
    public int duration;
    public float speed;
    public HitstopType HitstopType;
}
#endregion

#region Entities
[System.Serializable]
public class BaseObjectProperties
{
    [HorizontalGroup("ID", LabelWidth = 75)]
    public string Name;
    [HorizontalGroup("ID", LabelWidth = 15)]
    public int Priority;

    public Alliance BaseAlliance;
    [HorizontalGroup("Chemistry")]
    [BoxGroup("Chemistry/Material")]
    [HideLabel]
    public ChemistryObjectMaterial chemistryMaterial;
    [BoxGroup("Chemistry/State")]
    [HideLabel]
    public ChemistryObjectState chemistryState;

    [FoldoutGroup("Property Relationships")]
    public ActionProperties Weaknesses;
    [FoldoutGroup("Property Relationships")]
    public ActionProperties Resistances;
    [FoldoutGroup("Property Relationships")]
    public ActionProperties Immunities;
}

[System.Serializable]
public class PhysicalObjectProperties 
{
}

[System.Serializable]
public class SmartObjectProperties
{
    public Alliance Alliance;
    public PhysicalObjectTangibility CurrentTangibility;
    public int IFrames;
    public int ArmorFrames;

}

[System.Serializable]
public class SmartControllerProperties
{
    public PersonalityType PersonalityType;
    public Vector2Int actionSpeed;
}

[System.Serializable]
public class Stats //beginning of actively measured stats that exist on the  object initially determined by BaseStats but then modified by StatMods
{
    [VerticalGroup("Stats")]
    //[FoldoutGroup("Stats/Level")]
    //public int Level;
    //[FoldoutGroup("Stats/Level")]
    //public int EXP;
    [FoldoutGroup("Stats/Resources")]
    [BoxGroup("Stats/Resources/HP")]
    public int HP;
    [FoldoutGroup("Stats/Resources")]
    [BoxGroup("Stats/Resources/HP")]
    public int MaxHP;
    [FoldoutGroup("Stats/Resources")]
    [BoxGroup("Stats/Resources/MP")]
    public int MP;
    [FoldoutGroup("Stats/Resources")]
    [BoxGroup("Stats/Resources/MP")]
    public int MaxMP;
    [FoldoutGroup("Stats/Combat")]
    public int Strength;
    [FoldoutGroup("Stats/Combat")]
    public int Magic;
    [FoldoutGroup("Stats/Combat")]
    public int Defense;
    [FoldoutGroup("Stats/Combat")]
    public int Resistance;
    [FoldoutGroup("Stats/Movement")]
    public int MoveSpeed;
    [FoldoutGroup("Stats/Movement")]
    public int AirMoveSpeed;
    [FoldoutGroup("Stats/Other")]
    public int Luck; 
}

[System.Serializable]
public class StatMods //modifications applied to Stats
{
    public float MaxHPMod;
    public float MaxMPMod;
    public float DamageMod; //Overall Damage Modifier (IE ComboBoost from KH2)
    public float KnockbackMod; //Overall Knockback Modifier
    public float StrengthMod; //Calculated Phyiscal Damage Modifier
    public float MagicMod; //Calculated Magical Damage Modifier
    public float DefenseMod;//Calculated Phyiscal Resistance Modifier
    public float ResistanceMod;//Calculated Magical Resistance Modifier
    public float MoveSpeedMod;
    public float AirMoveSpeedMod;
    public float SpeedMod; //LocalTime Modifier
    public float ThoughtMod;
    public float AgilityMod;
    public float LuckMod;
    public float TimeMod;
    public float RangeMod;
    public float InputMod;
    public float EXPMod;
    public float PrizeMod;
}

[System.Serializable]
public class BaseStats //Exists on the job, of the object
{
    public AnimationCurve EXP;
    public AnimationCurve maxHP;
    public AnimationCurve maxMP;
    public AnimationCurve strength;
    public AnimationCurve magic;
    public AnimationCurve defense;
    public AnimationCurve resistance;
    public AnimationCurve speed;
    public AnimationCurve accuracy;
    public AnimationCurve agility;
    public AnimationCurve luck;
    public AnimationCurve thoughtPower;
    public AnimationCurve actionSpeedRangeMin;
    public AnimationCurve actionSpeedRangeMax;
    public AnimationCurve cost;
}

[System.Serializable]
public class BodyReferences 
{
    public GameObject[] ShootFX;
    public GameObject[] BoostFX;
    public GameObject[] HoverFX;
    public GameObject Weapon1;
    public GameObject Weapon2;
}

#endregion

#region Misc
[System.Serializable]
public class Rewards
{
    public AnimationCurve EXP;
    public AnimationCurve money;
    //ITEMS
}

[System.Serializable]
public class LedgeData 
{
    public bool CanGrab;
    public ClimbingState ClimbingState;
    public AutoLedge ActiveLedge;
    public BoxCollider LedgeGrabber;
    public float AnchorTime;
    public float LedgeInput;
    public float NormalizedPosition;
    public float LedgeSegmentState; 
    public float AnchorReOffset; //max offset before we are off the ledge
    public Quaternion RotationBeforeClimbing;
    public Vector3 TargetLedgePos;
    public Quaternion TargetLadderRot;
    public Vector3 AnchorStartPos;
    public Quaternion AnchorStartRot;
    public Vector2 AnchorClamp; //offset for chaining ledges together to not be forever stuck
}

public enum PlayerCharacter { Player1, Player2 }
public enum BodyVFX { Weapon1, Weapon2 }

[System.Serializable]
public class BodyVFXContainer
{
    public BodyVFX BodyVFX;
    public int Time;
    public bool Toggle;
}

[System.Serializable]
public class VFXContainer 
{
    public GameObject VFX;
    public Vector3 Position;
    public Vector3 Rotation;
    public int Time;
    public bool Local;
}

[System.Serializable]
public class SFXContainer 
{
    public SFX SFX;
    public int Time;
}

[System.Serializable]
public class ProjectileContainer
{
    public int Transform;
    public int Time;
    public bool Toggle;
}

#endregion
#endregion

#region Gamework
#region Managers
public enum GameState { Start, CharacterSelect, Loading, Intro, Gameplay, Paused, Credits, GameOver, Quitting, Controls }
#endregion
#endregion