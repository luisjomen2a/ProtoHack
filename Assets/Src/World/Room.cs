using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class Room : System.IComparable<Room>
{
    public Rect m_roomRect;

    public List<DoorWay> m_doorWayList;

    public bool m_generated = false;

    public Rect m_envelop;

    public enum DoorStatusType
    {
        None=-1, // Reserved for non doors artifacts. Theorically, every none-doorway tile is of None type
        Empty = 0,
        Closed,
        Open,
        Locked,
        Trapped
    }

    public struct DoorWay
    {
        public Vector2 position;
        public DoorStatusType status;

        public DoorWay(Vector2 pos, DoorStatusType stat) { position = pos; status = stat; }
    }

    //-----------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Generates a room within a certain rectangle envelop.
    /// </summary>
    /// <param name="envelope">minimum and maximal area the room could occupy</param>
    public Room(Rect envelope)
    {
        m_envelop = envelope;

        m_doorWayList = new List<DoorWay>();

        m_roomRect = new Rect();

        Generate();
    }

    //-----------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Generates a room with random properteis (width, height, abscissa and ordinate).
    /// The sole
    /// purpose of this method is to avoid doing all this in the constructor tu avoid useless object creation.
    /// </summary>
    public void Generate()
    {
        // Allow for bigger rooms when possible.
        if(m_envelop.width > Defines.LevelDefines.s_LARGE_ROOM_THRESHOLD)
            m_roomRect.width = Random.Range(Defines.LevelDefines.s_ROOM_MIN_WIDTH, 
                                          Defines.LevelDefines.s_LARGE_ROOM_MAX_WIDTH);
        else
            m_roomRect.width = Random.Range(Defines.LevelDefines.s_ROOM_MIN_WIDTH,
                                          Defines.LevelDefines.s_ROOM_MAX_WIDTH);
        m_roomRect.height = Random.Range(Defines.LevelDefines.s_ROOM_MIN_HEIGHT,
                                       Defines.LevelDefines.s_ROOM_MAX_HEIGHT);
        m_roomRect.x = Random.Range((int)m_envelop.x, (int)m_envelop.width);
        m_roomRect.y = Random.Range((int)m_envelop.y, (int)m_envelop.height);

        // Force the maximal area by minimizing the height.
        if(m_roomRect.width * m_roomRect.height > Defines.LevelDefines.s_ROOM_MAX_AREA)
        {
            m_roomRect.height = Defines.LevelDefines.s_ROOM_MAX_AREA / (int)m_roomRect.width;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

    public DoorStatusType GetDoorwayStatusAt(int x, int y)
    {
        for (int i = 0; i < m_doorWayList.Count; i++)
        {
            if (m_doorWayList[i].position == new Vector2(x, y))
                return m_doorWayList[i].status;
        }
        return DoorStatusType.None;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public bool SetDoorwayStatusAt(int x, int y, DoorStatusType doorStatus)
    {
        for (int i = 0; i < m_doorWayList.Count; i++)
        {
            if (m_doorWayList[i].position == new Vector2(x, y))
            {
                m_doorWayList[i] = new DoorWay(m_doorWayList[i].position, doorStatus);
                return true;
            }
        }
        return false;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public int CompareTo(Room compareRoom)
    {
        if (m_roomRect.x < compareRoom.m_roomRect.x)
            return -1;
        else if (m_roomRect.x > compareRoom.m_roomRect.x)
            return 1;
        else
            return 0;
    }


    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Finds a position for a doorway at random.
    /// </summary>
    /// <returns>position of the random generated door.</returns>
    public Vector2 FindDoorway()
    {
        // A door can be created in any of the four walls of this room.
        int direction = Random.Range(0, 4);

        int abscissa = (int)m_roomRect.x;
        int ordinate = (int)m_roomRect.y;
        int roomWidth = (int)m_roomRect.width;
        int roomHeight = (int)m_roomRect.height;
        Vector2 newDoor = new Vector2(-1, -1);

        switch (direction)
        {
            case 0: //Botoom.
                // Note that we place doorways on the room's wall, 
                // So all coordinates need to be offsetted by one.
                int rndAbs = Random.Range(abscissa, abscissa + roomWidth);
                newDoor = new Vector2(rndAbs, ordinate - 1);
                break;

            case 1: //Top
                rndAbs = Random.Range(abscissa, abscissa + roomWidth);
                newDoor = new Vector2(rndAbs, ordinate + roomHeight);
                break;
            case 2: // Left
                int rndOrd = Random.Range(ordinate, ordinate + roomHeight);
                newDoor = new Vector2(abscissa - 1, rndOrd);
                break;
            case 3: // Right
                rndOrd = Random.Range(ordinate, ordinate + roomHeight);
                newDoor = new Vector2(abscissa + roomWidth, rndOrd);
                break;
        }       
        return newDoor;
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Generates either a hidden door, closed door or a doorway. If a door is generated it is maked either locked or "open".
    /// Probabilites : 
    ///    - 2/3         : 66% of no door appearing at all.
    ///    - 1/3*4/5*5/6 : 22.22% Closed Door
    ///    - 1/3*1/5     : 6.66% Door is open
    ///    - 1/3*4/5*1/6 : 4.44% locked Door
    /// </summary>
    /// <param name="x">abscissa of the doorway.</param>
    /// <param name="y">ordinate of the doorway.</param>
    public void AddDoorway(Vector2 doorWay)
    {
        if(RnG.PassTest(2,3))
        {
            if(RnG.PassTest(1,5))
                m_doorWayList.Add(new DoorWay(doorWay, DoorStatusType.Open));
            else if(RnG.PassTest(1, 6))
                m_doorWayList.Add(new DoorWay(doorWay, DoorStatusType.Locked));
            else
                m_doorWayList.Add(new DoorWay(doorWay, DoorStatusType.Closed));
            // Trapped doors would be here.
        }
        else
            m_doorWayList.Add(new DoorWay(doorWay, DoorStatusType.Empty));
    }

    //-----------------------------------------------------------------------------------------------------------------

    public static string operator +(string s, Room a) => s + "\n x : " + a.m_roomRect.x + ", y : " + a.m_roomRect.y
                                                           + ", w : " + a.m_roomRect.width + ", h : " + a.m_roomRect.height;
    //-----------------------------------------------------------------------------------------------------------------

}
