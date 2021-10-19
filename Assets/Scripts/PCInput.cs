using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PCInput : MonoBehaviour
{
    public InputActionProperty Move;
    public InputActionProperty Look;

    public Transform RootTransform;
    public Transform CameraTransform;

    public Vector2 LookSpeed;
    public Vector2 MoveSpeed;
    public float Acceleration;

    Vector2 m_CurrentMoveDelta;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        var moveDelta = Move.action.ReadValue<Vector2>();

        m_CurrentMoveDelta = Vector2.MoveTowards( m_CurrentMoveDelta, moveDelta, Time.deltaTime * Acceleration );

        RootTransform.Translate( m_CurrentMoveDelta.x * MoveSpeed.x * Time.deltaTime, 0f, m_CurrentMoveDelta.y * MoveSpeed.y * Time.deltaTime, Space.Self );

        var lookDelta = Look.action.ReadValue<Vector2>();
        RootTransform.Rotate( 0f, lookDelta.x * LookSpeed.x, 0f, Space.Self );
        CameraTransform.Rotate( lookDelta.y * LookSpeed.y, 0f, 0f, Space.Self );
    }
}
