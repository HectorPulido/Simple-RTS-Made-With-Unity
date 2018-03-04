using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAttack : CombatScript
{
    public int Damage;
    protected override void Attack()
    {
        transform.LookAt(target.transform.position);
        target.health -= Damage;
        target.CheckHealth();
    }
}
