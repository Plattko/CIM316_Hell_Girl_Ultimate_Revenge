using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Data/Weapon Data/Basic Weapon Data", order = 0)]
public class WeaponDataSO : Item
{
    [field: SerializeField] public int NumberOfAttacks { get; private set; }

    [field: SerializeReference] public List<ComponentData> ComponentData { get; private set; }

    public T GetData<T>()
    {
        return ComponentData.OfType<T>().FirstOrDefault();
    }

    public void AddData(ComponentData data)
    {
        if (ComponentData.FirstOrDefault(t => t.GetType() == data.GetType()) != null) return;
        
        ComponentData.Add(data);
    }
}
