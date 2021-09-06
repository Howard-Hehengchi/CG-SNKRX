using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnit : Unit
{
    /// <summary>
    /// ��ʼ��ʱ���ø�����
    /// </summary>
    public override void Initialize()
    {
        type = UnitType.Normal;

        maxHealth = 5;

        attackInterval = 2f;
        attackRange = 10f;

        base.Initialize();
    }
}
