using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player m_player;

    private World m_world;

    public Camera m_camera;

    // Start is called before the first frame update
    void Start()
    {
        // there should be only one Player in the game.
        m_player = gameObject.GetComponent<Player>();

        // there should be only one World in the game.
        m_world = gameObject.GetComponent<World>();

        // there should be only one World in the game.
        m_camera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            m_player.Place(m_player.position.x, m_player.position.y - 1);
        }
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            m_player.Place(m_player.position.x, m_player.position.y + 1);
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            m_player.Place(m_player.position.x - 1, m_player.position.y);
        }
        if (Input.GetKeyUp(KeyCode.Keypad6))
        {
            m_player.Place(m_player.position.x + 1, m_player.position.y);
        }
        m_camera.transform.position = new Vector3(m_player.position.x, 10, m_player.position.y);
        m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, m_player.position.y));
    }
}
