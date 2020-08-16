using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class Room : System.IComparable<Room>
{
    public Rect roomRect;

    public List<DoorWay> doorWayList;

    public bool generated = false;

    public Rect envelop;

    public enum DoorStatusType
    {
        Empty,
        Created
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
        envelop = envelope;

        doorWayList = new List<DoorWay>();

        roomRect = new Rect();

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
        if(envelop.width > Defines.LevelDefines.s_LARGE_ROOM_THRESHOLD)
            roomRect.width = Random.Range(Defines.LevelDefines.s_ROOM_MIN_WIDTH, 
                                          Defines.LevelDefines.s_LARGE_ROOM_MAX_WIDTH);
        else
            roomRect.width = Random.Range(Defines.LevelDefines.s_ROOM_MIN_WIDTH,
                                          Defines.LevelDefines.s_ROOM_MAX_WIDTH);
        roomRect.height = Random.Range(Defines.LevelDefines.s_ROOM_MIN_HEIGHT,
                                       Defines.LevelDefines.s_ROOM_MAX_HEIGHT);
        roomRect.x = Random.Range((int)envelop.x, (int)envelop.width);
        roomRect.y = Random.Range((int)envelop.y, (int)envelop.height);

        // Force the maximal area by minimizing the height.
        if(roomRect.width * roomRect.height > Defines.LevelDefines.s_ROOM_MAX_AREA)
        {
            roomRect.height = Defines.LevelDefines.s_ROOM_MAX_AREA / (int)roomRect.width;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

    public int CompareTo(Room compareRoom)
    {
        if (roomRect.x < compareRoom.roomRect.x)
            return -1;
        else if (roomRect.x > compareRoom.roomRect.x)
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

        int abscissa = (int)roomRect.x;
        int ordinate = (int)roomRect.y;
        int roomWidth = (int)roomRect.width;
        int roomHeight = (int)roomRect.height;
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
    public void AddDoorway(DoorWay doorWay)
    {
        doorWayList.Add(doorWay);
    }

    //-----------------------------------------------------------------------------------------------------------------

    public static string operator +(string s, Room a) => s + "\n x : " + a.roomRect.x + ", y : " + a.roomRect.y
                                                           + ", w : " + a.roomRect.width + ", h : " + a.roomRect.height;

    //-----------------------------------------------------------------------------------------------------------------

}
