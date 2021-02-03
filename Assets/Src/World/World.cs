using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private Level m_level;

    private bool m_isGenerated = false;

    // Start is called before the first frame update
    void Start()
    {
        m_level = gameObject.AddComponent(typeof(Level)) as Level;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {   
            m_level.Generate();
            m_isGenerated = true;
        }
        if (Input.GetKeyUp(KeyCode.R))
            m_level.Clear(true);
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.F))
            m_level.Reveal();
    }

    //-----------------------------------------------------------------------------------------------------------------

    public Vector2 PlacePlayer()
    {
        Vector2 rndPos = m_level.GetRandomRoomPositon();
        return rndPos;
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if the given coordinate is usable for walking or not.
    /// </summary>
    /// <returns>False if the given coordinate corresponds to a Wall, or stone or etc.. True if not.</returns>
    public bool WalkableAt(int x, int j)
    {
        return m_level.WalkableAt(x, j);
    }

    //-----------------------------------------------------------------------------------------------------------------
    public void UpdateExplored(int x, int j)
    {
        m_level.UpdateExplored(x, j);
    }

    //-----------------------------------------------------------------------------------------------------------------
    public bool IsGenerated()
    {
        return m_isGenerated;
    }

    //-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///  Calls the open door method at the current level at the given tile.
    /// </summary>
    /// <param name="x">abscissa of the tile of intrest. </param>
    /// <param name="y">ordinate of the tile of intrest.</param>
    /// <returns>Status that the tile had while the attempt was made.</returns>
    public int OpenAt(int x, int y)
    {
        return m_level.OpenAt(x, y);
    }
}
