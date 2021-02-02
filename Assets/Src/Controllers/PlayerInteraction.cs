using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Player m_player;

    private World m_world;

    private HUDManager m_HUDManager;

    private bool m_waitingDirection = false;

    // Start is called before the first frame update
    void Start()
    {
        // there should be only one Player in the game.
        m_player = gameObject.GetComponent<Player>();

        // there should be only one World in the game.
        m_world = gameObject.GetComponent<World>();

        // there should be only one HUDManager in the game.
        m_HUDManager = gameObject.GetComponent<HUDManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_world.IsGenerated())
            return;

        if (Input.GetKeyUp(KeyCode.O))
        {
            m_HUDManager.prompt("In which direction?");

            m_waitingDirection = true;

            StartCoroutine(GetDirection());
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

    private IEnumerator GetDirection()
    {
        bool done = false;
        while (!done)
        {
            foreach (KeyCode kcode in KeyCode.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyUp(kcode) && kcode != KeyCode.O)
                {
                    if (IsDirection(kcode))
                        OpenDoor(kcode);
                    else
                        m_HUDManager.prompt("Not a valid direction ! Use NUM PAD.");
                    done = true;

                    m_waitingDirection = false;
                }
            }
            yield return null;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    public bool IsDirection(KeyCode key)
    {
        if (key == KeyCode.Keypad1 || key == KeyCode.Keypad2 || key == KeyCode.Keypad3 || key == KeyCode.Keypad4 ||
            key == KeyCode.Keypad6 || key == KeyCode.Keypad7 || key == KeyCode.Keypad8 || key == KeyCode.Keypad9)
            return true;
        else
            return false;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void OpenDoor(KeyCode key)
    {
        int openResult = 0;
        if (key == KeyCode.Keypad1)
        {
            openResult = m_world.OpenAt((int)m_player.position.x - 1, (int)m_player.position.y - 1);
        }
        if (key == KeyCode.Keypad2)
        {
            openResult = m_world.OpenAt((int)m_player.position.x, (int)m_player.position.y - 1);
        }
        if (key == KeyCode.Keypad3)
        {
            openResult = m_world.OpenAt((int)m_player.position.x + 1, (int)m_player.position.y - 1);
        }
        if (key == KeyCode.Keypad4)
        {
            openResult = m_world.OpenAt((int)m_player.position.x - 1, (int)m_player.position.y);
        }
        if (key == KeyCode.Keypad6)
        {
            openResult = m_world.OpenAt((int)m_player.position.x + 1, (int)m_player.position.y);
        }
        if (key == KeyCode.Keypad7)
        {
            openResult = m_world.OpenAt((int)m_player.position.x - 1, (int)m_player.position.y + 1);
        }
        if (key == KeyCode.Keypad8)
        {
            openResult = m_world.OpenAt((int)m_player.position.x, (int)m_player.position.y + 1);
        }
        if (key == KeyCode.Keypad9)
        {
            openResult = m_world.OpenAt((int)m_player.position.x + 1, (int)m_player.position.y + 1);
        }
        if(openResult == (int)Room.DoorStatusType.None)
            m_HUDManager.prompt("You see no door here.");
        if (openResult == (int)Room.DoorStatusType.Closed)
            m_HUDManager.prompt("The door opens.");
        if (openResult == (int)Room.DoorStatusType.Locked)
            m_HUDManager.prompt("This door is locked.");
        if (openResult == (int)Room.DoorStatusType.Empty)
            m_HUDManager.prompt("This doorway has no door.");
        if (openResult == (int)Room.DoorStatusType.Open)
            m_HUDManager.prompt("This door is already open.");


        m_world.UpdateExplored((int)m_player.position.x, (int)m_player.position.y);
    }

    //-----------------------------------------------------------------------------------------------------------------

    public bool IsWaitingDirection()
    {
        return m_waitingDirection;
    }

    //-----------------------------------------------------------------------------------------------------------------
}