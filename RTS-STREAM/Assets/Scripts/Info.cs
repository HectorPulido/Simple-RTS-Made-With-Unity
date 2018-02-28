using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    Text text;

	void Start ()
    {
        text = GetComponent<Text>();
        InfoChanged();

    }
    public void InfoChanged()
    {
        var resources = ResourceManager.singleton.resources.ToString();
        var maxTroops = CivilizationMetrics.singleton.maxTroops.ToString();
        var troops = CivilizationMetrics.singleton.troops.ToString();

        text.text = string.Format("Resources: {0} \nTroops: {1}/{2}", resources, troops, maxTroops);
    }

}
