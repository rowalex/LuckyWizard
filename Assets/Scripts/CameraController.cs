using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    
    public float distance = 10f; 
    public float angle = 45f; 
    public float moveSpeed = 5f; 
    public float smoothspeed = 1f;

    private bool isgame = false;
    private Vector2 border;

    private Transform target = null;
    public Vector3 pos = Vector3.zero;


    [HideInInspector]
    public static CameraController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Instance of cameraControlller");
        }
    }

    public void StartGameCamera(Transform target)
    {
        isgame = true;
        border = GridManager.Instance.GetSize();
        this.target = target;
    }
    public void StopGameCamera()
    {
        isgame = false;
        this.target = null;
    }

    private Vector3 lastMousePosition;

    private void LateUpdate()
    {
        if (isgame && target)
        {
            float mw =  -Input.GetAxis("Mouse ScrollWheel") * 2;

            distance = Math.Clamp(distance + mw, 10, 20);

            if (Input.GetMouseButton(2)) //колесо
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                float moveX = -delta.x * moveSpeed;
                float moveZ = -delta.y * moveSpeed;

                target.position += new Vector3(moveX, 0, moveZ);
                target.position = new Vector3(Mathf.Clamp(target.position.x, 0 , border.x), 0, Mathf.Clamp(target.position.z, 0, border.y));
            }

            lastMousePosition = Input.mousePosition;

            UpdateCameraPosition();

        }
        else if (pos != Vector3.zero && target)
        {
            transform.position = Vector3.Lerp(transform.position, pos, smoothspeed * Time.deltaTime);
            transform.LookAt(target.position);
        }

    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(angle, 0, 0);
        pos = target.position - rotation * Vector3.forward * distance;

        transform.position = Vector3.Lerp(transform.position, pos, smoothspeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(angle, 0, 0);
    }

    public void SetCamera(Vector3 pos, Transform target)
    {
        StopGameCamera();
        this.pos = pos;
        this.target = target;
    }

}