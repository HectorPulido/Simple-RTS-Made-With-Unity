using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Factory : MonoBehaviour
{
    public RtsEntity[] units;
    public float craftTime = 5;
    public Vector2 instanceRadius;
    RtsEntity entity;

    Queue<RtsEntity> unitsQueue = new Queue<RtsEntity>();

    void Start()
    {
        entity = GetComponent<RtsEntity>();
        InvokeRepeating("InstantiateUnit", craftTime, craftTime);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, instanceRadius.x);
        Gizmos.DrawWireSphere(transform.position, instanceRadius.y);
    }
    void OnMouseDown()
    {
        if (!enabled)
            return;
        if (!entity.isSelectable)
            return;
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        MenuLayout.singleton.DestroyAllChildren();
        for (int i = 0; i < units.Length; i++)
        {
            int u = i;
            MenuLayout.singleton.AddChildren(
                units[u].preview,
                units[u].price.ToString(),
                () =>
                {
                    if (units[u].price > CivilizationMetrics.singleton[entity.faction].resources)
                        return;
                    if (CivilizationMetrics.singleton[entity.faction].troops >= CivilizationMetrics.singleton[entity.faction].maxTroops)
                        return;
                    CivilizationMetrics.singleton[entity.faction].resources -= units[u].price;
                    AddUnitToQueue(u);
                }
                );
        }
    }
    public void InstantiateUnit()
    {
        if (unitsQueue.Count <= 0)
            return;
        if (CivilizationMetrics.singleton[entity.faction].troops >= CivilizationMetrics.singleton[entity.faction].maxTroops)
            return;

        CivilizationMetrics.singleton[entity.faction].troops++;
        var go = unitsQueue.Dequeue();
        var pos = RandomInsideDonut(instanceRadius);
        Instantiate(go,
            new Vector3(pos.x + transform.position.x, 0, pos.y + transform.position.z),
            go.transform.rotation);
    }
    public void AddUnitToQueue(int unit)
    {
        unitsQueue.Enqueue(units[unit]);
    }

    public static Vector2 RandomInsideDonut(Vector2 donutRadius)
    {
        var p = Random.Range(donutRadius.x, donutRadius.y);
        var a = Random.Range(0, 360);

        return new Vector2(Mathf.Sin(a * Mathf.Deg2Rad), Mathf.Cos(a * Mathf.Deg2Rad)) * p;

    }

}
