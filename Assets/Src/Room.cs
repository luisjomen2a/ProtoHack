using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int width = 0;
    public int height = 0;

    public int abscissa = 0;
    public int ordinate = 0;

    public List<DoorWay> doorWayList;

    public bool generated = false;

    private int m_levelWidth;
    private int m_levelHeight;


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

    public Room(int levelWidth, int levelHeight)
    {
        this.m_levelWidth = levelWidth;
        this.m_levelHeight = levelHeight;

        doorWayList = new List<DoorWay>();

        this.Generate();
    }
    /// <summary>
    /// Generates a room with random properteis (width, height, abscissa and ordinate).
    /// The sole purpose of this method is to avoid doing all this in the constructor tu avoid useless object creation.
    /// </summary>
    public void Generate()
    {
        width = Random.Range(3, 15);
        height = Random.Range(3, 11);
        abscissa = Random.Range(0, m_levelWidth);
        ordinate = Random.Range(0, m_levelHeight);
    }

    public void AddDoor(Vector2 pos)
    {
        doorWayList.Add(new DoorWay(pos, DoorStatusType.Empty));
    }

    public static string operator +(string s, Room a) => s + "\n x : " + a.abscissa + ", y : " + a.ordinate 
                                                           + ", w : " + a.width + ", h : " + a.width;
}
