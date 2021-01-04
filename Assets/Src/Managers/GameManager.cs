using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera m_camera;

    public Player m_player;

    private World m_world;

    public Light m_playerLight;

    // Start is called before the first frame update
    void Start()
    {
        m_world = gameObject.AddComponent(typeof(World)) as World;
        m_player = gameObject.AddComponent(typeof(Player)) as Player;
        m_camera = gameObject.AddComponent(typeof(Camera)) as Camera;
        m_camera.enabled = false;

        m_player.Generate();
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
            m_player.Place(rndPos.x, rndPos.y);
            m_camera.transform.position = new Vector3(rndPos.x, 15, rndPos.y);
            m_camera.transform.LookAt(new Vector3(rndPos.x, 2, rndPos.y));
            m_camera.enabled = true;
        }
        // Get a FPS camera
        // TODO : implement a switch fonction to go to overview aswell.
        if (Input.GetKeyUp(KeyCode.F1))
        {
            m_camera.enabled = false;

            // We try to find the FPS camera that is attached to the scene.
            Camera[] allCams = FindObjectsOfType<Camera>();
            Camera FPSCamera = null;

            foreach (Camera cam in allCams)
                if (cam.name == "FPSCamera")
                    FPSCamera = cam;

            if (FPSCamera)
            {
                Debug.Log("Ok mah man, here are the coords " + m_player.transform.position);
                FPSCamera.transform.position = new Vector3(m_player.position.x, m_player.playerHeight, m_player.position.y);
            }
        }
    }
}
