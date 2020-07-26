using System.Collections;
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

    public Room GenerateRoom()
    {
        if (!(m_envelops.Count > 0))
            return new Room(new Rect(-1, -1, -1, -1));

        Rect envelop = m_envelops[Random.Range(0, m_envelops.Count)];

        Room returnRoom = new Room(envelop);

        return returnRoom;
    }


}

