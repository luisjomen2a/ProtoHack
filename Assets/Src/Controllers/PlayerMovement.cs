using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player m_player;

    private World m_world;

    private Camera m_camera;

    private bool m_hasPressed = false;

    private int m_abs = 0;
    private int m_ord = 0;

    private int m_cameraHeight = 15;

    // Start is called before the first frame update
    void Start()
    {
        // there should be only one Player in the game.
        m_player = gameObject.GetComponent<Player>();

        // there should be only one World in the game.
        m_world = gameObject.GetComponent<World>();

        // there should be only one player camera in the game.
        m_camera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Keypad1))
        {
            m_abs = (int)m_player.position.x - 1;
            m_ord = (int)m_player.position.y - 1;
            m_hasPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            m_abs = (int)m_player.position.x;
            m_ord = (int)m_player.position.y - 1;
            m_hasPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            m_abs = (int)m_player.position.x + 1;
            m_ord = (int)m_player.position.y - 1;
            m_hasPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            m_abs = (int)m_player.position.x - 1;
            m_ord = (int)m_player.position.y;
            m_hasPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Keypad6))
        {
            m_abs = (int)m_player.position.x + 1;
            m_ord = (int)m_player.position.y;
            m_hasPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Keypad7))
        {
            m_abs = (int)m_player.position.x - 1;
            m_ord = (int)m_player.position.y + 1;
            m_hasPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            m_abs = (int)m_player.position.x;
            m_ord = (int)m_player.position.y + 1;
            m_hasPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Keypad9))
        {
            m_abs = (int)m_player.position.x + 1;
            m_ord = (int)m_player.position.y + 1;
            m_hasPressed = true;
        }
        if (m_hasPressed)
        {
            if (m_world.WalkableAt(m_abs, m_ord))
            {
                m_player.Place(m_abs, m_ord);
                m_camera.transform.position = new Vector3(m_player.position.x, m_cameraHeight, m_player.position.y);
                m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, m_player.position.y));
                m_world.UpdateExplored((int)m_player.position.x, (int)m_player.position.y);
                m_hasPressed = false;
            }
        }
    }
}
