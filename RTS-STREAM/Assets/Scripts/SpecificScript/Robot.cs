using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Robot : MonoBehaviour
{
    public Building[] builds;
    public MovileUnit mv;
    void Start()
    {
        mv = GetComponent<MovileUnit>();
        InvokeRepeating("AddCraftPoint", 1, 1);
        InvokeRepeating("Mine", 1, 1);
    }
    void AddCraftPoint()
    {
        if (mv.states != States.crafting)
            return;
        if (mv.targetTranform == null)
            return;
        
        if (Vector3.Distance(mv.targetTranform.position, transform.position) <= 10)
            mv.targetTranform.SendMessage("CraftPoint");
    }

    Resource resource;
    void Mine()
    {
        if (mv.states != States.extracting)
            return;
        if (mv.targetTranform == null)
            return;
        if (resource == null)
            resource = mv.targetTranform.GetComponent<Resource>();
        if (Vector3.Distance(mv.targetTranform.position, transform.position) <= 10)
        {
            mv.target = transform.position;
            resource.resourcesQuantity -= 100;
            ResourceManager.singleton.resources += 100;
        }

    }
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        MenuLayout.singleton.DestroyAllChildren();
        for (int i = 0; i < builds.Length; i++)
        {
            int u = i;
            MenuLayout.singleton.AddChildren(
                builds[u].craftableUnit.preview,
                builds[u].craftableUnit.price.ToString(),
                () =>
                {
                    if (builds[u].craftableUnit.price > ResourceManager.singleton.resources)
                        return;
                    var go = Instantiate(builds[u]);
                    BuildingCraft.singleton.CraftingBuilding = go;
                }
                );
        }
    }
}
