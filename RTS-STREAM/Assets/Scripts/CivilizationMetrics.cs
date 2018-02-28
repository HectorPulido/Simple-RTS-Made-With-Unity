using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationMetrics : MonoBehaviour
{
    public int maxTroops;
    [SerializeField]
    int _troops;
    public int troops
    {
        get { return _troops; }
        set
        {
            _troops = value;
            whenResourcesChanges.Invoke(_troops);
        }
    }
    public MyIntEvent whenResourcesChanges;

    public static CivilizationMetrics singleton;

    void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }
}
