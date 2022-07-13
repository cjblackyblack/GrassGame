using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotObject : TangibleObject
{
    bool isDug = false;
    [SerializeField] GameObject unDugVisuals = null;
    [SerializeField] GameObject dugVisuals = null;

    public override void TakeDamage(ref DamageInstance damageInstance)//WE GOT HIT FILTERED FROM A HITBOX BEHAVIOUR GO HERE
    {
        if (!isDug && Stats.HP <= 0)
        {
            isDug = true;
            unDugVisuals.SetActive(false);
            dugVisuals.SetActive(true);
        }
        else
        {

        }
    }
}
