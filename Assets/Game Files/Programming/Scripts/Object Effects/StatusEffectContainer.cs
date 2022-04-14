using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectContainer
{
    public TangibleObject origin;
    public int effectTime;
    public StatusEffect effect;

    public void OnAdd(SmartObject smartObject)
    {
        effectTime = effect.maxTime;
        effect.OnEnter(smartObject, origin);
    }

    public void OnFixedUpdate(SmartObject smartObject)
    {
        effectTime--;
        if (effectTime % effect.tickRate == 0 && (effectTime != 0 || effect.maxTime % effect.tickRate == 0))
            effect.OnTick(smartObject, origin);
        if (effectTime <= 0)
            smartObject.EffectMachine.RemoveEffect(this);
    }

    public void OnTakeDamage(SmartObject smartObject)
    {
        effect.OnTakeDamage(smartObject, origin);
	}

    public void OnRemove(SmartObject smartObject)
	{
        effect.OnExit(smartObject, origin);
	}
}
