using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClick : MonoBehaviour
{
    public MyEvent WhenMouseDown;

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        WhenMouseDown.Invoke();
    }
}
