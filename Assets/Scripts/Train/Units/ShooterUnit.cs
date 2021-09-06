using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterUnit : Unit
{
    public override void Initialize()
    {
        type = UnitType.Shooter;

        color = Color.cyan;
        maxHealth = 2;

        attackInterval = 1f;
        attackRange = 15f;

        base.Initialize();
    }
}
