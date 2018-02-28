using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum States
{
    moving,
    attacking,
    crafting,
    extracting
}

public class MovileUnit : MonoBehaviour
{
    public CraftableUnit craftableUnit;
    public bool isEnabled;
    public States states;


    public Vector3 target;    
    public Transform targetTranform;
    public NavMeshAgent nv;
    Renderer r;

    void Start()
    {
        r = GetComponentInChildren<Renderer>();
        nv = GetComponent<NavMeshAgent>();
        target = transform.position;
    }

    RaycastHit rh;
    void Update()
    {
        r.material.color = isEnabled ? Color.red : Color.blue;
        if (states == States.moving)
        {
            nv.SetDestination(target);
        }
        else if (states == States.attacking)
        {
            nv.SetDestination(targetTranform.position);
        }
        else if (states == States.crafting || states == States.extracting)
        {
            nv.SetDestination(target);
        }


        if (Input.GetMouseButtonUp(0) && isEnabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out rh))
                if (rh.collider != null)
                {
                    if (rh.collider.CompareTag("Terrain"))
                    {
                        target = rh.point;
                        states = States.moving;
                    }
                    else if (rh.collider.GetComponent<Building>() != null)
                    {
                        target = rh.collider.transform.position;
                        targetTranform = rh.collider.transform;
                        states = States.crafting;
                    }
                    else if (rh.collider.GetComponent<Resource>() != null)
                    {
                        target = rh.collider.transform.position;
                        targetTranform = rh.collider.transform;
                        states = States.extracting;
                    }
                }
        }

        if (Input.GetMouseButtonUp(0))
        {           
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Screen.height - pos.y;

            isEnabled = UnitSelecter.rect.Contains(pos, true);
        }
    }

}
