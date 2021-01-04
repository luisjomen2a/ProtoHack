using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player m_player;

    private World m_world;

    private Camera m_camera;

    private bool m_isMoving = false;

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
        if (m_isMoving)
            return;

        if (Input.GetKeyUp(KeyCode.Keypad1))
        {
            if(m_world.WalkableAt((int)m_player.position.x - 1, (int)m_player.position.y - 1))
            {
                StartCoroutine(MoveLeft());
                StartCoroutine(MoveDown());
                m_world.UpdateExplored((int)m_player.position.x - 1, (int)m_player.position.y - 1);
                m_isMoving = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            if (m_world.WalkableAt((int)m_player.position.x, (int)m_player.position.y - 1))
            {
                StartCoroutine(MoveDown());
                m_world.UpdateExplored((int)m_player.position.x, (int)m_player.position.y - 1);
                m_isMoving = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            if (m_world.WalkableAt((int)m_player.position.x + 1, (int)m_player.position.y - 1))
            {
                StartCoroutine(MoveRight());
                StartCoroutine(MoveDown());
                m_world.UpdateExplored((int)m_player.position.x + 1, (int)m_player.position.y - 1);
                m_isMoving = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            if (m_world.WalkableAt((int)m_player.position.x - 1, (int)m_player.position.y))
            {
                StartCoroutine(MoveLeft());
                m_world.UpdateExplored((int)m_player.position.x - 1, (int)m_player.position.y );
                m_isMoving = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad6))
        {
            if (m_world.WalkableAt((int)m_player.position.x + 1, (int)m_player.position.y))
            {
                StartCoroutine(MoveRight());
                m_world.UpdateExplored((int)m_player.position.x + 1, (int)m_player.position.y);
                m_isMoving = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad7))
        {
            if (m_world.WalkableAt((int)m_player.position.x - 1, (int)m_player.position.y + 1))
            {
                StartCoroutine(MoveLeft());
                StartCoroutine(MoveUp());
                m_world.UpdateExplored((int)m_player.position.x - 1, (int)m_player.position.y + 1);
                m_isMoving = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            if (m_world.WalkableAt((int)m_player.position.x, (int)m_player.position.y + 1))
            {
                StartCoroutine(MoveUp());
                m_world.UpdateExplored((int)m_player.position.x, (int)m_player.position.y + 1);
                m_isMoving = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad9))
        {
            if (m_world.WalkableAt((int)m_player.position.x + 1, (int)m_player.position.y + 1))
            {
                StartCoroutine(MoveUp());
                StartCoroutine(MoveRight());
                m_world.UpdateExplored((int)m_player.position.x + 1, (int)m_player.position.y + 1);
                m_isMoving = true;
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

    IEnumerator MoveLeft()
    {
        float abs = m_player.position.x;
        for (float i = abs; i > abs - 1; i -= 0.25f)
        {
            m_player.Place(i, m_player.position.y);
            m_camera.transform.position = new Vector3(i, m_cameraHeight, m_player.position.y);
            m_camera.transform.LookAt(new Vector3(i, 2, m_player.position.y));
            yield return null;
        }

        m_player.Place(abs - 1, m_player.position.y);
        m_camera.transform.position = new Vector3(abs - 1, m_cameraHeight, m_player.position.y);
        m_camera.transform.LookAt(new Vector3(abs - 1, 2, m_player.position.y));

        m_isMoving = false;
    }

    //-----------------------------------------------------------------------------------------------------------------

    IEnumerator MoveRight()
    {
        float abs = m_player.position.x;
        for (float i = abs; i < abs + 1; i += 0.25f)
        {
            m_player.Place(i, m_player.position.y);
            m_camera.transform.position = new Vector3(i, m_cameraHeight, m_player.position.y);
            m_camera.transform.LookAt(new Vector3(i, 2, m_player.position.y));
            yield return null;
        }

        m_player.Place(abs + 1, m_player.position.y);
        m_camera.transform.position = new Vector3(abs + 1, m_cameraHeight, m_player.position.y);
        m_camera.transform.LookAt(new Vector3(abs + 1, 2, m_player.position.y));
        m_isMoving = false;
    }

    //-----------------------------------------------------------------------------------------------------------------

    IEnumerator MoveDown()
    {
        float ord = m_player.position.y;
        for (float j = ord; j > ord - 1; j -= 0.25f)
        {
            m_player.Place(m_player.position.x, j);
            m_camera.transform.position = new Vector3(m_player.position.x, m_cameraHeight, j);
            m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, j));
            yield return null;
        }

        m_player.Place(m_player.position.x, ord - 1);
        m_camera.transform.position = new Vector3(m_player.position.x, m_cameraHeight, ord - 1);
        m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, ord - 1));

        m_isMoving = false;
    }

    //-----------------------------------------------------------------------------------------------------------------

    IEnumerator MoveUp()
    {
        float ord = m_player.position.y;
        for (float j = ord; j < ord + 1; j += 0.25f)
        {
            m_player.Place(m_player.position.x, j);
            m_camera.transform.position = new Vector3(m_player.position.x, m_cameraHeight, j);
            m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, j));
            yield return null;
        }

        m_player.Place(m_player.position.x, ord + 1);
        m_camera.transform.position = new Vector3(m_player.position.x, m_cameraHeight, ord + 1);
        m_camera.transform.LookAt(new Vector3(m_player.position.x, 2, ord + 1));

        m_isMoving = false;
    }

}
