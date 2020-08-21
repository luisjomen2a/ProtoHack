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
        StairsUp,
        StairsDown,
        WallOuter // "Imaginary" wall that ensure rooms won't be generated one next to the other
    }

    public enum StatusType
    {
        Unexplored,
        Explored,
        Lit
    }

    public int width;
    public int height;

    private TerrainType[,] m_terrainGrid;
    private StatusType[,] m_statusGrid;

    private List<Room> m_roomList;

    public TerrainGrid(int width, int height)
    {
        this.width = width;
        this.height = height;

        m_terrainGrid = new TerrainType[width, height];
        m_statusGrid = new StatusType[width, height];
        m_roomList = new List<Room>();

        // The level is created empty (aka. all tiles have no type).
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                m_terrainGrid[i, j] = TerrainType.None;

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                m_statusGrid[i, j] = StatusType.Unexplored;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public TerrainType GetTerrainAt(int x, int y)
    {
        return m_terrainGrid[x, y];
    }

    //-----------------------------------------------------------------------------------------------------------------

    public StatusType GetStatusAt(int x, int y)
    {
        return m_statusGrid[x, y];
    }

    //-----------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Generates by brute force a random number of random sized rooms (floor and walls) 
    /// and updates the logical grid with that data
    /// </summary>
    public void GenerateRooms()
    {
        RoomFactory roomFactory = new RoomFactory(width,
                                                  height);
        while (!roomFactory.IsComplete())
        {
            Room roomToInsert = roomFactory.GenerateRoom();

            int count = 0;

            // Brute force to find a suitable position and size until it fits.
            while (!RoomFits(roomToInsert) && count < 100)
            {
                roomToInsert.Generate();

                count++;
            }
            if (count < 25)
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

        // Once generation is over, clean up the outer walls.
        for (int i = 0; i < m_terrainGrid.GetLength(0); i++)
            for (int j = 0; j < m_terrainGrid.GetLength(1); j++)
                if (m_terrainGrid[i, j] == TerrainType.WallOuter)
                    m_terrainGrid[i, j] = TerrainType.None;

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
        int xBorder = 2 * Defines.LevelDefines.s_X_BORDER_SIZE;
        int yBorder = 2 * Defines.LevelDefines.s_Y_BORDER_SIZE;
        if (room.envelop.x == 0 || room.envelop.x + room.envelop.width >= width - 1)
        {
            xBorder = Defines.LevelDefines.s_X_BORDER_SIZE + 1;
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
                    if (m_terrainGrid[i, j] != TerrainType.None)
                        return false;
        }
        else
        {
            return false;
        }
        return true;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void GenerateCorridors()
    {
        // Saves which index is connected. At the end this should be filled with '0'.
        int[] connectIndexes = new int[m_roomList.Count];

        for (int i = 0; i < m_roomList.Count; i++)
            connectIndexes[i] = i;

        // Sort rooms from left to right.
        m_roomList.Sort();

        // Connect all rooms from left to right...
        for (int i = 0; i < m_roomList.Count - 1; i++)
        {
            Join(m_roomList[i], m_roomList[i + 1]);
            if (connectIndexes[i] < connectIndexes[i + 1]) // we mark both rooms as connected.
                connectIndexes[i + 1] = connectIndexes[i];
            else
                connectIndexes[i] = connectIndexes[i + 1];
            // ... But allow some randomness so that some other cases exist
            if (RnG.PassTest(1, 10))
                break;
        }

        // Connect all rooms by skipping one each time.
        for (int i = 0; i < m_roomList.Count - 2; i++)
        {
            if (connectIndexes[i] != connectIndexes[i + 2])
            {
                Join(m_roomList[i], m_roomList[i + 2]);
                if (connectIndexes[i] < connectIndexes[i + 2])
                    connectIndexes[i + 2] = connectIndexes[i];
                else
                    connectIndexes[i] = connectIndexes[i + 2];
            }
        }

        // Generate random corridors, based no Nethack code.
        for (int i = Random.Range(0, m_roomList.Count); i > 0; i--)
        {
            int idx1 = Random.Range(0, m_roomList.Count);
            int idx2 = Random.Range(0, m_roomList.Count);
            while (idx1 == idx2)
                idx2 = Random.Range(0, m_roomList.Count);
            Join(m_roomList[idx1], m_roomList[idx2]);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Generates a door on each of the passed rooms and joins them with a corridor.
    /// </summary>
    /// <param name="room1">firt room to join.</param>
    /// <param name="room2">second room to join.</param>
    public void Join(Room room1, Room room2)
    {
        // Find a door for each room.
        Vector2 doorWay1;
        Vector2 doorWay2;

        // We need each doorway to be placed on an empty wall.
        do
            doorWay1 = room1.FindDoorway();
        while (!IsDoorOk(doorWay1));
        do
            doorWay2 = room2.FindDoorway();
        while (!IsDoorOk(doorWay2));

        room1.AddDoorway(new Room.DoorWay(doorWay1, Room.DoorStatusType.Empty));
        room2.AddDoorway(new Room.DoorWay(doorWay2, Room.DoorStatusType.Empty));

        FillGrid(doorWay1);
        FillGrid(doorWay2);

        // Call dijkstra algorithm with booth doors.
        Dijkstra dijkstra = new Dijkstra(CreateDijkstraMap());

        List<Vector2> res = dijkstra.Run(doorWay1, doorWay2);

        FillGrid(res);
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Fills a map using the terrain grid for the dijkstra algorith,
    /// all obstacles (walls and rooms for corridor generation) are set to '-1', usable tiles are set to '0'.
    /// </summary>
    /// <returns>A map of obstacles for the dijkstra algorithm.</returns>
    public int[,] CreateDijkstraMap()
    {
        int[,] m_pathValues;

        m_pathValues = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_terrainGrid[i, j] == TerrainType.Wall
                || m_terrainGrid[i, j] == TerrainType.Room)
                {
                    m_pathValues[i, j] = -1;
                }
                else
                {
                    m_pathValues[i, j] = 0;
                }
            }
        }
        return m_pathValues;
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Tests if given position is ok for a doorway.
    /// aka. Not on an already existing door and not next to a door.
    /// </summary>
    /// <param name="doorWay">Position to test.</param>
    /// <returns>Returns ture if the position is available.</returns>
    public bool IsDoorOk(Vector2 doorWay)
    {
        // Current position is a free wall.
        if (m_terrainGrid[(int)doorWay.x, (int)doorWay.y] == TerrainType.Wall)
        {
            // Either horizontal neighbours are walls or vertical neighbours are free walls.
            // Note : Rooms are never generated on borders, not point in testing coordinates.
            if (m_terrainGrid[(int)doorWay.x - 1, (int)doorWay.y] == TerrainType.Wall &&
                m_terrainGrid[(int)doorWay.x + 1, (int)doorWay.y] == TerrainType.Wall)
            {
                return true;
            }
            if (m_terrainGrid[(int)doorWay.x, (int)doorWay.y + 1] == TerrainType.Wall &&
                m_terrainGrid[(int)doorWay.x, (int)doorWay.y - 1] == TerrainType.Wall)
            {
                return true;
            }
        }
        return false;
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Generates a pair of staris, one goes down and the other goes up.
    /// </summary>
    public void GenerateStairs()
    {
        // Staircase up.
        Room roomUp = RandomRoom();

        int stairUpX = Random.Range(0, (int)roomUp.roomRect.width) + (int)roomUp.roomRect.x;
        int stairUpY = Random.Range(0, (int)roomUp.roomRect.height) + (int)roomUp.roomRect.y;

        // Staircase down.

        Room roomDown = RandomRoom();

        int stairDownX = Random.Range(0, (int)roomDown.roomRect.width) + (int)roomDown.roomRect.x;
        int stairDownY = Random.Range(0, (int)roomDown.roomRect.height) + (int)roomDown.roomRect.y;

        m_terrainGrid[stairUpX, stairUpY] = TerrainType.StairsUp;
        m_terrainGrid[stairDownX, stairDownY] = TerrainType.StairsDown;
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a corridor to the logical grid.
    /// </summary>
    /// <param name="corridor">List of positions representing the corridor to add.</param>
    private void FillGrid(List<Vector2> corridors)
    {
        for (int i = 0; i < corridors.Count(); i++)
        {
            if (m_terrainGrid[(int)corridors[i].x, (int)corridors[i].y] != TerrainType.DoorWay)
                m_terrainGrid[(int)corridors[i].x, (int)corridors[i].y] = TerrainType.Corridor;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a door to the logical grid, that grid has to be placed on a wall.
    /// </summary>
    /// <param name="door">Position of the door .</param>
    private void FillGrid(Vector2 door)
    {
        m_terrainGrid[(int)door.x, (int)door.y] = TerrainType.DoorWay;
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a room with to the logical terrain grid.
    /// </summary>
    /// <param name="room">room to ad</param>
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
                if (m_terrainGrid[i, j] == TerrainType.None)
                    m_terrainGrid[i, j] = TerrainType.WallOuter;

        for (int i = abscissa - 1; i < abscissa + roomWidth + 1; i++)
            for (int j = ordinate - 1; j < ordinate + roomHeight + 1; j++)
                m_terrainGrid[i, j] = TerrainType.Wall;

        // Fill the actual room volume.
        for (int i = abscissa; i < abscissa + roomWidth; i++)
            for (int j = ordinate; j < ordinate + roomHeight; j++)
                m_terrainGrid[i, j] = TerrainType.Room;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void Clear()
    {
        m_terrainGrid = new TerrainType[width, height];

        // The level is created empty (aka. all tiles have no type).
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                m_terrainGrid[i, j] = TerrainType.None;

        m_roomList.Clear();
    }

    //-----------------------------------------------------------------------------------------------------------------

    public Room RandomRoom()
    {
        int rndIndex;
        if (m_roomList.Count > 0)
            rndIndex = Random.Range(0, m_roomList.Count);
        else
            return null;
        return m_roomList[rndIndex];
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void UpdateExplored(int x, int y)
    {
        //Start by making everything that was lit to the player explored.
        for (int i = 0; i < m_statusGrid.GetLength(0); i++)
        {
            for (int j = 0; j < m_statusGrid.GetLength(1); j++)
            {
                if (m_statusGrid[i, j] == StatusType.Lit)
                    m_statusGrid[i, j] = StatusType.Explored;
            }
        }

        // Then find which will be the tiles that are lit.
        Vector3 origin = new Vector3(x, 0, y);
        for (float abs = -1f; abs <= 1f; abs += 0.25f)
        {
            for (float ord = -1f; ord <= 1f; ord += 0.25f)
            {
                // Not a desirable direction.
                if (abs == 0 && ord == 0)
                    continue;

                Ray ray = new Ray(origin, new Vector3(abs, 0, ord));

                TraceRay(ray);
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void TraceRay(Ray ray)
    {
        float distance = 0;
        bool wallFound = false;
        bool isCorridor = false;

        int corridorCount = 0;

        while (!wallFound)
        {
            Vector3 currentPoint = ray.GetPoint(distance);

            if (m_terrainGrid[(int)currentPoint.x, (int)currentPoint.z] == TerrainType.Wall ||
               m_terrainGrid[(int)currentPoint.x, (int)currentPoint.z] == TerrainType.None ||
               m_terrainGrid[(int)currentPoint.x, (int)currentPoint.z] == TerrainType.WallOuter)
            {
                wallFound = true;
            }
            else if(m_terrainGrid[(int)currentPoint.x, (int)currentPoint.z] == TerrainType.Corridor)
            {
                isCorridor = true;
                corridorCount++;
            }
            else
            {
                corridorCount = 0;
            }
            // Corridors are only lit if they are next to the player
            if (isCorridor  && distance < 2)
                m_statusGrid[(int)currentPoint.x, (int)currentPoint.z] = StatusType.Lit;
            if (wallFound && corridorCount < 1)
                m_statusGrid[(int)currentPoint.x, (int)currentPoint.z] = StatusType.Lit;
            if (!isCorridor && ! wallFound)
                m_statusGrid[(int)currentPoint.x, (int)currentPoint.z] = StatusType.Lit;
            isCorridor = false;

            distance += 1f;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

}
