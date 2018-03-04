using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    Text text;
    [SerializeField]
    FactionType faction;

	void Start ()
    {
        text = GetComponent<Text>();
        InfoChanged();

    }
    public void InfoChanged()
    {
        var resources = CivilizationMetrics.singleton[faction].resources.ToString();
        var maxTroops = CivilizationMetrics.singleton[faction].maxTroops.ToString();
        var troops = CivilizationMetrics.singleton[faction].troops.ToString();

        text.text = string.Format("Resources: {0} \nTroops: {1}/{2}", resources, troops, maxTroops);
    }

}
