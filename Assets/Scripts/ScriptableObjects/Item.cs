using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [Header("Item Variables")]
    public new string name;
    public string description;
    public Sprite icon;
}
