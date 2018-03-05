using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelecter : MonoBehaviour {


    Vector2 startPos;
    Vector2 endPos;

    public Texture SelectionTexture;
    public static Rect rect;

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                startPos = Input.mousePosition;
            else  // Else we must be in "drag" mode.
                endPos = Input.mousePosition;
        }
        else
        {
            endPos = startPos = Vector2.zero;
        }
    }

    void OnGUI()
    {
        if (startPos != Vector2.zero && endPos != Vector2.zero)
        {
            rect = new Rect(startPos.x, Screen.height - startPos.y,
                                endPos.x - startPos.x,
                                startPos.y - endPos.y);
            GUI.DrawTexture(rect, SelectionTexture);
        }
    }
}
