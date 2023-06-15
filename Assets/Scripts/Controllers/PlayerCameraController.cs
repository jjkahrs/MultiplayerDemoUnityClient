using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerObject;

    [SerializeField] private float rotationSpeed;

    [SerializeField] private CinemachineFreeLook freeLook;
    private string xInputAxisName;
    private string yInputAxisName;
    private GameMaster _GM;

    void Awake()
    {
        _GM = GameObject.FindAnyObjectByType<GameMaster>();
    }

    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisOverride;
        xInputAxisName = freeLook.m_XAxis.m_InputAxisName;
        yInputAxisName = freeLook.m_YAxis.m_InputAxisName;
    }

    public float GetAxisOverride( string axisName )
    {
        if( "Mouse X" == axisName )
        {
            return Input.GetMouseButton( 1 ) ? UnityEngine.Input.GetAxis( "Mouse X" ) : 0;
        }

        return UnityEngine.Input.GetAxis( axisName );
    }

    // Update is called once per frame
    void Update()
    {
        bool isFreelook = Input.GetMouseButton( 1 );
        freeLook.m_XAxis.m_InputAxisName = isFreelook ? xInputAxisName : "";
        freeLook.m_YAxis.m_InputAxisName = isFreelook ? yInputAxisName : "";
        Cursor.visible = !isFreelook;
        
        // Rotate the orientation
        Vector3 viewDirection = player.transform.position - new Vector3( transform.position.x, player.transform.position.y, transform.position.z );
        //orientation.forward = viewDirection.normalized;
        _GM.GetGameStore().cameraVector = viewDirection.normalized;

        // Rotate the player object
        // float horizontalInput = Input.GetAxis( "Horizontal" );
        // float verticalInput = Input.GetAxis( "Vertical" );
        // Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // if( inputDirection != Vector3.zero )
        // {
        //     playerObject.forward = Vector3.Slerp( playerObject.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed );
        // }
    }
}
