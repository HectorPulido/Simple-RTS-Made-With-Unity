using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : CombatScript
{
    public Proyectile canonBallPrefab;
    public Transform canon;
    protected override void Attack()
    {
        transform.LookAt(target.transform.position);
        var cb = Instantiate(canonBallPrefab, canon.transform.position, canon.transform.rotation);
        cb.faction = entity.faction;
        cb.proyectileRigidbody.velocity = cb.transform.forward * 10;
    }

}
