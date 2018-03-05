using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public int maxResources = 500;
    int _resourcesQuantity;
    public int resourcesQuantity
    {
        get { return _resourcesQuantity; }
        set
        {
            _resourcesQuantity = value;
            if (_resourcesQuantity <= 0)
                Destroy(gameObject);
            transform.localScale = initialScale * _resourcesQuantity / maxResources;
        }
    }

    Vector3 initialScale;
    void Start()
    {
        initialScale = transform.localScale;
        resourcesQuantity = maxResources;
        
    }

}
