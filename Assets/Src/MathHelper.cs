using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RnG
{
    static public bool PassTest(int favOutcomes, int totalOutcomes)
    {
        int rndRes = Random.Range(0, totalOutcomes);
        return rndRes < favOutcomes;
    }
}
public class Dijkstra
{
    private Node[,] m_nodeGrid;
    private List<Node> m_unexploredNodes = new List<Node>();
    /// <summary>
    /// Creates the grid of nodes and the unexplplored node list.
    /// </summary>
    /// <param name="nodeValues">Contains the actual node values. Used for neighbour and obstacle extracting.</param>
    public Dijkstra(int[,] nodeValues)
    {
        m_nodeGrid = new Node[nodeValues.GetLength(0), nodeValues.GetLength(1)];
        for (int i = 0; i < nodeValues.GetLength(0); i ++)
        {
            for (int j = 0; j < nodeValues.GetLength(1); j++)
            {
                if(nodeValues[i,j] != -1)
                {
                    List<Vector2> neighbours = new List<Vector2>();

                    // Test all four neighbours for map limits for neighbours that don't need to be added.
                    if(i != 0) // Bottom.
                        neighbours.Add(new Vector2(i-1, j));
                    if (i != nodeValues.GetLength(0) - 1) // Top.
                        neighbours.Add(new Vector2(i+1, j));
                    if (j != 0) // Left.
                        neighbours.Add(new Vector2(i, j - 1));
                    if (j != nodeValues.GetLength(1) - 1) // Right.
                        neighbours.Add(new Vector2(i, j + 1));

                    Node newNode = new Node(new Vector2(i, j), neighbours);

                    if (nodeValues[i, j] != -1)
                    {
                        newNode.walkable = true;
                        m_unexploredNodes.Add(newNode);
                    }

                    m_nodeGrid[i, j] = newNode;
                }
            }
        }
    }

    /// <summary>
    /// Run a basic Disjkstra using a discrete 4-neighbour grid.
    /// </summary>
    /// <param name="startPoint">Point from which the iteration wiil start. Weight = 0</param>
    /// <param name="endPoint">Point on which the iteration wiil end. Weight = n, where n is the smallest weight possible</param>
    
    /// <returns></returns>
    public List<Vector2> Run(Vector2 startPoint, Vector2 endPoint)
    {
        double startTime = Time.realtimeSinceStartup;
        // The first node is considered the only one explored, it's the iteration's entry point.
        Node startNode = m_unexploredNodes.Find(x => x.position.x == startPoint.x && x.position.y == startPoint.y);

        startNode.weight = 0;

        while(m_unexploredNodes.Count > 0)
        {
            // Sorting the unexplored nodes list by weight ensures we always choose the closes node to the start.
            m_unexploredNodes.Sort((x, y) => x.weight.CompareTo(y.weight));

            Node current = m_unexploredNodes[0];    
            m_unexploredNodes.Remove(current);

            // Add to all our neighbours our current node as a parent only if our weight is small enough.
            foreach( Vector2 neighbour in current.neighbours)
            {
                Node nghbNode = m_nodeGrid[(int)neighbour.x, (int)neighbour.y];
                if( m_unexploredNodes.Contains(nghbNode) && nghbNode.walkable )
                {
                    float distance = current.weight + 1;

                    if (distance < nghbNode.weight)
                    {
                        // We update the new distance as weight and update the new path now.
                        nghbNode.weight = distance;
                        if(nghbNode.position == endPoint)
                        {
                            m_unexploredNodes.Clear();
                        }
                        nghbNode.parentNode = new Vector2((int)current.position.x, (int)current.position.y);
                    }
                }
            }
        }

        // Return an actual comprehensible list of positions.
        List<Vector2> result = new List<Vector2>();
        Node node = m_nodeGrid[(int)endPoint.x, (int)endPoint.y];

        // While there's still a previous node, we continue.
        while (node.parentNode.x != -1 && node.parentNode.y != -1)
        {
            result.Add(new Vector2(node.position.x, node.position.y));

            node = m_nodeGrid[(int)node.parentNode.x, (int)node.parentNode.y];
        }

        // Reverse the list so that it will be from start to end.
        result.Reverse();

        double endTime = (Time.realtimeSinceStartup - startTime);
        Debug.Log("Compute time: " + endTime);

        Debug.Log("Path completed!");

        return result;
    }
}

public class Node
{
    public Vector2 position;
    public Vector2 parentNode = new Vector2(-1, -1);
    public List<Vector2> neighbours;
    public bool walkable = false;
    public float weight=float.MaxValue;

    public Node(Vector2 position, List<Vector2> neighbours)
    {
        this.position = position;
        this.neighbours = new List<Vector2>(neighbours);
    }
}