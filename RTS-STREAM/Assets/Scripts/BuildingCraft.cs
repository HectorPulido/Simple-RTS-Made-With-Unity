using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingCraft : MonoBehaviour
{
    [SerializeField]
    LayerMask terrainMask;

    public Building CraftingBuilding;
    RaycastHit rh;

    public static BuildingCraft singleton;

    void Start()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }

    void Update ()
    {
        if (CraftingBuilding == null)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out rh, terrainMask))
        {
            if (rh.collider.CompareTag("Terrain"))
            {
                CraftingBuilding.transform.position = rh.point;
                CraftingBuilding.canCraft = rh.normal == Vector3.up;
                if (CraftingBuilding.canCraft)
                {
                    CraftingBuilding.canCraft = CraftingBuilding.collidersCount <= 1;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            if (CraftingBuilding.canCraft == true)
            {
                ResourceManager.singleton.resources -= CraftingBuilding.craftableUnit.price;
                CraftingBuilding.onCrafted.Invoke();                
                CraftingBuilding = null;
            }
        }
    }
}
