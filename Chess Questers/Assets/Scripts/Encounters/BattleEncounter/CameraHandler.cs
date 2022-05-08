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

    [SerializeField] private Vector3 MinZoom;
    [SerializeField] private Vector3 MaxZoom;

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
    private Vector3 CreaturePosition;

    private Transform[] _obstructions;
    private int _oldHitsNumber;

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
        //if (CreatureTransform != null)
        //{
        //    // set default zoom, rotation etc...
        //    //StartCoroutine(MoveToCharacterCoroutine());
        //    _transform.position = CreatureTransform.position;
        //}

        if (CreaturePosition != null)
        {
            _transform.position = CreaturePosition;
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
            _transform.position = Vector3.Lerp(NewPosition, CreaturePosition, t);

            yield return null;
        }

       // NewPosition = _transform.position;
       
        
    }

    public IEnumerator SwapToCharacterCoroutine(Vector3 creaturePosition)
    {
        CreaturePosition = creaturePosition;
        NewPosition = _transform.position;

        var t = 0f;
        var cameraMovementDuration = 1f;

        while (t < 1)
        {
            // variable
            t += Time.deltaTime / cameraMovementDuration;
            _transform.position = Vector3.Lerp(NewPosition, CreaturePosition, t);

            yield return null;
        }

    }


    private void MoveToCharacter()
    {
        NewPosition = _transform.position;

        var t = 0f;
        var cameraMovementDuration = 1f;

        while (t < 1)
        {
            // variable
            t += Time.deltaTime / cameraMovementDuration;
            _transform.position = Vector3.Lerp(NewPosition, CreaturePosition, t);
        }
    }


    private void Start()
    {
        NewPosition = _transform.position;
        NewRotation = _transform.rotation;
        NewZoom = CameraTransform.localPosition;
        _oldHitsNumber = 0;



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


    private void FixedUpdate()
    {
        int layerMask = 1 << 7;

        RaycastHit hit;
        if (Physics.Raycast(_transform.position, CameraTransform.position, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(_transform.position, CameraTransform.position * hit.distance, Color.yellow);
            Debug.Log("Hit something!" + hit);
        }

    }


    //private void LateUpdate()
    //{
    //    int layerNumber = LayerMask.NameToLayer("Environment");
    //    int layerMask = 1 << layerNumber;

    //    RaycastHit[] hits = Physics.RaycastAll(_transform.position, CameraTransform.position - _transform.position, Mathf.Infinity, layerMask);
    //    if (hits.Length > 0)
    //    {

    //        int newHits = hits.Length - _oldHitsNumber;

    //        if (_obstructions != null && _obstructions.Length > 0 && newHits < 0)
    //        {
    //            // Repaint all the previous obstructions. Because some of the stuff might be not blocking anymore
    //            for (int i = 0; i < _obstructions.Length; i++)
    //            {
    //                _obstructions[i].gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    //            }
    //        }
    //        _obstructions = new Transform[hits.Length];
    //        // Hide the current obstructions
    //        for (int i = 0; i < hits.Length; i++)
    //        {


    //            Transform obstruction = hits[i].transform;
    //            Debug.Log(obstruction);

    //            LODGroup lodGroup = obstruction.gameObject.GetComponent<LODGroup>();
    //            if (lodGroup == null)
    //            {
    //                obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    //            }
    //            else
    //            {
    //                Transform lodTransform = lodGroup.transform;
    //                foreach (Transform child in lodTransform)
    //                {
    //                    var renderer = child.GetComponent<MeshRenderer>();
    //                    if (renderer != null && renderer.isVisible)
    //                    {
    //                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    //                    }                        
    //                }
    //            }
    //            _obstructions[i] = obstruction;
    //        }
    //        _oldHitsNumber = hits.Length;

    //    }
    //    else
    //    {
    //        // Mean that no more stuff is blocking the view and sometimes all the stuff is not blocking as the same time
    //        if (_obstructions != null && _obstructions.Length > 0)
    //        {
    //            for (int i = 0; i < _obstructions.Length; i++)
    //            {
    //                Transform obstruction = _obstructions[i].transform;
    //                LODGroup lodGroup = obstruction.gameObject.GetComponent<LODGroup>();
    //                if (lodGroup == null)
    //                {
    //                    _obstructions[i].gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    //                }
    //                else
    //                {
    //                    Transform lodTransform = lodGroup.transform;
    //                    foreach (Transform child in lodTransform)
    //                    {
    //                        var renderer = child.GetComponent<MeshRenderer>();
    //                        if (renderer != null && renderer.isVisible)
    //                        {
    //                            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    //                        }                            
    //                    }
    //                }

    //            }
    //            _oldHitsNumber = 0;
    //            _obstructions = null;
    //        }
    //    }


    //}


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
        //Debug.Log("Before: " + NewZoom);

        NewZoom += MouseZoomAmount * increment;

        if (NewZoom.sqrMagnitude > MaxZoom.sqrMagnitude)
        {
            NewZoom = MaxZoom;
        }

        if (NewZoom.sqrMagnitude < MinZoom.sqrMagnitude)
        {
            NewZoom = MinZoom;
        }

        //if (Vector3.Distance(NewZoom, MaxZoom) > 0)
        //{
        //    NewZoom = MaxZoom;
        //}
        //if (Vector3.Distance(NewZoom, MinZoom) < 0)
        //{
        //    NewZoom = MinZoom;
        //}
        
        //Debug.Log("After: " + NewZoom);

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
        //Debug.Log(NewPosition);
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

        if (NewPosition.x < GameGrid.Instance.Bottom)
        {
            NewPosition.x = GameGrid.Instance.Bottom;
        }
        else if (NewPosition.x > GameGrid.Instance.Top)
        {
            NewPosition.x = GameGrid.Instance.Top;
        }
        if (NewPosition.z < GameGrid.Instance.Left)
        {
            NewPosition.z = GameGrid.Instance.Left;
        }
        else if (NewPosition.z > GameGrid.Instance.Right)
        {
            NewPosition.z = GameGrid.Instance.Right;
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

    //public void SwapToCharacter(Transform creatureTransform)
    //{
    //    CreatureTransform = creatureTransform;

    //    StartCoroutine(MoveToCharacterCoroutine());
    //}

    public void SwapToCharacter(Vector3 creaturePosition)
    {
        CreaturePosition = creaturePosition;
        StartCoroutine(MoveToCharacterCoroutine());
        //MoveToCharacter();
    }


}
