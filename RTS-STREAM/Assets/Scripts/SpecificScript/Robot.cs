using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public Building[] builds;
    void OnMouseDown()
    {
        MenuLayout.singleton.DestroyAllChildren();
        for (int i = 0; i < builds.Length; i++)
        {
            int u = i;
            MenuLayout.singleton.AddChildren(
                builds[u].craftableUnit.preview,
                builds[u].craftableUnit.price.ToString(),
                () =>
                {
                    print("Construct build " + builds[u].name);
                }
                );
        }
    }
}
