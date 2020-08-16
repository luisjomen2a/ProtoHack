using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject m_playerPrefab;

    private bool m_generated = false;

    public Vector2 position;

    public void Place(float x, float y)
    {
        position.x = x;
        position.y = y;

        if(!m_generated)
        {
            m_playerPrefab = Resources.Load("Prefabs/player") as GameObject;

            m_playerPrefab = Instantiate(m_playerPrefab, new Vector3(position.x, 1, position.y), Quaternion.identity);

            Renderer renderer = m_playerPrefab.GetComponent(typeof(Renderer)) as Renderer;

            Material roomMaterial = Resources.Load("Materials/PlayerMat") as Material;
            renderer.material = roomMaterial;

            m_generated = true;
        }
        else
        {
            m_playerPrefab.transform.position = new Vector3(position.x, 1, position.y) ;
        }
    }
}
