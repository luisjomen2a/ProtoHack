using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera m_camera;

    public Player m_player;

    private World m_world;

    public Light m_playerLight;

    // Start is called before the first frame update
    void Start()
    {
        m_world = gameObject.AddComponent(typeof(World)) as World;
        m_player = gameObject.AddComponent(typeof(Player)) as Player;

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
            m_player.Place(rndPos.x, rndPos.y);
            m_camera.transform.position = new Vector3(rndPos.x, 10, rndPos.y - 2);
            m_camera.transform.LookAt(new Vector3(rndPos.x, 2, rndPos.y));
            m_playerLight.transform.position = new Vector3(rndPos.x, 2, rndPos.y);
        }
        // Player movement.
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            m_player.Place(m_player.position.x, m_player.position.y - 1);
            m_camera.transform.position = new Vector3(m_player.position.x, 10, m_player.position.y - 2);
            m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, m_player.position.y));
            m_playerLight.transform.position = new Vector3(m_player.position.x, 3, m_player.position.y);
        }
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            m_player.Place(m_player.position.x, m_player.position.y + 1);
            m_camera.transform.position = new Vector3(m_player.position.x, 10, m_player.position.y - 2);
            m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, m_player.position.y));
            m_playerLight.transform.position = new Vector3(m_player.position.x, 3, m_player.position.y);
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            m_player.Place(m_player.position.x - 1, m_player.position.y);
            m_camera.transform.position = new Vector3(m_player.position.x, 10, m_player.position.y - 2);
            m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, m_player.position.y));
            m_playerLight.transform.position = new Vector3(m_player.position.x, 3, m_player.position.y);
        }
        if (Input.GetKeyUp(KeyCode.Keypad6))
        {
            m_player.Place(m_player.position.x + 1, m_player.position.y);
            m_camera.transform.position = new Vector3(m_player.position.x, 10, m_player.position.y - 2);
            m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, m_player.position.y));
            m_playerLight.transform.position = new Vector3(m_player.position.x, 3, m_player.position.y);
        }
    }
}
