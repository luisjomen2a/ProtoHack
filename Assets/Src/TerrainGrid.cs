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

    private int m_completion;

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
        while(!this.IsCompleted())
        {
            Room roomToInsert = new Room(width, height);
            roomToInsert.Generate();

            // Brute force to find a suitable position and size until it fits.
            while (!this.RoomFits(roomToInsert))
                roomToInsert.Generate();

            // Fill the logical grid with the room's floors and walls. 
            this.FillGrid(roomToInsert);

            m_roomList.Add(roomToInsert);  
        }
    }
    
    private bool RoomFits(Room room)
    {
        // Account for walls.
        int minXWall = room.abscissa - 1 < 0 ? 0 : room.abscissa - 1;
        int maxXWall = room.abscissa + room.width + 1 > width ? width : room.abscissa + room.width + 1;
        int minYWall = room.ordinate - 1 < 0 ? 0 : room.ordinate - 1;
        int maxYWall = room.ordinate + room.height + 1 > height ? height : room.ordinate + room.height + 1;

        // Test level boundries. 
        // Important : note that we restrict to stictly less or great, that is to ensure wall space.
        if (room.abscissa > 0 && room.abscissa < width - room.width 
            && room.ordinate > 0 && room.ordinate < height - room.height)
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
            // At maximum the room's walls are made 20% of doors.
            int nbMaxDoors = Defines.s_MAX_DOOR_PRC * (room.width * 2 + room.height * 2) / 100;
            int nbDoors = 0;

            // Leave the actual amount of doors to luck.
            while (RnG.PassTest(nbMaxDoors - nbDoors, nbMaxDoors))
            {
                // A door can be created in any of the four walls of this room.
                int direction = Random.Range(0, 4);

                // Bottom and top.
                if (direction == 0 || direction == 1)
                {
                    int ordOffset = direction == 0 ? -1 : room.height;
                    //doors at the edge of the level are not allowed.
                    if (room.ordinate + ordOffset != 0 && room.ordinate + ordOffset != height - 1) 
                    {
                        int rndAbs = Random.Range(room.abscissa, room.abscissa + room.width);
                        m_floorGrid[rndAbs, room.ordinate + ordOffset] = TerrainType.DoorWay;
                        room.AddDoor(new Vector2(rndAbs, room.ordinate + ordOffset));
                        nbDoors++;
                    }
                }
                // Left and Right.
                if (direction == 2 || direction == 3)
                {
                    int absOffset = direction == 2 ? -1 : room.width;
                    
                    //doors at the edge of the level are not allowed.
                    if (room.abscissa + absOffset != 0 && room.abscissa + absOffset != width - 1)
                    {
                        int rndOrd = Random.Range(room.ordinate, room.ordinate + room.height);
                        m_floorGrid[room.abscissa + absOffset, rndOrd] = TerrainType.DoorWay;
                        room.AddDoor(new Vector2(room.abscissa + absOffset, rndOrd));
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
        // Fill the whole space with walls and outer walls.
        // If we reach any side of the level the the outter wall is not necessary.
        int minXWall = room.abscissa - 2 < 0 ? 0 : room.abscissa - 2;
        int maxXWall = room.abscissa + room.width + 2 > width ? width : room.abscissa + room.width + 2;
        int minYWall = room.ordinate - 2 < 0 ? 0 : room.ordinate - 2;
        int maxYWall = room.ordinate + room.height + 2 > height ? height : room.ordinate + room.height + 2;

        for (int i = minXWall; i < maxXWall; i++)
            for (int j = minYWall; j < maxYWall; j++)
                if(m_floorGrid[i, j] == TerrainType.None)
                   m_floorGrid[i, j] = TerrainType.WallOuter;

        for (int i = room.abscissa - 1; i < room.abscissa + room.width + 1; i++)
            for (int j = room.ordinate - 1; j < room.ordinate + room.height + 1; j++)
                m_floorGrid[i, j] = TerrainType.Wall;

        // Fill the actual room volume.
        for (int i = room.abscissa; i < room.abscissa + room.width; i++)
            for (int j = room.ordinate; j < room.ordinate + room.height; j++)
                m_floorGrid[i, j] = TerrainType.Room;

        // Space occupied in the whole level.
        m_completion += (room.width + 1) * (room.height + 1);
    }

    public bool IsCompleted()
    {
        return m_completion > Defines.s_MAX_CAPACITY;
    }

    public void Clear()
    {
        m_floorGrid = new TerrainType[width, height];

        // The level is created empty (aka. all tiles have no type).
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                m_floorGrid[i, j] = TerrainType.None;

        m_completion = 0;
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
}
