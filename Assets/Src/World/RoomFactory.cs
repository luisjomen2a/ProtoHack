using System.Collections.Generic;
using UnityEngine;

public class RoomFactory 
{
    private int m_width;
    private int m_height;

    private List<Rect> m_envelops;

    public RoomFactory(int width, int height)
    {
        m_width = width;
        m_height = height;

        // At the beginning the envelop is the whole level.
        m_envelops = new List<Rect>
        {
            new Rect(0, 0, m_width, m_height)
        };
    }

    //-----------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Simply chooses a random envelop and assigns it to a Room.
    /// </summary>
    /// <returns> Room with the random chosen envelop.</returns>
    public Room GenerateRoom()
    {
        if (!(m_envelops.Count > 0))
            return new Room(new Rect(-1, -1, -1, -1));

        Rect envelop = m_envelops[Random.Range(0, m_envelops.Count)];

        Room returnRoom = new Room(envelop);

        return returnRoom;
    }

    //-----------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a maximum of 4 envelops around the input room.
    /// </summary>
    /// <param name="savedRoom">room and the envelop that is going to be split.</param>
    public void SaveRoom(Room savedRoom)
    {
        Rect envelop = savedRoom.m_envelop;
        Rect roomRect = savedRoom.m_roomRect;

        if(!m_envelops.Remove(envelop))
        {
            return;
        }

        if (roomRect.x - envelop.x > Defines.LevelDefines.s_ROOM_MIN_WIDTH &&
            m_envelops.Count < Defines.LevelDefines.s_MAX_ENVELOP_COUNT)
        {
            Rect newRect = new Rect(envelop);
            newRect.width = roomRect.x - envelop.x;
            m_envelops.Add(newRect);
        }
        if (roomRect.y - envelop.y > Defines.LevelDefines.s_ROOM_MIN_HEIGHT &&
            m_envelops.Count < Defines.LevelDefines.s_MAX_ENVELOP_COUNT)
        {
            Rect newRect = new Rect(envelop);
            newRect.height = roomRect.y - envelop.y;
            m_envelops.Add(newRect);
        }
        if (envelop.width - (roomRect.x - envelop.x + roomRect.width) > Defines.LevelDefines.s_ROOM_MIN_WIDTH &&
            m_envelops.Count < Defines.LevelDefines.s_MAX_ENVELOP_COUNT)
        {
            Rect newRect = new Rect(envelop);
            newRect.x = roomRect.x - envelop.x + roomRect.width;
            m_envelops.Add(newRect);
        }
        if (envelop.height - (roomRect.y - envelop.y + roomRect.height) > Defines.LevelDefines.s_ROOM_MIN_HEIGHT &&
            m_envelops.Count < Defines.LevelDefines.s_MAX_ENVELOP_COUNT)
        {
            Rect newRect = new Rect(envelop);
            newRect.y = roomRect.y - envelop.y + roomRect.height;
            m_envelops.Add(newRect);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

    public bool IsComplete()
    {
        return m_envelops.Count <= 0;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public bool RemoveEnvelop(Rect envelop)
    {
        return m_envelops.Remove(envelop);
    }

    //-----------------------------------------------------------------------------------------------------------------

}

