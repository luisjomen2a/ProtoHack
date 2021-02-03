using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int width = 80;
    public int height = 20;

    private GameObject[,] m_floor;
    private TerrainGrid m_logicGrid;
    public Light m_playerLight;

    private GameObject m_tilePrefab;
    private GameObject m_wallPrefab;
    private GameObject m_doorOpenPrefab;
    private GameObject m_doorClosedPrefab;

    public bool generated = false;

    // Start is called before the first frame update
    public void Start()
    {
        m_tilePrefab = Resources.Load("Prefabs/tile") as GameObject;
        m_wallPrefab = Resources.Load("Prefabs/wall") as GameObject;
        m_doorOpenPrefab = Resources.Load("Prefabs/doorOpen") as GameObject;
        m_doorClosedPrefab = Resources.Load("Prefabs/doorClosed") as GameObject;

        m_logicGrid = new TerrainGrid(width, height);
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void Generate()
    {
        if (this.generated)
            this.Clear(true);

        m_logicGrid.GenerateRooms();
        m_logicGrid.GenerateCorridors();
        m_logicGrid.GenerateNiches();
        m_logicGrid.GenerateStairs();

        Render();

        //m_logicGrid.Print();        
        generated = true;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void Clear(bool reset = false)
    {
        if(reset)
        {
            m_logicGrid.Clear();
        }
        for (int i = 0; i < m_floor.GetLength(0); i++)
        {
            for (int j = 0; j < m_floor.GetLength(1); j++)
            {
                Destroy(m_floor[i, j].gameObject);
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// This Renders the whole level, adapting each gameObject to the type of terrain.
    /// </summary>
    private void Render()
    {
        m_floor = new GameObject[width, height];

        //Instatiate objects on the whole level.
        for (int i = 0; i < m_floor.GetLength(0); i++)
        {
            for (int j = 0; j < m_floor.GetLength(1); j++)
            {
                if(m_logicGrid.GetStatusAt(i, j) == TerrainGrid.LightStatusType.Unexplored || 
                   m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.None)
                {
                    m_floor[i, j] = Instantiate(m_wallPrefab, new Vector3(i, 2, j), Quaternion.identity);
                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial = Resources.Load("Materials/NoneMat") as Material;
                    renderer.material = roomMaterial;
                }
                else if (m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.Room ||
                         m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.Corridor
                    )
                {
                    m_floor[i, j] = Instantiate(m_tilePrefab, new Vector3(i, 0, j), Quaternion.identity);

                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial;
                    if(m_logicGrid.GetStatusAt(i, j) == TerrainGrid.LightStatusType.Lit)
                        roomMaterial = Resources.Load("Materials/RoomMatLit") as Material;
                    else
                        roomMaterial = Resources.Load("Materials/RoomMat") as Material;
                    renderer.material = roomMaterial;
                }
                else if(m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.DoorWay)
                { 
                    float rotY = 0;
                    //Find which way is the doorway facing. ( We are certain that we can't exceed levels bounds so no
                    // testing is needed.
                    if(m_logicGrid.GetTerrainAt(i, j-1) == TerrainGrid.TerrainType.Wall)
                    {
                        rotY = 90;
                    }

                    Material roomMaterial;
                    if (m_logicGrid.GetDoorwayStatusAt(i,j) == Room.DoorStatusType.Closed ||
                       m_logicGrid.GetDoorwayStatusAt(i, j) == Room.DoorStatusType.Locked)
                    {
                        m_floor[i, j] = Instantiate(m_doorClosedPrefab, new Vector3(i, 2, j), Quaternion.Euler(0, rotY, 0));
                        Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;
                        roomMaterial = Resources.Load("Materials/DoorMat") as Material;
                        renderer.material = roomMaterial;
                    }
                    else if (m_logicGrid.GetDoorwayStatusAt(i, j) == Room.DoorStatusType.Open)
                    {
                        m_floor[i, j] = Instantiate(m_doorOpenPrefab, new Vector3(i, 2, j), Quaternion.Euler(0, rotY, 0));
                        Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;
                        roomMaterial = Resources.Load("Materials/DoorMat") as Material;
                        renderer.material = roomMaterial;
                    }
                    else if (m_logicGrid.GetDoorwayStatusAt(i, j) == Room.DoorStatusType.Hidden) // Hidden doors are just treated as Walls
                    {
                        m_floor[i, j] = Instantiate(m_wallPrefab, new Vector3(i, 2, j), Quaternion.identity);
                        Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                        roomMaterial = Resources.Load("Materials/StairsDownMat") as Material;
                        renderer.material = roomMaterial;
                    }
                    else 
                    {
                        m_floor[i, j] = Instantiate(m_tilePrefab, new Vector3(i, 0, j), Quaternion.identity);
                        Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;
                        if (m_logicGrid.GetStatusAt(i, j) == TerrainGrid.LightStatusType.Lit)
                            roomMaterial = Resources.Load("Materials/RoomMatLit") as Material;
                        else
                            roomMaterial = Resources.Load("Materials/RoomMat") as Material;

                        renderer.material = roomMaterial;
                    }
                    
                }
                else if (m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.Wall)
                {
                    m_floor[i, j] = Instantiate(m_wallPrefab, new Vector3(i, 2, j), Quaternion.identity);
                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial = Resources.Load("Materials/WallMat") as Material;
                    renderer.material = roomMaterial;
                }
                else if (m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.StairsDown)
                {
                    m_floor[i, j] = Instantiate(m_tilePrefab, new Vector3(i, 0, j), Quaternion.identity);
                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial = Resources.Load("Materials/StairsDownMat") as Material;
                    renderer.material = roomMaterial;
                }
                else if (m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.StairsUp)
                {
                    m_floor[i, j] = Instantiate(m_tilePrefab, new Vector3(i, 0, j), Quaternion.identity);
                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial = Resources.Load("Materials/StairsUpMat") as Material;
                    renderer.material = roomMaterial;
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    public Vector2 GetRandomRoomPositon()
    {
        Room rndRoom = m_logicGrid.RandomRoom();
        int rndAbs = -1;
        int rndOrd = -1;
        if (rndRoom != null)
        {
            rndAbs = Random.Range(0, (int)rndRoom.m_roomRect.width) + (int)rndRoom.m_roomRect.x;
            rndOrd = Random.Range(0, (int)rndRoom.m_roomRect.height) + (int)rndRoom.m_roomRect.y;

            Debug.Log("rndAbs " + rndAbs + " rndOrd " + rndOrd);
        }
        return new Vector2(rndAbs, rndOrd);
    }

    //-----------------------------------------------------------------------------------------------------------------

    public bool WalkableAt(int x, int y)
    {
        bool isNotWall = m_logicGrid.GetTerrainAt(x, y) != TerrainGrid.TerrainType.Wall &&
            m_logicGrid.GetTerrainAt(x, y) != TerrainGrid.TerrainType.WallOuter &&
            m_logicGrid.GetTerrainAt(x, y) != TerrainGrid.TerrainType.None;

        bool isNotAClosedDoor = m_logicGrid.GetTerrainAt(x, y) != TerrainGrid.TerrainType.DoorWay ||
                (m_logicGrid.GetDoorwayStatusAt(x, y) != Room.DoorStatusType.Closed &&
                m_logicGrid.GetDoorwayStatusAt(x, y) != Room.DoorStatusType.Locked &&
                m_logicGrid.GetDoorwayStatusAt(x, y) != Room.DoorStatusType.Hidden);

        return isNotWall && isNotAClosedDoor;
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///  Calls the open door method at the current level at the given tile. If the door was closed, it opens it. 
    /// </summary>
    /// <param name="x">abscissa of the tile of intrest. </param>
    /// <param name="y">ordinate of the tile of intrest.</param>
    /// <returns>Status that the tile had while the attempt was made.</returns>
    public int OpenAt(int x, int y)
    {
        // Only closed and locked doors are available for openning.
        if (m_logicGrid.GetTerrainAt(x, y) != TerrainGrid.TerrainType.DoorWay)
            return -1;
        else if (m_logicGrid.GetDoorwayStatusAt(x, y) == Room.DoorStatusType.Closed)
        { 
            m_logicGrid.OpenAt(x, y);
            return (int)Room.DoorStatusType.Closed;
        }
        
        return (int)m_logicGrid.GetDoorwayStatusAt(x, y);
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void UpdateExplored(int x, int y)
    {
        m_logicGrid.UpdateExplored(x, y);
        Clear();
        Render();
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void Reveal()
    {
        m_logicGrid.Reveal();
        Clear();
        Render();
    }

    //-----------------------------------------------------------------------------------------------------------------
}
