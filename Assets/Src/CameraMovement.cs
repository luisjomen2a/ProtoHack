using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Camera m_cam;
    bool m_isCameraMoving;

    Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        m_cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(0.1f, 0, 0));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(-0.1f, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftControl))
        { 
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(new Vector3(0, 0.1f, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(new Vector3(0, -0.1f, 0));
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(new Vector3(0, 0, 0.1f));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(new Vector3(0, 0, -0.1f));
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_isCameraMoving = true;
            position = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
            m_isCameraMoving = false;
        if (m_isCameraMoving)
        {
            Vector3 lastPosition = Input.mousePosition - position;
            transform.Rotate(new Vector3(1,0,0), -lastPosition.y);
            transform.Rotate(new Vector3(0,1,0), lastPosition.x);
            position = Input.mousePosition;
        }
    }

}
