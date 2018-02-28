using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public CraftableUnit craftableUnit;
    public float craftRadius;
    public MyEvent onCrafted;
    public MyEvent onCraftCompleted;

    bool _canCraft = false;
    public bool canCraft
    {
        get
        {
            return _canCraft;
        }
        set
        {
            _canCraft = value;
            buildRenderer.material.color = _canCraft ? Color.green : Color.red;
        }
    }
    public int collidersCount
    {
        get
        {
            var Sphere = Physics.SphereCastAll(transform.position, craftRadius, -Vector3.up);
            return Sphere.Length;
        }
    }
    public Renderer buildRenderer;

    public int buildTime = 10;

    void CraftPoint()
    {
        print("CRAFTING...");
        buildTime--;
        if (buildTime <= 0)
        {
            onCraftCompleted.Invoke();
            buildRenderer.material.color = Color.blue;
            print("CRAFT COMPLETED!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, craftRadius);
    }
}
