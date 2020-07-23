using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera m_camera;

    private World m_world;

    public Light m_playerLight;

    // Start is called before the first frame update
    void Start()
    {
        m_world = gameObject.AddComponent(typeof(World)) as World;

        m_playerLight.range = 200;
        m_playerLight.intensity = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            Vector2 rndPos = m_world.PlacePlayer();
            m_camera.transform.position = new Vector3(rndPos.x, 2, rndPos.y);
            m_playerLight.transform.position = new Vector3(rndPos.x, 2, rndPos.y);
        }
    }
}
