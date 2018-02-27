using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovileUnit : MonoBehaviour
{
    public CraftableUnit craftableUnit;
    public bool isEnabled;

    Renderer r;
    Vector3 target;
    NavMeshAgent nv;

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
        nv.SetDestination(target);

        if (Input.GetMouseButtonUp(0) && isEnabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out rh))
                if (rh.collider != null)
                    if (rh.collider.CompareTag("Terrain"))
                        target = rh.point;
        }

        if (Input.GetMouseButtonUp(0))
        {           
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Screen.height - pos.y;

            isEnabled = UnitSelecter.rect.Contains(pos, true);
        }
    }

}
