using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigController : MonoBehaviour
{
    [SerializeField] private float cameraHeight = 1.75f;
    [SerializeField] private float cameraMaxDistance = 25f;
    [SerializeField] private float cameraSpeed = 1f;
    [SerializeField] private float cameraZoomHeightInterval = 0.25f;

    private float currentHeight;
    private float currentPan;
    private float currentTilt = 10f;
    private float currentDistance = 5f;

    Camera mainCamera;
    public GameObject playerObject;
    public Transform tilt;

    void Start()
    {
        mainCamera = Camera.main;
        currentHeight = cameraHeight;

        transform.position = playerObject.transform.position + Vector3.up * currentHeight;
        transform.rotation = playerObject.transform.rotation;

        tilt.eulerAngles = new Vector3( currentTilt, transform.eulerAngles.y, transform.eulerAngles.z );
        mainCamera.transform.position += tilt.forward * -currentDistance;
    }

    void Update()
    {
        ZoomCamera();

        if( Input.GetMouseButton(1) )
        {
            currentTilt += Input.GetAxis( "Mouse Y" ) * cameraSpeed;
        }
    }

    void LateUpdate()
    {
        MoveCamera();
    }

    void ZoomCamera()
    {
        float delta = cameraSpeed * Input.mouseScrollDelta.y * cameraSpeed;
        currentDistance = Mathf.Clamp( currentDistance + delta, 0f, cameraMaxDistance );

        if( delta > 0  && currentDistance < cameraMaxDistance )
        {
            currentHeight += cameraZoomHeightInterval;
        }
        else if ( delta < 0  && currentDistance > 0 )
        {
            currentHeight -= cameraZoomHeightInterval;
        }
    }

    void MoveCamera()
    {
        currentPan = playerObject.transform.eulerAngles.y;

        transform.position = playerObject.transform.position + Vector3.up * currentHeight;
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, currentPan, transform.eulerAngles.z );
        tilt.eulerAngles = new Vector3( currentTilt, tilt.eulerAngles.y, tilt.eulerAngles.z );

        mainCamera.transform.position = transform.position + tilt.forward * -currentDistance;
    }
}
