using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int width = 80;
    public int height = 20;

    private GameObject[,] m_floor;
    private TerrainGrid m_logicGrid;

    private GameObject m_roomPrefab;
    private GameObject m_wallPrefab;

    public bool generated = false;

    // Start is called before the first frame update
    public void Start()
    {
        m_roomPrefab = Resources.Load("Prefabs/tile") as GameObject;
        m_wallPrefab = Resources.Load("Prefabs/wall") as GameObject;

        Renderer renderer = m_roomPrefab.GetComponent(typeof(Renderer)) as Renderer;

        Material roomMaterial = Resources.Load("Materials/NoneMaterial") as Material;

        renderer.material = roomMaterial;

        m_logicGrid = new TerrainGrid(width, height);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Generate()
    {
        if (this.generated)
            this.Clear(true);

        m_logicGrid.GenerateRooms();
        m_logicGrid.GenerateDoors();
        m_logicGrid.GenerateCorridors();

        this.Render();
        
        this.generated = true;
    }

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
                Destroy(m_floor[i, j]);
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    private void Render()
    {
        m_floor = new GameObject[width, height];

        //Instatiate objects on the whole level.
        for (int i = 0; i < m_floor.GetLength(0); i++)
        {
            for (int j = 0; j < m_floor.GetLength(1); j++)
            {
                if (m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.Room ||
                    m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.DoorWay ||
                    m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.Corridor
                    )
                {
                    m_floor[i, j] = Instantiate(m_roomPrefab, new Vector3(i, 0, j), Quaternion.identity);

                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial = Resources.Load("Materials/RoomMaterial") as Material;
                    renderer.material = roomMaterial;
                }
                else if (m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.Wall)
                {
                    m_floor[i, j] = Instantiate(m_wallPrefab, new Vector3(i, 2, j), Quaternion.identity);
                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial = Resources.Load("Materials/WallMaterial") as Material;
                    renderer.material = roomMaterial;
                }
                else
                {
                    m_floor[i, j] = Instantiate(m_wallPrefab, new Vector3(i, 2, j), Quaternion.identity);
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
            rndAbs = Random.Range((int)rndRoom.roomRect.x, (int)rndRoom.roomRect.x + (int)rndRoom.roomRect.width);
            rndOrd = Random.Range((int)rndRoom.roomRect.x, (int)rndRoom.roomRect.y+ (int)rndRoom.roomRect.height);
        }
        return new Vector2(rndAbs, rndOrd);
    }
}
