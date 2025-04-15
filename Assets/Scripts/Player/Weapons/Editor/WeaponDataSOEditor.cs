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

    private void OnEnable()
    {
        dataSO = target as WeaponDataSO;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        foreach (var dataCompType in dataCompTypes)
        {
            if (GUILayout.Button(dataCompType.Name))
            {
                var comp = (ComponentData)Activator.CreateInstance(dataCompType);

                dataSO.AddData(comp);
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
