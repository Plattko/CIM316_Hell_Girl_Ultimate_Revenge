using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ComponentData
{
    [SerializeField, HideInInspector] private string name;

    public Type ComponentDependency { get; protected set; }

    public ComponentData()
    {
        SetComponentName();
    }

    public void SetComponentName() => name = GetType().Name;

    public virtual void SetAttackDataNames() { }

    public virtual void InitialiseAttackData(int numberOfAttacks) { }
}

[Serializable]
public class ComponentData<T> : ComponentData where T : AttackData
{
    [SerializeField] private T[] attackData;
    public T[] AttackData { get => attackData; private set => attackData = value; }

    public override void SetAttackDataNames()
    {
        base.SetAttackDataNames();
        
        for (int i = 0; i < AttackData.Length; i++)
        {
            AttackData[i].SetAttackName(i + 1);
        }
    }

    public override void InitialiseAttackData(int numberOfAttacks)
    {
        base.InitialiseAttackData(numberOfAttacks);

        int oldLength = attackData != null ? AttackData.Length : 0;

        if (oldLength == numberOfAttacks) return;

        Array.Resize(ref attackData, numberOfAttacks);

        if (oldLength < numberOfAttacks)
        {
            for (int i = oldLength; i < attackData.Length; i++)
            {
                var newObj = (T)Activator.CreateInstance(typeof(T));
                attackData[i] = newObj;
            }
        }

        SetAttackDataNames();
    }
}
