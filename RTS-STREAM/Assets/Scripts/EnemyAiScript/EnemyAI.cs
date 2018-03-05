using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    CivilizationMetrics metrics;
    RtsEntity entity;
    List<Factory> factories = new List<Factory>();
    List<Robot> robots = new List<Robot>();
    List<CombatScript> combatUnits = new List<CombatScript>();
    List<Building> enemyBuild = new List<Building>();

    // Use this for initialization
    void Start()
    {
        entity = GetComponent<RtsEntity>();
        metrics = CivilizationMetrics.singleton[entity.faction];
        StartCoroutine(AI());
    }

    IEnumerator AI()
    {
        factories.Add(GetComponent<Factory>());

        yield return GenerateRobots(factories[0], 0, 3);
        GetRobots();
        yield return SetResourcesRobots(robots.GetRange(0,2).ToArray()
            , GameObject.FindObjectOfType<Resource>());
        yield return CreateBuild(robots[2], 1);
        yield return SetResourcesRobots(new Robot[] { robots[2] }
            , GameObject.FindObjectOfType<Resource>());
        yield return GenerateRobots(factories[0], 1, 4);
        GetEnemyBuild();
        GetBattlers();
        yield return Battle();
    }
    IEnumerator SetResourcesRobots(Robot[] robots, Resource res)
    {
        for (int i = 0; i < robots.Length; i++)
        {
            robots[i].resource = res;
            robots[i].building = null;
        }
        yield return new WaitForSeconds(0);
    }
    IEnumerator GenerateRobots(Factory factory, int id,int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (factory.units[id].price > metrics.resources)
                break;
            factory.AddUnitToQueue(id);
            metrics.resources -= factory.units[id].price;
        }
        yield return new WaitForSeconds(count * factories[0].craftTime + 1);

    }
    IEnumerator CreateBuild(Robot builder, int id)
    {
        if (builder.builds[id].entity.price <= metrics.resources)
        {
            var go = Instantiate(builder.builds[id]);
            bool sw = true;
            while (sw)
            {
                sw = false;
                var pos = Factory.RandomInsideDonut(new Vector2(1, 50)) 
                    + new Vector2(builder.transform.position.x, builder.transform.position.z);

                RaycastHit rh;
                if (Physics.Raycast(new Vector3(pos.x , 100, pos.y), Vector3.down, out rh))
                {
                    if (rh.collider.CompareTag("Terrain"))
                    {
                        go.transform.position = new Vector3(pos.x, 0, pos.y);
                        if (rh.normal != Vector3.up || go.collidersCount <= 1)
                        {
                            sw = true;
                        }
                    }
                }
            }
            metrics.resources -= builder.builds[id].entity.price;
            go.onCrafted.Invoke();
            go.SendMessage("SetBuild");
            yield return new WaitForSeconds(1);
            builder.building = go;
            yield return new WaitForSeconds(go.buildTime);
            builder.building = null;
        }

    }
    IEnumerator Battle()
    {
        foreach (var item in enemyBuild)
        {
            for (int i  = 0; i < combatUnits.Count; i ++)
            {
                combatUnits[i].target = item.entity;
            }
            while (item != null)
            {
                yield return new WaitForSeconds(5);
            }
        }
    }
    void GetRobots()
    {
        var r = GameObject.FindObjectsOfType<Robot>();
        for (int i = 0; i < r.Length; i++)
        {
            if (r[i].movileEntity.entity.faction == entity.faction)
                if (!robots.Contains(r[i]))
                    robots.Add(r[i]);
        }
    }
    void GetBattlers()
    {
        var r = GameObject.FindObjectsOfType<CombatScript>();
        for (int i = 0; i < r.Length; i++)
        {
            if (r[i].entity.faction == entity.faction)
                if (!combatUnits.Contains(r[i]))
                    combatUnits.Add(r[i]);
        }       
    }
    void GetEnemyBuild()
    {
        var enemy = GameObject.FindObjectsOfType<Building>();
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].entity.faction != entity.faction)
                if (!enemyBuild.Contains(enemy[i]))
                    enemyBuild.Add(enemy[i]);
        }
    }

}
