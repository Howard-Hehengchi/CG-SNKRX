using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitFactory : ScriptableObject
{
    [SerializeField]
    private Unit[] prefabs;

    public Unit Get(UnitType type)
    {
        Unit unit = Instantiate(prefabs[(int)type]);
        unit.Initialize();
        return unit;
    }
}
