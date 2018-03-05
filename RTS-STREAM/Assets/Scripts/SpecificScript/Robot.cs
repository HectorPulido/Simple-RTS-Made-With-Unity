using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Robot : MonoBehaviour
{
    public Building[] builds;    
    public float craftRate;
    public float mineRate;
    public float range;

    [HideInInspector]
    public MovileEntity movileEntity;
    void Start()
    {
        movileEntity = GetComponent<MovileEntity>();
        InvokeRepeating("Craft", craftRate, craftRate);
        InvokeRepeating("Mine", mineRate, mineRate);
    }


    [HideInInspector]
    public Building building;
    void Craft()
    {
        if (building == null)
            return;
        if (Vector3.Distance(transform.position, building.transform.position) > range)
        {
            movileEntity.target = building.transform.position;
            return;
        }
        if (building.buildTime <= 0)
        {
            building = null;
            return;
        }
        transform.LookAt(building.transform.position);

        building.SendMessage("CraftPoint");
    }

    [HideInInspector]
    public Resource resource;
    void Mine()
    {

        if (resource == null)
            return;
        if (Vector3.Distance(transform.position, resource.transform.position) > range)
        {
            movileEntity.target = resource.transform.position;
            return;
        }
        transform.LookAt(resource.transform.position);

        resource.resourcesQuantity -= 10;
        CivilizationMetrics.singleton[movileEntity.entity.faction].resources += 100;

    }
    void MouseUp(RaycastHit rh)
    {
        if (!movileEntity.entity.isSelectable)
            return;
        if (rh.collider == null)
        {
            resource = null;
            building = null;
            return;
        }

        if (rh.collider.GetComponent<Resource>() != null)
        {
            var res = rh.collider.GetComponent<Resource>();
            building = null;
            resource = res;
        }
        else if (rh.collider.GetComponent<Building>() != null)
        {
            var build = rh.collider.GetComponent<Building>();
            if (build.entity.faction == movileEntity.entity.faction)
            {
                building = build;
                resource = null;
            }
        }
        else
        {
            building = null;
            resource = null;
        }
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (!movileEntity.entity.isSelectable)
            return;
        MenuLayout.singleton.DestroyAllChildren();
        for (int i = 0; i < builds.Length; i++)
        {
            int u = i;
            MenuLayout.singleton.AddChildren(
                builds[u].entity.preview,
                builds[u].entity.price.ToString(),
                () =>
                {
                    if (builds[u].entity.price > CivilizationMetrics.singleton[movileEntity.entity.faction].resources)
                        return;
                    var go = Instantiate(builds[u]);
                    BuildingCraft.singleton.CraftingBuilding = go;
                }
                );
        }
    }
}
