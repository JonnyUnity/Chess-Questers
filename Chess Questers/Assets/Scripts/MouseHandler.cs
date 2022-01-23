using UnityEngine;
using Cinemachine;

public class MouseHandler : MonoBehaviour
{

    [SerializeField]
    private float PanSpeed = 20f;
    [SerializeField]
    private float ZoomSpeed = 30f;
    [SerializeField]
    private float ZoomInMax = 40f;
    [SerializeField]
    private float ZoomOutMax = 90f;

    [SerializeField]
    private float MinOrthographicSize = 5f;
    [SerializeField]
    private float MaxOrthographicSize = 20f;

    [SerializeField]
    private float RotationSpeed = 30f;

    private CinemachineInputProvider InputProvider;
    private CinemachineVirtualCamera VirtualCamera;
    private Transform CameraTransform;

    private MouseControls Controls;
    private float rot;
    private Vector2 Pan;

    private void Awake()
    {
        InputProvider = GetComponent<CinemachineInputProvider>();
        VirtualCamera = GetComponent<CinemachineVirtualCamera>();
        CameraTransform = VirtualCamera.VirtualCameraGameObject.transform;

        Controls = new MouseControls();

        //Controls.Battle.Rotate.performed += ctx => rot = ctx.ReadValue<float>();
        //Controls.Battle.Rotate.canceled += ctx => rot = 0f;

        //Controls.BattleMouse.Pan.performed += ctx => Pan = ctx.ReadValue<Vector2>();
        //Controls.BattleMouse.Pan.performed += ctx => Pan = Vector2.zero;

        Controls.Enable();

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = InputProvider.GetAxisValue(0);
        float y = InputProvider.GetAxisValue(1);
        float z = InputProvider.GetAxisValue(2);

        if (x != 0 || y != 0)
        {
            PanScreen(x, y);
        }
        if (z != 0)
        {
            ZoomScreen(z);
        }


        //if (Pan != Vector2.zero)
        //{
        //    PanScreen(Pan);
        //}


        //if (rot != 0)
        //{
        //    Vector3 rotation = new Vector3(0, RotationSpeed * Time.deltaTime * rot, 0);
        //    CameraTransform.Rotate(rotation);
        //}

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

    public void PanScreen(float x, float y)
    {
        Vector2 direction = PanDirection(x, y);

        CameraTransform.position = Vector3.Lerp(CameraTransform.position, 
            CameraTransform.position + (Vector3)direction * PanSpeed, Time.deltaTime);

    }

    private void PanScreen(Vector2 keyDirection)
    {
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, CameraTransform.position + (Vector3)keyDirection * PanSpeed, Time.deltaTime);
    }

    public void ZoomScreen(float increment)
    {
        // for orthographic camera
        Debug.Log(increment);
        float size = VirtualCamera.m_Lens.OrthographicSize;
        float target = Mathf.Clamp(size + increment, MinOrthographicSize, MaxOrthographicSize);

        //VirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(size, target, ZoomSpeed * Time.deltaTime);
        VirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(size, target, ZoomSpeed * Time.deltaTime);


        // for perspective camera
        //float fov = VirtualCamera.m_Lens.FieldOfView;
        //float target = Mathf.Clamp(fov + increment, ZoomInMax, ZoomOutMax);

        //VirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(fov, target, ZoomSpeed * Time.deltaTime);
    }


}
