using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAirplane : CombatScript
{

    public Proyectile canonBallPrefab;
    public Transform canon;
    protected override void Attack()
    {
        transform.LookAt(target.transform.position);
        canon.LookAt(target.transform.position + Vector3.up * 3.324f);
        var cb = Instantiate(canonBallPrefab, canon.transform.position, canon.transform.rotation);
        cb.faction = entity.faction;
        cb.proyectileRigidbody.velocity = cb.transform.forward * 10;
    }


}

