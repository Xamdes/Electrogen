using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float zeroF = 0.0f;
    [SerializeField]
    public LaserController currentLaserSelected;
    [SerializeField]
    private LaserController lastLaserSelected;
    [SerializeField]
    bool pointerHit = false;

    private float rotationSpeed = 100.0f;
    private float panSpeed = 5.0f;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        InputControls();

        var moveX = GetAxisSpeed("Horizontal", panSpeed);
        var moveZ = GetAxisSpeed("Vertical", panSpeed);
        
        transform.Translate(moveX, zeroF, moveZ);        
    }

    float GetDeltaSpeed(float a)
    {
        return (Time.deltaTime * a);
    }

    float GetAxisSpeed(string type, float speed)
    {
        return Input.GetAxis(type) * GetDeltaSpeed(speed);
    }

    void SelectObjects()
    {
        Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // the object identified by hit.transform was clicked
            // do whatever you want
            currentLaserSelected = hit.collider.GetComponent<LaserController>();

            if (currentLaserSelected == null)
            {
                if (lastLaserSelected != null)
                {
                    lastLaserSelected.Selected(false);
                    lastLaserSelected = null;
                }
                pointerHit = false;
            }
            if (lastLaserSelected == null && currentLaserSelected != null)
            {
                currentLaserSelected.Selected(true);
                lastLaserSelected = currentLaserSelected;
                pointerHit = true;
            }
            if (currentLaserSelected != lastLaserSelected)
            {
                currentLaserSelected.Selected(true);
                lastLaserSelected.Selected(false);
                lastLaserSelected = currentLaserSelected;
                pointerHit = true;
            }
        }
        else
        {
            if (currentLaserSelected != null)
            {
                currentLaserSelected.Selected(false);
            }
            currentLaserSelected = null;
            lastLaserSelected = null;
            pointerHit = false;
        }
    }

    void RotatePlayerView(Transform t)
    {
        var x = GetAxisSpeed("Mouse X", rotationSpeed);
        var y = GetAxisSpeed("Mouse Y", -1* rotationSpeed);
        t.Rotate(y, x, zeroF);
        t.rotation = Quaternion.Euler(t.rotation.eulerAngles.x, t.rotation.eulerAngles.y, zeroF);
    }

    void InputControls()
    {

        if (Input.GetMouseButtonDown(0))
        {
            SelectObjects();
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            RotatePlayerView(transform);
        }

        if (Input.GetKeyDown(KeyCode.Q) && pointerHit)
        {
            currentLaserSelected.transform.Rotate(0, -90.0f, 0);
        }

        if (Input.GetKeyDown(KeyCode.E) && pointerHit)
        {
            currentLaserSelected.transform.Rotate(0, 90.0f, 0);
        }
    }
}
