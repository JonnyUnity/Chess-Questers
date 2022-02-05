using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private float MovementTime;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float ZoomScrollSpeed;
    [SerializeField] private float RotateSpeed;
    [SerializeField] private float RotationAmount;
    [SerializeField] private Vector3 ZoomAmount;
    [SerializeField] private Vector3 MouseZoomAmount;

    private MouseControls Controls;

    private Vector3 NewPosition;
    private Vector2 MousePosition;
    private Vector3 NewZoom;

    private Vector2 MoveDirection;
    private float MouseZoom;

    private Quaternion NewRotation;

    private Coroutine RotationCoroutine;
    private bool IsRotating;
    private bool IsZooming;

    [SerializeField]
    private Transform CameraTransform;

    private Transform CreatureTransform;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;

        Controls = new MouseControls();

        Controls.Battle.MousePan.performed += ctx => MousePosition = ctx.ReadValue<Vector2>();
        Controls.Battle.MousePan.canceled += ctx => MousePosition = Vector2.zero;

        Controls.Battle.KeyPan.performed += ctx => MoveDirection = ctx.ReadValue<Vector2>();
        Controls.Battle.KeyPan.canceled += ctx => MoveDirection = Vector2.zero;

        Controls.Battle.RotateClockwise.performed += ctx => RotateClockwise();
        Controls.Battle.RotateClockwise.canceled += ctx => StopRotating();

        Controls.Battle.RotateAnticlockwise.performed += ctx => RotateAnticlockwise();
        Controls.Battle.RotateAnticlockwise.canceled += ctx => StopRotating();

        Controls.Battle.KeyZoomIn.performed += ctx => ZoomIn();
        Controls.Battle.KeyZoomIn.canceled += ctx => StopZooming();

        Controls.Battle.KeyZoomOut.performed += ctx => ZoomOut();
        Controls.Battle.KeyZoomOut.canceled += ctx => StopZooming();

        Controls.Battle.Zoom.performed += ctx => MouseZoom = ctx.ReadValue<float>();
        Controls.Battle.Zoom.canceled += ctx => MouseZoom = 0f;

        Controls.Battle.RecenterOnActiveCharacter.performed += ctx => RecenterOnActiveCharacter();

        Controls.Enable();
    }

    private void RecenterOnActiveCharacter()
    {
        if (CreatureTransform != null)
        {
            // set default zoom, rotation etc...
            //StartCoroutine(MoveToCharacterCoroutine());
            _transform.position = CreatureTransform.position;
        }

    }

    private IEnumerator MoveToCharacterCoroutine()
    {
        NewPosition = _transform.position;
        var t = 0f;
        var cameraMovementDuration = 1f;

        while (t < 1)
        {
            // variable
            t += Time.deltaTime / cameraMovementDuration;
            _transform.position = Vector3.Lerp(NewPosition, CreatureTransform.position, t);

            yield return null;
        }

       // NewPosition = _transform.position;
        
    }


    private void Start()
    {
        NewPosition = _transform.position;
        NewRotation = _transform.rotation;
        NewZoom = CameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (MoveDirection != Vector2.zero)
        {
            KeyPan(MoveDirection);
        }
        if (MouseZoom != 0)
        {
            MouseZoomScreen(MouseZoom);
        }

    }


    private void RotateClockwise()
    {
        StartRotating(1f);
    }


    private void RotateAnticlockwise()
    {
        StartRotating(-1f);
    }


    private void StartRotating(float rotateDirection)
    {
        IsRotating = true;
        NewRotation = _transform.rotation;
        StartCoroutine(RotateScreen(rotateDirection));
    }

    private void StopRotating()
    {
        if (IsRotating)
        {
            IsRotating = false;
        }
    }


    private IEnumerator RotateScreen(float rotateDirection)
    {
        while (IsRotating)
        {
            Debug.Log(NewRotation);
            NewRotation *= Quaternion.Euler(rotateDirection * RotationAmount * Vector3.up);
            _transform.rotation = Quaternion.Lerp(_transform.rotation, NewRotation, 1/(Time.deltaTime * MovementTime));
            yield return null;
        }
    }


    private void ZoomIn()
    {
        StartZooming(ZoomAmount);
    }

    private void ZoomOut()
    {
        StartZooming(-ZoomAmount);
    }

    private void StartZooming(Vector3 zoom)
    {
        IsZooming = true;
        NewZoom = CameraTransform.localPosition;
        StartCoroutine(ZoomScreen(zoom));
    }

    private void StopZooming()
    {
        if (IsZooming)
        {
            IsZooming = false;
        }
    }


    private void MouseZoomScreen(float increment)
    {

        NewZoom += (MouseZoomAmount * increment);
        CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, NewZoom, 1/(Time.deltaTime * ZoomScrollSpeed));

    }

    private IEnumerator ZoomScreen(Vector3 zoom)
    {
        while (IsZooming)
        {
            Debug.Log(CameraTransform.localPosition);
            NewZoom += zoom;
            CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, NewZoom, 1/(Time.deltaTime * MovementTime));
            yield return null;
        }
    }


    private void KeyPan(Vector2 direction)
    {
        NewPosition = _transform.position;

        if (direction.x > 0)
        {
            NewPosition += _transform.right * MoveSpeed;
        }
        else if (direction.x < 0)
        {
            NewPosition += _transform.right * -MoveSpeed;
        }
        if (direction.y > 0)
        {
            NewPosition += _transform.forward * MoveSpeed;
        }
        else if (direction.y < 0)
        {
            NewPosition += _transform.forward * -MoveSpeed;
        }

        _transform.position = Vector3.Lerp(_transform.position, NewPosition, 1/(Time.deltaTime * MovementTime));
    }


    private void MousePan(Vector2 position)
    {

        Vector2 direction = PanDirection(position.x, position.y);
        NewPosition += (Vector3)direction * MoveSpeed;

        transform.position = Vector3.Lerp(_transform.position, NewPosition, Time.deltaTime * MovementTime);

    }

    private Vector2 PanDirection(float x, float y)
    {
        Vector2 direction = Vector2.zero;

        if (y >= Screen.height * 0.95f)
        {
            direction.y += 1;
        }
        if (y <= Screen.height * 0.05f)
        {
            direction.y -= 1;
        }
        if (x >= Screen.width * 0.95f)
        {
            direction.x += 1;
        }
        if (x <= Screen.width * 0.05f)
        {
            direction.x -= 1;
        }

        return direction;
    }


    public void LookAtCreature(Transform creatureTransform)
    {
        CreatureTransform = creatureTransform;

        //RecenterOnActiveCharacter();
        _transform.position = creatureTransform.position;
    }

    public void SwapToCharacter(Transform creatureTransform)
    {
        CreatureTransform = creatureTransform;

        StartCoroutine(MoveToCharacterCoroutine());
    }


}
