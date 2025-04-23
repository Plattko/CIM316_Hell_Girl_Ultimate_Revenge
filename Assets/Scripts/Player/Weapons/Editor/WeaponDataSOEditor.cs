using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Linq;

[CustomEditor(typeof(WeaponDataSO))]
public class WeaponDataSOEditor : Editor
{
    private static List<Type> dataCompTypes = new List<Type>();

    private WeaponDataSO dataSO;

    private bool showForceUpdateButtons;
    private bool showAddComponentButtons;

    private void OnEnable()
    {
        dataSO = target as WeaponDataSO;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set Number of Attacks"))
        {
            foreach (ComponentData item in dataSO.ComponentData)
            {
                item.InitialiseAttackData(dataSO.NumberOfAttacks);
            }
        }

        showAddComponentButtons = EditorGUILayout.Foldout(showAddComponentButtons, "Add Component Buttons");

        if (showAddComponentButtons)
        {
            foreach (var dataCompType in dataCompTypes)
            {
                if (GUILayout.Button(dataCompType.Name))
                {
                    var comp = (ComponentData)Activator.CreateInstance(dataCompType);

                    comp.InitialiseAttackData(dataSO.NumberOfAttacks);

                    dataSO.AddData(comp);
                }
            }
        }

        showForceUpdateButtons = EditorGUILayout.Foldout(showForceUpdateButtons, "Force Update Buttons");

        if (showForceUpdateButtons)
        {
            if (GUILayout.Button("Force Update Component Names"))
            {
                foreach (ComponentData item in dataSO.ComponentData)
                {
                    item.SetComponentName();
                }
            }

            if (GUILayout.Button("Force Update Attack Names"))
            {
                foreach (ComponentData item in dataSO.ComponentData)
                {
                    item.SetAttackDataNames();
                }
            }
        }
    }

    [DidReloadScripts]
    private static void OnRecompile()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(assembly => assembly.GetTypes());
        var filteredTypes = types.Where(type => type.IsSubclassOf(typeof(ComponentData)) && !type.ContainsGenericParameters && type.IsClass);
        dataCompTypes = filteredTypes.ToList();
    }
}
