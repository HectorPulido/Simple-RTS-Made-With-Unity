using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHealer : CombatScript
{
    public int cure;
    protected override void Attack()
    {
        if (target.faction != entity.faction)
        {
            target = null;
        }
        transform.LookAt(target.transform.position);
        target.health += cure;
        target.CheckHealth();
    }

}
