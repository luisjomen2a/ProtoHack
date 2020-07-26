using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Rect roomRect;

    public List<DoorWay> doorWayList;

    public bool generated = false;

    private Rect m_envelop;


    public enum DoorStatusType
    {
        Empty,
        Created
    }

    public struct DoorWay
    {
        public Vector2 position;
        public DoorStatusType status;
        public bool connected;

        public DoorWay(Vector2 pos, DoorStatusType stat) { position = pos; status = stat; connected = false; }
    }

    /// <summary>
    /// Generates a room within a certain rectangle envelop.
    /// </summary>
    /// <param name="envelope">minimum and maximal area the room could occupy</param>
    public Room(Rect envelope)
    {
        this.m_envelop = envelope;

        doorWayList = new List<DoorWay>();

        roomRect = new Rect();

        this.Generate();
    }

    /// <summary>
    /// Generates a room with random properteis (width, height, abscissa and ordinate).
    /// The sole
    /// purpose of this method is to avoid doing all this in the constructor tu avoid useless object creation.
    /// </summary>
    public void Generate()
    {
        // Allow for bigger rooms when possible.
        if(m_envelop.width > Defines.LevelDefines.s_LARGE_ROOM_THRESHOLD)
            roomRect.width = Random.Range(Defines.LevelDefines.s_ROOM_MIN_WIDTH_OR_HEIGHT, 
                                          Defines.LevelDefines.s_LARGE_ROOM_MAX_WIDTH);
        else
            roomRect.width = Random.Range(Defines.LevelDefines.s_ROOM_MIN_WIDTH_OR_HEIGHT,
                                          Defines.LevelDefines.s_ROOM_MAX_WIDTH);
        roomRect.height = Random.Range(Defines.LevelDefines.s_ROOM_MIN_WIDTH_OR_HEIGHT,
                                       Defines.LevelDefines.s_ROOM_MAX_HEIGHT);
        roomRect.x = Random.Range(m_envelop.x, m_envelop.width);
        roomRect.y = Random.Range(m_envelop.y, m_envelop.height);

        // Force the maximal area by minimizing the height.
        if(roomRect.width * roomRect.height > Defines.LevelDefines.s_ROOM_MAX_AREA)
        {
            roomRect.height = Defines.LevelDefines.s_ROOM_MAX_AREA / roomRect.width;
        }
    }

    public void AddDoor(Vector2 pos)
    {
        doorWayList.Add(new DoorWay(pos, DoorStatusType.Empty));
    }

    public static string operator +(string s, Room a) => s + "\n x : " + a.roomRect.x + ", y : " + a.roomRect.y 
                                                           + ", w : " + a.roomRect.width + ", h : " + a.roomRect.height;
}
