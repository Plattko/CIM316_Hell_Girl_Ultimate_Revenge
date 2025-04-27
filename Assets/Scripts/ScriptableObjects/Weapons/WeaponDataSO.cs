using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Data/Weapon Data/Basic Weapon Data", order = 0)]
public class WeaponDataSO : Item
{
    [field: SerializeField] public int NumberOfAttacks { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController AnimController { get; private set; }
    [field: SerializeField] public AudioClip[] swingSFX { get; private set; }

    [field: SerializeReference] public List<ComponentData> ComponentData { get; private set; }

    public T GetData<T>()
    {
        return ComponentData.OfType<T>().FirstOrDefault();
    }

    public List<Type> GetComponentDependencies()
    {
        return ComponentData.Select(component => component.ComponentDependency).ToList();
    }

    public void AddData(ComponentData data)
    {
        if (ComponentData.FirstOrDefault(t => t.GetType() == data.GetType()) != null) return;
        
        ComponentData.Add(data);
    }
}
