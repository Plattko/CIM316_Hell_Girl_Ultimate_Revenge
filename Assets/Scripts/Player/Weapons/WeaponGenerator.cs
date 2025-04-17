using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WeaponGenerator : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private WeaponDataSO data;

    private List<WeaponComponent> curWeaponComponents = new List<WeaponComponent>();
    private List<WeaponComponent> addedWeaponComponents = new List<WeaponComponent>();
    private List<Type> componentDependencies = new List<Type>();

    private void Start()
    {
        GenerateWeapon(data);
    }

    public void GenerateWeapon(WeaponDataSO data)
    {
        weapon.SetData(data);
        weapon.Anim.runtimeAnimatorController = data.AnimController;

        curWeaponComponents.Clear();
        addedWeaponComponents.Clear();
        componentDependencies.Clear();

        curWeaponComponents = GetComponents<WeaponComponent>().ToList();
        componentDependencies = data.GetComponentDependencies();

        foreach (Type dependency in componentDependencies)
        {
            if (addedWeaponComponents.FirstOrDefault(component => component.GetType() == dependency)) continue;

            WeaponComponent weaponComponent = curWeaponComponents.FirstOrDefault(component => component.GetType() == dependency);

            if (weaponComponent == null)
            {
                weaponComponent = (WeaponComponent)gameObject.AddComponent(dependency);
            }

            weaponComponent.Initialise();

            addedWeaponComponents.Add(weaponComponent);
        }

        var componentsToRemove = curWeaponComponents.Except(addedWeaponComponents);

        foreach (var weaponComponent in componentsToRemove)
        {
            Destroy(weaponComponent);
        }
    }
}
