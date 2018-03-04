using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScript : MonoBehaviour
{
    public float range;
    public float cadence;
    public bool targetCanBeAlly;
    [HideInInspector]
    public RtsEntity entity;
    MovileEntity movileEntity;
    
    void Start ()
    {
        movileEntity = GetComponent<MovileEntity>();
        entity = GetComponent<RtsEntity>();
        InvokeRepeating("PrepareToAttack", cadence, cadence);
    }

    RaycastHit rh;
    void Update()
    {
        if (!entity.isSelectable)
            return;
        if (!movileEntity.isSelected)
            return;
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out rh))
            {
                 if (rh.collider != null)
                {
                    print(rh.collider.name);
                    if (rh.collider.GetComponent<RtsEntity>() != null)
                    {
                        var cs = rh.collider.GetComponent<RtsEntity>();
                        if (cs.faction != entity.faction || targetCanBeAlly)
                            target = cs;
                        else
                            target = null;
                    }
                    else
                    {
                        target = null;
                    }
                }
                else
                {
                    target = null;
                }
            }
        }
        if (target != null)
        {
            movileEntity.target = target.transform.position;
        }
    }
    protected RtsEntity target;
    void PrepareToAttack()
    {
        if (target == null)
            return;
        if (Vector3.Distance(transform.position, target.transform.position) > range)
            return;
        Attack();
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Proyectile>() != null)
        {
            var pro = col.GetComponent<Proyectile>();
            if (pro.faction != entity.faction)
            {
                entity.health -= pro.damage;
                entity.CheckHealth();
                Destroy(pro.gameObject);
            }
        }
    }
    protected virtual void Attack() { }

}
