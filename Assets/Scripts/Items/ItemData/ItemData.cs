using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public abstract class ItemData : ScriptableObject
{
    [SerializeField] protected int id = default;
    [SerializeField] protected string nameKey = default;
    [SerializeField] protected string desckey = default;
    [SerializeField] private int stackCountMax = default;
    [SerializeField] private float itemMass = default;
    [SerializeField] protected ItemWorldVisual[] worldVisuals = default;


    public int Id => id;
    
    public string NameKey => nameKey;
    
    public string DescKey => desckey;
    
    public int StackCountMax => stackCountMax;

    public float ItemMass => itemMass;

    public ItemWorldVisual[] WorldVisuals => worldVisuals;

    public bool CanStack =>
        stackCountMax > 1;
    
    public abstract ItemType ItemType { get; }

    public virtual Sprite Icon =>
        Resources.Load<Sprite>("Sprites/Items/" + id);
    
    
    public virtual ItemWorldVisual GetItemWorldVisual(int value)
    {
        // TODO: need complete
        return worldVisuals[0];
    }
}
