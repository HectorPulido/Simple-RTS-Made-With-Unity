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
	}
    public void ResourcesChanged(int resources)
    {
        text.text = "Resources: " + resources.ToString();
    }
}
