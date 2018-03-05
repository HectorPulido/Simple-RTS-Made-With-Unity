using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public int maxResources = 500;
    int ResourcesQuantity;
    public int resourcesQuantity
    {
        get { return ResourcesQuantity; }
        set
        {
            ResourcesQuantity = value;
            if (ResourcesQuantity <= 0)
                Destroy(gameObject);
            transform.localScale = initialScale * ResourcesQuantity / maxResources;
        }
    }

    Vector3 initialScale;
    void Start()
    {
        initialScale = transform.localScale;
        resourcesQuantity = maxResources;        
    }

}
