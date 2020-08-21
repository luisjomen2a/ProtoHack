﻿using System.Collections; 
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

    public bool generated = false;

    // Start is called before the first frame update
    public void Start()
    {
        m_tilePrefab = Resources.Load("Prefabs/tile") as GameObject;
        m_wallPrefab = Resources.Load("Prefabs/wall") as GameObject;

        Renderer renderer = m_tilePrefab.GetComponent(typeof(Renderer)) as Renderer;

        Material roomMaterial = Resources.Load("Materials/NoneMaterial") as Material;

        renderer.material = roomMaterial;

        m_logicGrid = new TerrainGrid(width, height);
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void Generate()
    {
        if (this.generated)
            this.Clear(true);

        m_logicGrid.GenerateRooms();
        m_logicGrid.GenerateCorridors();
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
    private void Render()
    {
        m_floor = new GameObject[width, height];

        //Instatiate objects on the whole level.
        for (int i = 0; i < m_floor.GetLength(0); i++)
        {
            for (int j = 0; j < m_floor.GetLength(1); j++)
            {
                if(m_logicGrid.GetStatusAt(i, j) == TerrainGrid.StatusType.Unexplored || 
                   m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.None)
                {
                    m_floor[i, j] = Instantiate(m_wallPrefab, new Vector3(i, 2, j), Quaternion.identity);
                }
                else if (m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.Room ||
                         m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.DoorWay ||
                         m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.Corridor
                    )
                {
                    m_floor[i, j] = Instantiate(m_tilePrefab, new Vector3(i, 0, j), Quaternion.identity);

                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial;
                    if(m_logicGrid.GetStatusAt(i, j) == TerrainGrid.StatusType.Lit)
                        roomMaterial = Resources.Load("Materials/RoomMatLit") as Material;
                    else
                        roomMaterial = Resources.Load("Materials/RoomMat") as Material;
                    renderer.material = roomMaterial;
                }
                else if(m_logicGrid.GetTerrainAt(i, j) == TerrainGrid.TerrainType.DoorWay)
                {
                    m_floor[i, j] = Instantiate(m_wallPrefab, new Vector3(i, 2, j), Quaternion.identity);

                    Renderer renderer = m_floor[i, j].GetComponent(typeof(Renderer)) as Renderer;

                    Material roomMaterial = Resources.Load("Materials/TestMat") as Material;
                    renderer.material = roomMaterial;
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
            rndAbs = Random.Range(0, (int)rndRoom.roomRect.width) + (int)rndRoom.roomRect.x;
            rndOrd = Random.Range(0, (int)rndRoom.roomRect.height) + (int)rndRoom.roomRect.y;

            Debug.Log("rndAbs " + rndAbs + " rndOrd " + rndOrd);
        }
        return new Vector2(rndAbs, rndOrd);
    }

    //-----------------------------------------------------------------------------------------------------------------

    public bool WalkableAt(int x, int y)
    {
        return m_logicGrid.GetTerrainAt(x, y) != TerrainGrid.TerrainType.Wall &&
            m_logicGrid.GetTerrainAt(x, y) != TerrainGrid.TerrainType.WallOuter &&
            m_logicGrid.GetTerrainAt(x, y) != TerrainGrid.TerrainType.None;
    }

    //-----------------------------------------------------------------------------------------------------------------

    public void UpdateExplored(int x, int y)
    {
        m_logicGrid.UpdateExplored(x, y);
        Clear();
        Render();
    }

    //-----------------------------------------------------------------------------------------------------------------
}
