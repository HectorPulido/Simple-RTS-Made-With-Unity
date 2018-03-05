using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovileEntity : MonoBehaviour
{
    public bool isSelected;
    public GameObject selectionSphere;
    Vector3 Target;
    public Vector3 target
    {
        get { return Target; }
        set {
            Target = value;
            nv.SetDestination(Target);
        }
    }

    NavMeshAgent nv;
    public RtsEntity entity;
    RaycastHit rh;

    void Start()
    {
        nv = GetComponent<NavMeshAgent>();
        entity = GetComponent<RtsEntity>();
        target = transform.position;
    }

    void Update()
    {
        if (!entity.isSelectable)
            return;
        if (isSelected)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out rh))
                {
                    SendMessage("MouseUp", rh, SendMessageOptions.DontRequireReceiver);

                    if (rh.collider != null)
                    {
                        if (rh.collider.CompareTag("Terrain"))
                        {
                            target = rh.point;
                        }
                    }                
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Screen.height - pos.y;

            isSelected = UnitSelecter.rect.Contains(pos, true);
            selectionSphere.SetActive(isSelected);
        }
    }
}
