using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMachine : MonoBehaviour
{
	public SmartObject smartObject => GetComponent<SmartObject>();
	public List<StatusEffectContainer> statusEffects;

	public SmartState OverrideState()
	{
		foreach (StatusEffectContainer effectContainer in statusEffects)
			if (effectContainer.effect.overrideState != null)
				return effectContainer.effect.overrideState;
		return null;
	}

	public void OnFixedUpdate()
	{
		foreach (StatusEffectContainer effectContainer in statusEffects)
			effectContainer?.OnFixedUpdate(smartObject);
	}

	public void OnTakeDamage(DamageInstance damageInstance)
	{
		if (damageInstance.statusEffects?.Length > 0)
			foreach (StatusEffect statusEffect in damageInstance.statusEffects)
				GetComponent<EffectMachine>().AddEffect(statusEffect, damageInstance.origin);

		foreach (StatusEffectContainer effectContainer in statusEffects)
			effectContainer?.OnTakeDamage(smartObject);
	}

	public void AddEffect(StatusEffect effect, TangibleObject origin)
	{
		StatusEffectContainer effectContainer = new StatusEffectContainer();
		effectContainer.origin = origin;
		effectContainer.effect = effect;
		statusEffects.Add(effectContainer);
		effectContainer.OnAdd(smartObject);
	}

	public void RemoveEffect(StatusEffectContainer effectContainer)
	{
		if (statusEffects.Contains(effectContainer))
			StartCoroutine(RemoveEffectWait(effectContainer));
	}

	IEnumerator RemoveEffectWait(StatusEffectContainer effectContainer)
	{
		yield return new WaitForEndOfFrame();
		statusEffects.Remove(effectContainer);
		effectContainer.OnRemove(smartObject);
	}
}
