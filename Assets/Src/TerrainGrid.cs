using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGrid 
{
    public enum TerrainType
    {
        None,
        Room,
        DoorWay,
        Corridor,
        Wall,
        WallOuter // "Imaginary" wall that ensure rooms won't be generated one next to the other
    }

    public int width;
    public int height;

    private TerrainType[,] m_floorGrid;
    private int[,] m_pathValues;

    private List<Room> m_roomList;
    
    public TerrainGrid(int width, int height)
    {
        this.width = width;
        this.height = height;

        m_floorGrid = new TerrainType[width, height];
        m_roomList = new List<Room>();

        // The level is created empty (aka. all tiles have no type).
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                m_floorGrid[i, j] = TerrainType.None;
    }

    public TerrainType GetTerrainAt(int x, int y)
    {
        return m_floorGrid[x, y];
    }

    /// <summary>
    /// Generates by brute force a random number of random sized rooms (floor and walls) 
    /// and updates the logical grid with that data
    /// </summary>
    public void GenerateRooms()
    {
        RoomFactory roomFactory = new RoomFactory(width,
                                                  height);
        while (!roomFactory.IsComplete() )
        {
            Room roomToInsert = roomFactory.GenerateRoom();

            int count = 0;

            // Brute force to find a suitable position and size until it fits.
            while (!RoomFits(roomToInsert) && count < 100)
            {
                roomToInsert.Generate();

                count++;
            }
            if(count < 25)
            {
                roomFactory.SaveRoom(roomToInsert);

                // Fill the logical grid with the room's floors and walls. 
                FillGrid(roomToInsert);

                m_roomList.Add(roomToInsert);
            }
            else
            {
                roomFactory.RemoveEnvelop(roomToInsert.envelop);
            }
        }
    }
    
    private bool RoomFits(Room room)
    {
        int abscissa = (int)room.roomRect.x;
        int ordinate = (int)room.roomRect.y;
        int roomWidth = (int)room.roomRect.width;
        int roomHeight = (int)room.roomRect.height;

        // Account for walls.
        int minXWall = abscissa - 1;
        int maxXWall = abscissa + roomWidth + 1;
        int minYWall = ordinate - 1;
        int maxYWall = ordinate + roomHeight + 1;

        // Differentiate borders of the level and borders of a non edge envelop.
        int xBorder = 2 * Defines.LevelDefines.s_X_BORDER_SIZE ;
        int yBorder = 2 * Defines.LevelDefines.s_Y_BORDER_SIZE;
        if (room.envelop.x == 0 || room.envelop.x + room.envelop.width >= width - 1)
        {
            xBorder =  Defines.LevelDefines.s_X_BORDER_SIZE + 1;
        }
        if (room.envelop.y == 0 || room.envelop.y + room.envelop.height >= height - 1)
        {
            yBorder = Defines.LevelDefines.s_Y_BORDER_SIZE + 1;
        }

        // Test envelop boundries. 
        // Important : note that we restrict to stictly less or great, that is to ensure wall space.
        if (abscissa > xBorder && abscissa < width - roomWidth - xBorder &&
            ordinate > yBorder && ordinate < height - roomHeight - yBorder)
        {
            for (int i = minXWall; i < maxXWall; i++) // Test the terrain that the room would contain.
                for (int j = minYWall; j < maxYWall; j++)
                    if (m_floorGrid[i, j] != TerrainType.None)
                        return false;
        }
        else
        {
            return false;
        }
        return true;
    }

    // TODO bis : no double doors.
    public void GenerateDoors()
    {
        foreach (Room room in m_roomList)
        {
            int abscissa = (int)room.roomRect.x;
            int ordinate = (int)room.roomRect.y;
            int roomWidth = (int)room.roomRect.width;
            int roomHeight = (int)room.roomRect.height;

            // At maximum the room's walls are made 20% of doors.
            int nbMaxDoors = Defines.LevelDefines.s_MAX_DOOR_PRC * (roomWidth * 2 + roomHeight * 2) / 100;
            int nbDoors = 0;

            // Leave the actual amount of doors to luck.
            while (RnG.PassTest(nbMaxDoors - nbDoors, nbMaxDoors))
            {
                // A door can be created in any of the four walls of this room.
                int direction = Random.Range(0, 4);

                // Bottom and top.
                if (direction == 0 || direction == 1)
                {
                    int ordOffset = direction == 0 ? -1 : roomHeight;
                    //doors at the edge of the level are not allowed.
                    if (ordinate + ordOffset != 0 && ordinate + ordOffset != height - 1) 
                    {
                        int rndAbs = Random.Range(abscissa, abscissa + roomWidth);
                        m_floorGrid[rndAbs, ordinate + ordOffset] = TerrainType.DoorWay;
                        room.AddDoor(new Vector2(rndAbs, ordinate + ordOffset));
                        nbDoors++;
                    }
                }
                // Left and Right.
                if (direction == 2 || direction == 3)
                {
                    int absOffset = direction == 2 ? -1 : roomWidth;
                    
                    //doors at the edge of the level are not allowed.
                    if (abscissa + absOffset != 0 && abscissa + absOffset != width - 1)
                    {
                        int rndOrd = Random.Range(ordinate, ordinate + roomHeight);
                        m_floorGrid[abscissa + absOffset, rndOrd] = TerrainType.DoorWay;
                        room.AddDoor(new Vector2(abscissa + absOffset, rndOrd));
                        nbDoors++;
                    }
                }

            }
        }
    }

    public void GenerateCorridors()
    {
        m_pathValues = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(m_floorGrid[i,j] == TerrainType.Wall 
                || m_floorGrid[i, j] == TerrainType.Room )
                {
                    m_pathValues[i, j] = -1;
                }
                else
                {
                    m_pathValues[i, j] = 0;
                }
            }
        }

        List<Room.DoorWay> tempDoorList = new List<Room.DoorWay>();

        // TODO : check if modifications to the doorway are passed on through the room.
        foreach (Room room in m_roomList)
        {
            tempDoorList.AddRange(room.doorWayList);
        }

        while (tempDoorList.Count > 1)
        {
            int firstIndex = Random.Range(0, tempDoorList.Count);
            Room.DoorWay doorWayStart = tempDoorList[firstIndex];
            tempDoorList.RemoveAt(firstIndex);

            int secondIndex = Random.Range(0, tempDoorList.Count);
            Room.DoorWay doorWayEnd = tempDoorList[secondIndex];
            tempDoorList.RemoveAt(secondIndex);

            Dijkstra dijkstra = new Dijkstra(m_pathValues);

            List<Vector2> res = dijkstra.Run(doorWayStart.position, doorWayEnd.position);

            for (int i = 0; i < res.Count(); i++)
            {
                if (m_floorGrid[(int)res[i].x, (int)res[i].y] != TerrainType.DoorWay)
                    m_floorGrid[(int)res[i].x, (int)res[i].y] = TerrainType.Corridor;
            }
        }
    }

    private void FillGrid(Room room)
    {
        int abscissa = (int)room.roomRect.x;
        int ordinate = (int)room.roomRect.y;
        int roomWidth = (int)room.roomRect.width;
        int roomHeight = (int)room.roomRect.height;

        // Fill the whole space with walls and outer walls.
        // If we reach any side of the level the the outter wall is not necessary.
        int minXWall = abscissa - 2 < 0 ? 0 : abscissa - 2;
        int maxXWall = abscissa + roomWidth + 2 > width ? width : abscissa + roomWidth + 2;
        int minYWall = ordinate - 2 < 0 ? 0 : ordinate - 2;
        int maxYWall = ordinate + roomHeight + 2 > height ? height : ordinate + roomHeight + 2;

        for (int i = minXWall; i < maxXWall; i++)
            for (int j = minYWall; j < maxYWall; j++)
                if(m_floorGrid[i, j] == TerrainType.None)
                   m_floorGrid[i, j] = TerrainType.WallOuter;

        for (int i = abscissa - 1; i < abscissa + roomWidth + 1; i++)
            for (int j = ordinate - 1; j < ordinate + roomHeight + 1; j++)
                m_floorGrid[i, j] = TerrainType.Wall;

        // Fill the actual room volume.
        for (int i = abscissa; i < abscissa + roomWidth; i++)
            for (int j = ordinate; j < ordinate + roomHeight; j++)
                m_floorGrid[i, j] = TerrainType.Room;
    }

    public void Clear()
    {
        m_floorGrid = new TerrainType[width, height];

        // The level is created empty (aka. all tiles have no type).
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                m_floorGrid[i, j] = TerrainType.None;

        m_roomList.Clear();
    }

    public Room RandomRoom()
    {
        int rndIndex;
        if (m_roomList.Count > 0)
            rndIndex = Random.Range(0, m_roomList.Count);
        else
            return null;
        return m_roomList[rndIndex];
    }

    public void Print()
    {
        string s = new string("Hello".ToCharArray());

        for (int j = 0; j < m_floorGrid.GetLength(1); j++)
        {
            for (int i = 0; i < m_floorGrid.GetLength(0); i++)
                s += " " + m_floorGrid[i, j];
            s += '\n';
        }
    }
}
