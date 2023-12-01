using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Movement
    public float borderMoveSpeed = 1.2f;
    public float screenOffset = .005f;
    public Vector2 positionXLimits;
    public Vector2 positionZLimits;
    // ZOOM
    public float zoomSpeed = 4f;
    public Vector2 zoomLimits;

    Camera myCam;
    private void Start()
    {
        myCam = GetComponent<Camera>();
    }

    void Update()
    {
        // Zoom code 
        var zoom = Input.GetAxis("Mouse ScrollWheel");
        myCam.orthographicSize -= zoom * zoomSpeed;
        myCam.orthographicSize = Mathf.Clamp(myCam.orthographicSize,
            zoomLimits.x, zoomLimits.y);


        // Camera movement per border
        Vector3 Speed = new();

        if (Input.mousePosition.x < Screen.width * screenOffset)
            Speed.x -= borderMoveSpeed;
        else if (Input.mousePosition.x > Screen.width - (Screen.width * screenOffset))
            Speed.x += borderMoveSpeed;

        if (Input.mousePosition.y < Screen.height * screenOffset)
            Speed.z -= borderMoveSpeed;
        else if (Input.mousePosition.y > Screen.height - (Screen.height * screenOffset))
            Speed.z += borderMoveSpeed;

        var tempPositions = transform.position;
        tempPositions += Speed * Time.deltaTime;

        tempPositions.x = Mathf.Clamp(tempPositions.x, positionXLimits.x, positionXLimits.y);
        tempPositions.z = Mathf.Clamp(tempPositions.z, positionZLimits.x, positionZLimits.y);

        transform.position = tempPositions;
    }
}