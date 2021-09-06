using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUnit : Unit
{
    public override void Initialize()
    {
        type = UnitType.Shield;

        color = Color.red;
        maxHealth = 7;

        attackInterval = 5f;
        attackRange = 5f;

        base.Initialize();
    }
}
