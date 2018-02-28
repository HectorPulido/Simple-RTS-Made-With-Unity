using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float cameraSpeed;

	void Update ()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        transform.position += new Vector3(h, 0, v) * cameraSpeed * Time.deltaTime;
		
	}
}
