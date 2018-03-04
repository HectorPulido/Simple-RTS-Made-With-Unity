using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationMetrics : MonoBehaviour
{
    public FactionType faction;

    public int maxTroops;
    [SerializeField]
    int _troops;
    public int troops
    {
        get { return _troops; }
        set
        {
            _troops = value;
            whenTroopsChanges.Invoke(_troops);
        }
    }
    public MyIntEvent whenTroopsChanges;

    [SerializeField]
    int Resources;
    public int resources
    {
        get { return Resources; }
        set
        {
            Resources = value;
            whenResourcesChanges.Invoke(Resources);
        }
    }
    public MyIntEvent whenResourcesChanges;


    public static Dictionary<FactionType,CivilizationMetrics> singleton = new Dictionary<FactionType, CivilizationMetrics>();

    void Awake()
    {
        if (singleton.ContainsKey(faction))
        {
            Destroy(gameObject);
            return;
        }
        singleton.Add(faction,this);
    }


}
