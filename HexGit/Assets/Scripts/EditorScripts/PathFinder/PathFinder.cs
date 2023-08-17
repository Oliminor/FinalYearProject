using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private PathNode[,,] pathArray;

    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private Vector2Int xSize;
    [SerializeField] private Vector2Int ySize;
    [SerializeField] private Vector2Int zSize;

    [SerializeField] private float wallAngleLimit;
    [SerializeField] private float slopeAngleLimit;

    [SerializeField] private int SizeBetweenNodes;

    [SerializeField] private Transform startPosTest;
    [SerializeField] private Transform endPosTest;
    [SerializeField] private Transform checkPositionsParent;

    [SerializeField][HideInInspector] private List<Vector3Int> saveVector = new();
    [SerializeField][HideInInspector] private List<PathNode> saveNode = new();
    [SerializeField][HideInInspector] private List<GameObject> cubeList = new();

    private List<Transform> checkPositions = new();

    private LineRenderer lr;

    private int mapSizeX;
    private int mapSizeY;
    private int mapSizeZ;

    public void Initailaze()
    {
        lr = GetComponent<LineRenderer>();

        mapSizeX = Mathf.Abs(xSize.x) + xSize.y;
        mapSizeY = Mathf.Abs(ySize.x) + ySize.y;
        mapSizeZ = Mathf.Abs(zSize.x) + zSize.y;
    }

    
    private void OnEnable()
    {
        Initailaze();
        LoadArray();
    }

    private void OnValidate()
    {
        RefreshColliderSize();
    }

    /// <summary>
    /// Instantiate the Node elements in a 3D grid
    /// </summary>
    public void InstantiateGrid()
    {
        Initailaze();

        pathArray = new PathNode[mapSizeX, mapSizeY, mapSizeZ];

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                for (int k = 0; k < mapSizeZ; k++)
                {
                    // Set the spacing between grid elements
                    int posX = (i - Mathf.Abs(xSize.x)) * SizeBetweenNodes;
                    int posY = (j - Mathf.Abs(ySize.x)) * SizeBetweenNodes;
                    int posZ = (k - Mathf.Abs(zSize.x)) * SizeBetweenNodes;

                    Vector3 position = new Vector3(posX, posY, posZ);

                    bool isGroundThere;
                    float slopeAngle;

                    // Checks if the ground is walkable or not
                    CheckGround(position, out isGroundThere, out slopeAngle);

                    bool isReachable = false;

                    // Checks if the angle under the node element is walkable or is there a wall nearby
                    if (isGroundThere && slopeAngle < slopeAngleLimit && !CheckWall(position)) isReachable = true;

                    pathArray[i, j, k] = new PathNode();

                    pathArray[i, j, k].position = new Vector3(i, j, k);
                    pathArray[i, j, k].isReachable = isReachable;
                }
            }
        }

        // Counter to to check the reachable Node numbers
        int counter = 0;

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                for (int k = 0; k < mapSizeZ; k++)
                {
                    if (pathArray[i, j, k].isReachable) counter++;
                }
            }
        }

        Debug.Log("Reachable Nodes: " + counter);

        DeleteUnreachableNodes();
    }

    /// <summary>
    /// Loads in the check positions for the DeleteUnreachableNodes function
    /// </summary>
    private void LoadCheckPositions()
    {
        checkPositions.Clear();

        for (int i = 0; i < checkPositionsParent.childCount; i++)
        {
            checkPositions.Add(checkPositionsParent.GetChild(i));
        }
    }

    /// <summary>
    /// Deletes the unreachable nodes from the scene
    /// </summary>
    private void DeleteUnreachableNodes()
    {
        LoadCheckPositions();

        // This algorithm do a secondary cleanup to delete the unreachable nodes (such us under other models)
        // Check Positions are the starting point of the algorithm
        // Multiple check Points are avaliable in case there separate islands and such
        foreach (var item in checkPositions)
        {
            Vector3Int startPos = WorldToGrid(GroundPosition(item.position));

            List<Vector3> nodeList = new();

            Vector3Int currentPos = startPos;

            nodeList.Add(currentPos);
            
            while (true)
            {
                if (nodeList.Count == 0) break;

                currentPos = new Vector3Int((int)nodeList[0].x, (int)nodeList[0].y, (int)nodeList[0].z);
                pathArray[currentPos.x, currentPos.y, currentPos.z].isOpen = true;
                nodeList.RemoveAt(0);

                for (int l = currentPos.x - 1; l <= currentPos.x + 1; l++)
                {
                    for (int m = currentPos.y - 1; m <= currentPos.y + 1; m++)
                    {
                        for (int n = currentPos.z - 1; n <= currentPos.z + 1; n++)
                        {
                            if (l < 0 || l > mapSizeX - 1) continue;
                            if (m < 0 || m > mapSizeY - 1) continue;
                            if (n < 0 || n > mapSizeZ - 1) continue;

                            if (!pathArray[l, m, n].isReachable) continue;

                            if (pathArray[l, m, n].isOpen) continue;

                            // If there is a wall between the current node and the neighbour, that node is switched off
                            if (CheckWallBetweenTwoNodes(GridToWorld(currentPos), GridToWorld(pathArray[l, m, n].position)))
                            {
                                pathArray[l, m, n].isOpen = false;
                                continue;
                            }

                            nodeList.Insert(0, pathArray[l, m, n].position);
                        }
                    }
                }
            }
        }

        // Another counter to check the final result
        int counter = 0;

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                for (int k = 0; k < mapSizeZ; k++)
                {
                    if (!pathArray[i, j, k].isOpen)
                    {
                        pathArray[i, j, k].isReachable = false;
                    }

                    if (pathArray[i, j, k].isReachable)
                    {
                        counter++;
                    }
                }
            }
        }

        SaveArray();

        Debug.Log("Cleaned Nodes: " + counter);
    }

    /// <summary>
    /// Saves the array, because 3D arrays are not serializable by default (Using 2 separate list for the Node and the Position)
    /// </summary>
    private void SaveArray()
    {
        saveNode.Clear();
        saveVector.Clear();

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                for (int k = 0; k < mapSizeZ; k++)
                {
                    if (pathArray[i, j, k].isReachable)
                    {
                        saveNode.Add(pathArray[i, j, k]);
                        saveVector.Add(new Vector3Int(i, j, k));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Loads the array if needed
    /// </summary>
    private void LoadArray()
    {
        pathArray = new PathNode[mapSizeX, mapSizeY, mapSizeZ];

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                for (int k = 0; k < mapSizeZ; k++)
                {
                    pathArray[i,j,k] = new PathNode();
                }
            }
        }

        for (int i = 0; i < saveVector.Count; i++)
        {
            pathArray[saveVector[i].x, saveVector[i].y, saveVector[i].z] = saveNode[i];
        }
    }

    /// <summary>
    /// Raycast check if there is a wall(other ground object) between 2 nodes or not
    /// </summary>
    private bool CheckWallBetweenTwoNodes(Vector3 _posA, Vector3 _posB)
    {
        float distance = Vector3.Distance(_posA, _posB);
        Vector3 dir = _posB - _posA;

        if (Physics.Raycast(_posA, dir, out RaycastHit hitinfo, distance, whatIsGround))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Raycast check if there is a ground under the certan position and the slop angle of it (UPDATE)
    /// </summary>
    public void CheckGround(Vector3 _pos, out bool isTrue, out float slopeAngle)
    {
        if (Physics.Raycast(_pos, Vector3.down, out RaycastHit hitinfo, SizeBetweenNodes, whatIsGround))
        {
            isTrue = true;
            slopeAngle = Vector3.Angle(transform.up, hitinfo.normal);
            return;
        }

        slopeAngle = 0;
        isTrue = false;
    }

    /// <summary>
    /// Nodes position are grid based, this function set those position to the ground (mainly visual reasons)
    /// </summary>
    private Vector3 GroundPosition(Vector3 _pos)
    {
        if (Physics.Raycast(_pos, Vector3.down, out RaycastHit hitinfo, Mathf.Infinity, whatIsGround))
        {
            return hitinfo.point + transform.up * 0.5f;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Wall check for all 4 direction from the node
    /// </summary>
    public bool CheckWall(Vector3 _pos)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 dir = Vector3.zero;

            switch (i)
            {
                case 0: dir = Vector3.right; break;
                case 1: dir = Vector3.left; break;
                case 2: dir = Vector3.forward; break;
                case 3: dir = Vector3.back; break;

            }

            if (Physics.Raycast(_pos, dir, out RaycastHit hitinfo, SizeBetweenNodes, whatIsGround))
            {
                if (Vector3.Angle(transform.up, hitinfo.normal) > wallAngleLimit) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Instantiate cube primitives to visualize the nodes on the game world
    /// </summary>
    public void GenerateNodeCubes()
    {
        if (pathArray == null) LoadArray();

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                for (int k = 0; k < mapSizeZ; k++)
                {
                    if (pathArray[i, j, k].isReachable) InstantiatePrimitives(GridToWorld(new Vector3(i, j, k)), PrimitiveType.Cube);
                }
            }
        }
    }

    /// <summary>
    /// Delete the visualize nodes primitives
    /// </summary>
    public void DeletePrimitives()
    {
        foreach (var item in cubeList) DestroyImmediate(item.gameObject);
        cubeList.Clear();
    }

    /// <summary>
    /// Funtion to instantiate any PrimitiveType object
    /// </summary>
    public void InstantiatePrimitives(Vector3 _pos, PrimitiveType _type)
    {
        GameObject obj = GameObject.CreatePrimitive(_type);
        if (obj.TryGetComponent(out Collider col)) DestroyImmediate(col);
        obj.transform.position = _pos;
        obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        obj.GetComponent<MeshRenderer>().sharedMaterial.color = Color.cyan;

        cubeList.Add(obj);
    }

    /// <summary>
    /// Refresh the collder size, which is a visual box collider to visualize the bake area for the nodes
    /// </summary>
    public void RefreshColliderSize()
    {
        Initailaze();

        int centerX = xSize.x + xSize.y;
        int centerY = ySize.x + ySize.y;
        int centerZ = zSize.x + zSize.y;

        if (TryGetComponent(out BoxCollider box))
        {
            box.size = new Vector3(mapSizeX, mapSizeY, mapSizeZ) * SizeBetweenNodes;
            box.center = new Vector3(centerX, centerY, centerZ) * SizeBetweenNodes / 2;
        }
    }

    /// <summary>
    /// Testing the Astar pathfinder output drawing out LineRenderer between the result nodes
    /// </summary>
    public void AstarTest()
    {
        DrawLine(AStar(startPosTest.position, endPosTest.position));
    }

    /// <summary>
    /// Erase the LineRenderer
    /// </summary>
    public void EraseLine()
    {
        lr.enabled = false;
    }

    /// <summary>
    /// Draw LineRenderer between nodes
    /// </summary>
    public void DrawLine(List<Vector3> nodeList)
    {
        lr.enabled = true;

        if (nodeList == null) return;

        for (int i = 0; i < nodeList.Count; i++)
        {
            nodeList[i] = GroundPosition(nodeList[i]);
        }

        lr.positionCount = 0;
        lr.loop = false;

        Vector3[] linePos = nodeList.ToArray();

        lr.positionCount = linePos.Length;
        lr.SetPositions(linePos);
    }

    /// <summary>
    /// Bezier curve calculation (Not in use)
    /// </summary>
    private Vector3 CreateBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
    }

    /// <summary>
    /// Convert the world position to the grid position (offset, spacing and such)
    /// </summary>
    private Vector3Int WorldToGrid(Vector3 _position)
    {
        float posX = (int)_position.x - (int)_position.x % SizeBetweenNodes + ((int)_position.x % SizeBetweenNodes == 0 ? 0 : SizeBetweenNodes);
        float posY = (int)_position.y - (int)_position.y % SizeBetweenNodes + ((int)_position.y % SizeBetweenNodes == 0 ? 0 : SizeBetweenNodes);
        float posZ = (int)_position.z - (int)_position.z % SizeBetweenNodes + ((int)_position.z % SizeBetweenNodes == 0 ? 0 : SizeBetweenNodes);

        _position = new Vector3(posX, posY, posZ);

        Vector3Int finalV = new Vector3Int((int)_position.x / SizeBetweenNodes + Mathf.Abs(xSize.x), (int)_position.y / SizeBetweenNodes + Mathf.Abs(ySize.x), (int)_position.z / SizeBetweenNodes + Mathf.Abs(zSize.x));

        if (pathArray[finalV.x, finalV.y, finalV.z].isReachable) return finalV;

        // Checks if the neighbour nodes are avaliable
        for (int i = finalV.x - 1; i <= finalV.x + 1; i++)
        {
            for (int j = finalV.y; j <= finalV.y + 1; j++)
            {
                for (int k = finalV.z - 1; k <= finalV.z + 1; k++)
                {
                    if (pathArray[i, j, k].isReachable) return new Vector3Int(i,j,k);
                }
            }
        }

        return Vector3Int.zero;
    }

    // Converts back the from Grid position to World Position
    private Vector3 GridToWorld(Vector3 _position)
    {
        _position = new Vector3((_position.x - Mathf.Abs(xSize.x)) * SizeBetweenNodes, (_position.y - Mathf.Abs(ySize.x)) * SizeBetweenNodes, (_position.z - Mathf.Abs(zSize.x)) * SizeBetweenNodes);

        return _position;
    }

    /// <summary>
    /// Astar Pathfind calculation between 2 points
    /// </summary>
    public List<Vector3> AStar(Vector3 _startPos, Vector3 _endPos)
    {
        List<Vector3> pathNodes = new();

        if (pathArray == null) LoadArray();

        _startPos = GroundPosition(_startPos);
        _endPos = GroundPosition(_endPos);

        Vector3Int startPos = WorldToGrid(_startPos);
        Vector3Int endPos = WorldToGrid(_endPos);

        if (startPos == endPos) return pathNodes;
        if (startPos == Vector3Int.zero || endPos == Vector3Int.zero) return pathNodes;

        List<PathNode> openList = new();
        HashSet<PathNode> closedList = new();

        PathNode startNode = new PathNode();

        startNode.position = startPos;
        startNode.parent = startPos;

        openList.Add(startNode);

        int counter = 0;

        while (true)
        {
            // if the F cost list empty, thats mean the algorithm checked every available nodes
            // so return an empty list (thats mean new end position needed)
            // usually happens if the end position is on an "island"  surrounded  by walls
            if (openList.Count == 0)
            {
                Debug.Log("empty");
                pathNodes.Clear();
                return pathNodes;
            }

            // Searching for the smallest F cost node
            //checkFCost = checkFCost.OrderBy(ch => ch.fCost).ToList();

            int smallest = openList[0].fCost;
            int index = 0;

            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].fCost < smallest)
                {
                    smallest = openList[i].fCost;
                    index = i;
                }
            }

            PathNode currentNode = openList[index];
            openList.RemoveAt(index);
            closedList.Add(currentNode);

            // If we reached the end position
            if (currentNode.position == endPos)
            {
                //Debug.Log("End " + counter);

                while (currentNode.position != startPos)
                {
                    pathNodes.Insert(0, GridToWorld(currentNode.position));
                    currentNode = pathArray[(int)currentNode.parent.x, (int)currentNode.parent.y, (int)currentNode.parent.z];
                }

                return pathNodes;
            }

            // Set x, y, z to our current lowest final cost Node
            int x = (int)currentNode.position.x;
            int y = (int)currentNode.position.y;
            int z = (int)currentNode.position.z;

            // Checks the neighbour nodes
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    for (int k = z - 1; k <= z + 1; k++)
                    {
                        if (i < 0 || i > mapSizeX - 1) continue;
                        if (j < 0 || j > mapSizeY - 1) continue;
                        if (k < 0 || k > mapSizeZ - 1) continue;

                        if (!pathArray[i, j, k].isReachable) continue;

                        if (currentNode.position == new Vector3(i, j, k)) continue;

                        PathNode node = pathArray[i, j, k];

                        // Distance between the current node and the neighbour
                        int distance = EuclideanDistanceHeuristicDistance(new Vector3(x, y, z), new Vector3(i, j, k));

                        // This section refreshes the cost for the already open neighbours 
                        if (closedList.Contains(node)) continue;

                        counter++;

                        if (openList.Contains(node)) continue;

                        // This section checks the unchecked new neighbour nodes
                        int h = (int)Vector3.Distance(endPos, new Vector3(i, j, k));

                        node.gCost = currentNode.gCost + distance;
                        node.hCost = h;
                        node.fCost = h + currentNode.gCost + distance;
                        node.parent = currentNode.position;

                        openList.Add(node);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Euclidean distance calculation
    /// </summary>
    private int EuclideanDistanceHeuristicDistance(Vector3 _posA, Vector3 _posB)
    {
        float nx = Mathf.Abs(_posA.x - _posB.x);
        float ny = Mathf.Abs(_posA.y - _posB.y);
        float nz = Mathf.Abs(_posA.z - _posB.z);

        float nd = Mathf.Min(nx, ny) * 14 + Mathf.Abs(nx - ny) * 10;
        nd += Mathf.Min(nd, nz) * 14 + Mathf.Abs(nd - nz) * 10;

        return (int)nd;
    }
}

// Node class
[System.Serializable]
public class PathNode
{
    public Vector3 position;
    public Vector3 parent;

    public bool isReachable;
    public bool isOpen;

    public int gCost = 0;
    public int hCost = 0;
    public int fCost = 0;

    public PathNode () { }

    public PathNode(PathNode _pathNode)
    {
        position = _pathNode.position;
        parent = _pathNode.parent;

        isReachable = _pathNode.isReachable;
        isOpen = _pathNode.isOpen;

        gCost = _pathNode.gCost;
        hCost = _pathNode.hCost;
        fCost = _pathNode.fCost;
    }
}

