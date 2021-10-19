using UnityEngine;
using System.Collections.Generic;
using System;

public class mazeExplorer : IA {

    Graph myGraph;
    public dataCollector dC;
    int ticket;

    //set this to true in the editor to hide the graph
    public bool doNotSpawnAnything = false;
    public GameObject NodeObject;
    public Material deadNodeMaterial;

    /// <summary>
    /// Class representing the node of the graph.
    /// </summary>
    class Node
    {
        static int nodesID = 0;
        int ID;
        /// <summary>
        /// Gets the ID of the node.
        /// </summary>
        public int getID() { return ID; }

        int nrOfSons = 0;
        const int maxNrOfSons = 8;
        public bool acceptMoreSons() { return nrOfSons <= maxNrOfSons; }

        Vector3 center;
        /// <summary>
        /// Gets the position associated with the node.
        /// </summary>
        public Vector3 getCenter() { return center; }
        List<Node> joinedNodes;
        /// <summary>
        /// Gets a list of all the nodes joined to this one.
        /// </summary>
        public List<Node> getJoinedNodes() { return joinedNodes; }

        Graph graph;

        private bool doNotSpawnAnything;
        private GameObject phisicalObject;
        private Material deadNodeMaterial;
        private LineRenderer lr;

        bool active = true;
        /// <summary>
        /// Indicate if the node is active (can still create sons) or not
        /// </summary>
        public bool isActive() { return active; }
        /// <summary>
        /// Deactivate the node and eventually change its color
        /// </summary>
        public void deactivateNode() { active = false; if (!doNotSpawnAnything) phisicalObject.GetComponent<MeshRenderer>().material = deadNodeMaterial; }
          
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position of the node</param>
        /// <param name="g">Model of the drone that will be spawn in its position</param>
        /// <param name="deadNodeMaterial">Material that will be assigned to the gameObject representing the node
        /// when it'll be deactivated</param>
        /// <param name="graph">Graph of the node</param>
        /// <param name="father">Node that generates this node</param>
        /// <param name="doNotSpawnAnything">If TRUE indicates that the node doesn't spawn the model and the archs between nodes</param>
        public Node(Vector3 pos, GameObject g, Material deadNodeMaterial, Graph graph, Node father = null, bool doNotSpawnAnything = false) {
            ID = ++nodesID;
            this.doNotSpawnAnything = doNotSpawnAnything;
            this.graph = graph;
            center = pos;
            joinedNodes = new List<Node>();

            if (this.doNotSpawnAnything) return;

            // if !doNotSpawnAnything

            phisicalObject = (GameObject) Instantiate(g, pos, Quaternion.identity);
            this.deadNodeMaterial = deadNodeMaterial;
            
            // initializing LineRenderer
            lr = phisicalObject.AddComponent<LineRenderer>();
            lr.SetVertexCount(maxNrOfSons * 2);
            lr.SetWidth(0.35f, 0f);
            for (int i = 0; i < maxNrOfSons * 2; i++)
                lr.SetPosition(i, center);

        }
      
        /// <summary>
        /// Obtains a list of the neighbors nodes, check if it is possible to create a connection with all of them
        /// and if it is possible it creates it.
        /// </summary>
        public void createConnections()
        {
            if (!acceptMoreSons()) return;

            List<Node> neighbors = graph.getNeighbors(this);
            foreach(Node n in neighbors)
                if (isConnectable(n))
                {
                    reJoinWith(n);
                    n.reJoinWith(this);
                }
        }

        /// <summary>
        /// Complete the join with a node
        /// </summary>
        /// <param name="n">The node is requesting the rejoin</param>
        public void reJoinWith(Node n)
        {
            if (!doNotSpawnAnything)
                lr.SetPosition(nrOfSons * 2 + 1, n.center);

            joinedNodes.Add(n);
            nrOfSons++;
        }

        /// <summary>
        /// Check if it is possible to create a connection between this node and the one passed as parameter
        /// </summary>
        /// <param name="n">Node we want to check</param>
        /// <returns>Returns TRUE if it is possible to establish a connection, otherwise FALSE</returns>
        private bool isConnectable(Node n) {
            return (!n.Equals(this) && !joinedNodes.Contains(n) && n.acceptMoreSons() &&  graph.areConnectables(this, n));
        }      

        /// <summary>
        /// Gets the hashCode of the node object.
        /// </summary>
        public override int GetHashCode() { return center.GetHashCode(); }

        /// <summary>
        /// Returns TRUE if the object passed as parameter is a node and is equal to this one
        /// </summary>
        /// <param name="obj">The object we want to check</param>
        public override bool Equals(object obj)
        {
            Node tmp;
            if (obj == null) return false;
            else if ((tmp = (Node)obj) == null) return false;
            else return tmp.center.Equals(this.center);
        }


        // -- path searching -- //

        float estimation;
        /// <summary>
        /// Get the estimation. Is related to the Dijkstra algorithm of path-finding.
        /// </summary>
        public float getEstimation() { return estimation; }
        bool hasBeenChecked;
        bool isSource;
        Node predecessor;
        /// <summary>
        /// Get the predecessor of the node. Is related to the Dijkstra algorithm of path-finding.
        /// </summary>
        public Node getPredecessor() { return predecessor; }

        /// <summary> 
        /// Initializes the variables used for the path finding functions 
        /// </summary>
        public void initializeForPathFinding() {
            estimation = int.MaxValue;
            hasBeenChecked = false;
            isSource = false;
            predecessor = null;
        }

        /// <summary> 
        /// Execute a Relax on the adjacents node, changing their value if necessary (node to node) 
        /// </summary>
        private void relax(Node u)
        {           
            hasBeenChecked = true;
            this.estimation = Vector3.Distance(this.getCenter(), u.getCenter()) + u.estimation;
            predecessor = u;

            foreach (Node n in joinedNodes)
                if (!n.hasBeenChecked || n.estimation > this.estimation + Vector3.Distance(this.getCenter(), n.getCenter()))
                    n.relax(this);            
        }
        
        /// <summary> 
        /// Execute a Relax on the node, changing its value if necessary (graph to node) 
        /// </summary>
        public void relax()
        {
            estimation = 0;
            isSource = true;
            hasBeenChecked = true;

            foreach (Node n in joinedNodes)
                if (!n.hasBeenChecked || n.estimation > this.estimation + Vector3.Distance(this.getCenter(), n.getCenter()))
                    n.relax(this);
        }
    }

    /// <summary>
    /// Class representing a graph.
    /// </summary>
    class Graph
    {
        const int minimumFreeCellsRadiusToCreateANode = 10;
        const int minimumDistanceBetweenCells = 5;

        Node root;
        /// <summary>
        /// Gets the first-spawned node of the graph.
        /// </summary>
        public Node getRoot() { return root; }

        // list of all active nodes.
        LinkedList<Node> activeNodes;
        // list of all nodes.
        List<Node> allNodes;
        // Data-Structure containing the matrix where the information about the environment is saved
        dataCollector.Data data;

        GameObject nodeObj;
        Material deadNodeMaterial;

        // matrix of lists. Each node is assigned to a position in the matrix depending of its position
        List<Node>[,] sectors;
        Vector3 upperLeftPoint;
        float sectorWidth;
        int sectorsDim;   
        const float sectorPercentualWidth = 0.05f;
        bool doNotSpawnAnything;
        public bool stoppedGrowing = false;

        /// <summary>
        /// Constructor of the graph class
        /// </summary>
        /// <param name="rootPosition">Position of the first node</param>
        /// <param name="data">Data-Structure containing the matrix where the information about the environment is saved</param>
        /// <param name="nodeObj">Model of the node that will be spawned</param>
        /// <param name="deadNodeMaterial">Material assigned to the node once it has been deactivated</param>
        /// <param name="doNotSpawnAnything">If TRUE indicates that the node doesn't spawn the model and the archs between nodes</param>
        public Graph(Vector3 rootPosition, dataCollector.Data data, GameObject nodeObj, Material deadNodeMaterial, bool doNotSpawnAnything = false)
        {
            this.nodeObj = nodeObj;
            this.deadNodeMaterial = deadNodeMaterial;
            this.doNotSpawnAnything = doNotSpawnAnything;

            root = new Node(rootPosition, nodeObj, deadNodeMaterial, this, null, doNotSpawnAnything);
            activeNodes = new LinkedList<Node>();
            activeNodes.AddFirst(root);
            allNodes = new List<Node>();
            allNodes.Add(root);
            this.data = data;

            // sectors structure inizialization
            upperLeftPoint = data.indxToV3(new dataCollector.indx(0, 0));
            float totalLengthOfExplorableZone = Mathf.Abs(upperLeftPoint.x - data.indxToV3(new dataCollector.indx(data.getArraySize()-1 , 0)).x);
            sectorWidth = totalLengthOfExplorableZone * sectorPercentualWidth;
            sectorsDim = (int)(1 / sectorPercentualWidth);
            sectors = new List<Node>[sectorsDim, sectorsDim];
            for (int i = 0; i < sectorsDim; i++)
                for (int j = 0; j < sectorsDim; j++)
                    sectors[i, j] = new List<Node>();

            assignNodeToSector(root);   
        }

        /// <summary>
        /// Given a position and the upperLeft point of the sectors, it returns a Vector2 that represents the index of the 
        /// matrix containing the nodes belonging to that sector
        /// </summary>
        /// <param name="pos">The position we want to discover the sector</param>
        /// <param name="zeroPoint">Upper left point of the sectors matrix</param>
        /// <returns></returns>
        private Vector2 getSectorIndxs(Vector3 pos, Vector3 zeroPoint)
        {
            int x = 0;
            while (!(zeroPoint.x + sectorWidth * x < pos.x && zeroPoint.x + sectorWidth * (x + 1) >= pos.x)) { x++; if (x > sectorsDim) throw new Exception(); }
            int z = 0;
            while (!(zeroPoint.z + sectorWidth * z < pos.z && zeroPoint.z + sectorWidth * (z + 1) >= pos.z)) { z++; if (z > sectorsDim) throw new Exception(); }
            return new Vector2(x, z);
        }

        /// <summary>
        /// Put a point in its list inside the sectors-matrix, using its position to find the right sector
        /// </summary>
        /// <param name="n">Node we want to put in the matrix of sectors</param>
        private void assignNodeToSector(Node n)
        {
            Vector2 indx = getSectorIndxs(n.getCenter(), upperLeftPoint);
            sectors[(int)indx.x, (int)indx.y].Add(n);            
        }

        /// <summary>
        /// Function that returns a list with all the nodes that are in the sector of the node passed as argument
        /// and the nodes that lie in the adjacent sectors
        /// </summary>
        /// <param name="n">We want to obtain the neighbors of this node</param>
        /// <returns>A list of all the neighbors node of 'n'</returns>
        public List<Node> getNeighbors(Node n)
        {
            List<Node> tmpList = new List<Node>();
            Vector2 indx = getSectorIndxs(n.getCenter(), upperLeftPoint);
            int x = (int)indx.x;
            int z = (int)indx.y;

            for (int i = (x > 0 ? x - 1 : x); i <= (x < sectorsDim - 1 ? x + 1 : x); i++)
                for (int j = (z > 0 ? z - 1 : z); j <= (z < sectorsDim - 1 ? z + 1 : z); j++)
                    foreach (Node node in sectors[i, j])
                        tmpList.Add(node);
            return tmpList;
        }

        /// <summary>
        /// Function that returns a list with all the nodes that are in the sector of the position passed as argument
        /// </summary>
        /// <param name="pos">We want to obtain the nodes in the sector where lies this position</param>
        /// <returns>A list with all the nodes in the same sector of the given position</returns>
        public List<Node> getCloseNeighbors(Vector3 pos)
        {
            List<Node> tmpList = new List<Node>();
            Vector2 indx = getSectorIndxs(pos, upperLeftPoint);
            int x = (int)indx.x;
            int z = (int)indx.y;

            foreach (Node node in sectors[x,z])
                tmpList.Add(node);
            return tmpList;
        }

        /// <summary>
        /// Given an active node, the algorithm spawn every possible node around it and check if it is still active
        /// or has to be deactivated
        /// </summary>
        /// <param name="node">Node we want to expand</param>
        /// <returns>Returns a list with all the sons of the node. It could be empty.</returns>
        private LinkedList<Node> expandNode(Node node)
        {

            LinkedList<Node> sons = new LinkedList<Node>();
            bool deadNode = true;

            bool reloadNeighbors = false;
            List<Node> neighbors = getNeighbors(node);

            // check all around the node if it is possible to create a son
            for (float angle = 0; angle < 2 * Mathf.PI; angle += 0.5f)
            {
                if (reloadNeighbors) neighbors = getNeighbors(node);

                bool enoughDistanceFromOthersNode = true;
                Vector3 newPos = node.getCenter() + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized * (minimumDistanceBetweenCells * 1.1f);
                // check if the path is free (no unknown cells, no walls)
                if (noWallsBetween(node.getCenter(), newPos))
                {
                    // check the distances between the nodes in the same sector
                    foreach (Node n in neighbors)
                        if (Vector3.Distance(n.getCenter(), newPos) < minimumDistanceBetweenCells)
                            enoughDistanceFromOthersNode = false;
                }
                else
                    enoughDistanceFromOthersNode = false;


                if (enoughDistanceFromOthersNode)               // there is enough distance between the choosen position and all its neighbors
                {                                        
                    int result = data.areAllFree(newPos, minimumFreeCellsRadiusToCreateANode);
                    // 0 -> areAllFree; 1 -> there are walls; 2 -> no walls but unknown cells

                    if (result == 0)
                    {
                        Node newNode = new Node(newPos, nodeObj, deadNodeMaterial, this, node, doNotSpawnAnything);
                        assignNodeToSector(newNode);
                        node.createConnections();
                        sons.AddFirst(newNode);
                        reloadNeighbors = true;
                    }
                    else if (result == 2)
                        deadNode = false;
                }

            }

            if (deadNode || !node.acceptMoreSons()) node.deactivateNode();
            return sons;
        }

        /// <summary>
        /// Search in the active nodes of the graph and if it is possible to expand them, it does it.
        /// </summary>
        public void expandGraph()
        {
            if (activeNodes.Count == 0) { stoppedGrowing = true; return; }

            // copying the active nodes to a vector
            Node[] actualNodes = new Node[activeNodes.Count];           
            activeNodes.CopyTo(actualNodes,0);
            activeNodes = new LinkedList<Node>();

            foreach(Node n in actualNodes)
            {
                LinkedList<Node> sons = expandNode(n);
                while (sons.Count > 0)
                {
                    // add the new node to the data-structures
                    activeNodes.AddFirst(sons.First.Value);
                    allNodes.Add(sons.First.Value);
                    sons.RemoveFirst();
                }

                // the expansion procedure cheks also if the node is active. If not we deactivate it
                if (n.isActive())   activeNodes.AddFirst(n);
            }
        }

        /// <summary>
        /// Check if two nodes are connectables
        /// </summary>
        /// <param name="n1">First node</param>
        /// <param name="n2">Second node</param>
        /// <returns>Returns TRUE if it is possible to create a connection between the centers of the two nodes</returns>
        public bool areConnectables(Node n1, Node n2) { return areConnectables(n1.getCenter(), n2.getCenter()); }

        /// <summary>
        /// Check if two points are connectables
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <returns>Returns TRUE if it is possible to create a connection the two points</returns>
        public bool areConnectables(Vector3 v1, Vector3 v2)
        {
            if (Vector3.Distance(v1, v2) > minimumDistanceBetweenCells * 1.8f) return false;

            while (Vector3.Distance((v1 = Vector3.MoveTowards(v1, v2, 1)), v2) > 0)
                if (data.getValue(v1) != 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Check if there are no walls between two points.
        /// <para> This function is stricter than <c>areConnectables()</c> because this assures that there is a 
        /// wide line of empty cells between the two points</para>
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <returns>Returns TRUE if it is possible to create a connection the two points</returns>
        public bool noWallsBetween(Vector3 v1, Vector3 v2)
        {
            Vector3 tmpV = v1;
            while (Vector3.Distance((tmpV = Vector3.MoveTowards(tmpV, v2, 1)), v2) > 0)
                if (data.getValue(tmpV) == 1)
                    return false;

            tmpV = v1 + Vector3.right * 0.5f;
            while (Vector3.Distance((tmpV = Vector3.MoveTowards(tmpV, v2, 1)), v2) > 0)
                if (data.getValue(tmpV) == 1)
                    return false;

            tmpV = v1 + Vector3.forward * 0.5f;
            while (Vector3.Distance((tmpV = Vector3.MoveTowards(tmpV, v2, 1)), v2) > 0)
                if (data.getValue(tmpV) == 1)
                    return false;

            tmpV = v1 + Vector3.right * -0.5f;
            while (Vector3.Distance((tmpV = Vector3.MoveTowards(tmpV, v2, 1)), v2) > 0)
                if (data.getValue(tmpV) == 1)
                    return false;

            tmpV = v1 + Vector3.forward * -0.5f;
            while (Vector3.Distance((tmpV = Vector3.MoveTowards(tmpV, v2, 1)), v2) > 0)
                if (data.getValue(tmpV) == 1)
                    return false;

            return true;
        }
       

        // -- path searching -- //

        /// <summary>
        /// Given a position and a list of nodes it returns the nearest node. 
        /// It is used to find the first node of a path.
        /// </summary>
        /// <param name="pos">Position we use to check the nearest node to</param>
        /// <param name="nodes">List of nodes where we have to look for the nearest one</param>
        /// <returns>Return the nearest node to the position</returns>
        public Node nearestTo(Vector3 pos, List<Node> nodes)
        {
            float distance = float.MaxValue;
            Node tmpNode = getRoot();
            foreach (Node n in nodes)
                if (Vector3.Distance(pos, n.getCenter()) < distance)
                {
                    distance = Vector3.Distance(pos, n.getCenter());
                    tmpNode = n;
                }
            return tmpNode;
        }

        /// <summary>
        /// Runs the path-finding algorithm and returns the nearest destination.
        /// <para>As the drones is always going to the nearest active node to deactivate it
        /// we are reading the estimation of each node still active and from this we get the nearest</para>
        /// </summary>
        /// <param name="initialNode">Node that the path-finding algorithm use as starting point</param>
        /// <returns>Returns the nearest active node to the initialNode</returns>
        public Node getNearestDestination(Node initialNode)
        {
            foreach (Node n in allNodes)
                n.initializeForPathFinding();

            initialNode.relax();

            Node nearestNode = null;
            float distance = float.MaxValue;
            foreach (Node n in activeNodes)
                if (n.getEstimation() < distance)
                {
                    distance = n.getEstimation();
                    nearestNode = n;
                }
            return nearestNode;
        }

        /// <summary>
        /// Given the nearest node (that is obtained with <c>getNearestDestination(Node initialNode)</c>
        /// the function returns a list of nodes that represents the path to get from the initial node to the final one.
        /// </summary>
        /// <param name="nearestNode">Destination node</param>
        /// <returns>Returns a list of nodes that represents the path to get from the initial node to the final one.</returns>
        public LinkedList<Node> getPathTo(Node nearestNode)
        {
            if (nearestNode == null) return null;

            LinkedList<Node> path = new LinkedList<Node>();
            while (nearestNode.getPredecessor() != null)
            {
                path.AddFirst(nearestNode);
                nearestNode = nearestNode.getPredecessor();
            }

            return path;
        }

        /// <summary>
        /// Function that runs sporadically and creates new connections between nodes
        /// </summary>
        public void recreateConnections() { foreach (Node n in allNodes) n.createConnections(); }
    }

    /// <summary>
    /// Function called before of the first update. Used to initialize the components.
    /// </summary>
    void Start () {
        dC = GameObject.FindGameObjectWithTag("dataCollector").GetComponent<dataCollector>();
        ticket = dC.initialize(transform.position);
        setSinglePointcommandResponsive(false);
        lockSinglePoint();
        setDistanceOfPointToLookAt(1000);
        myGraph = new Graph(transform.position, dC.data, NodeObject, deadNodeMaterial, doNotSpawnAnything);
	}

    float interval = 0.5f;
    float actualTime = 0.25f;
    int connectionCounter = 0;

    /// <summary>
    /// Function called once per frame.
    /// </summary>
    void Update () {

        if (FINISHED) return;

        // every time the timer tics (actualTime <= 0)..
        if ((actualTime -= Time.deltaTime) < 0)
        {
            // launches raycasts all around the drone to get informations about the walls and the empty space surrounding it
            for (float angle = 0; angle < 2 * Mathf.PI; angle += 0.02f)
                raycast(new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)));
            actualTime += interval;

            // updates the position of the drone in the dataCollector
            dC.updatePosition(ticket, transform.position);

            // try to expand the graph with the new information
            myGraph.expandGraph();

            // sporadically tries to create new connections
            if (++connectionCounter % 100 == 0)
                myGraph.recreateConnections(); 
        }

        // function that manage the exploration (path finding)
        pathManager();
    }


    public float raycastDistance = 50f;
    /// <summary>
    /// Perform a raycast in the given direction and update the matrix containing the information
    /// about the environment
    /// </summary>
    /// <param name="direction">Direction we are launching the raycast to</param>
    void raycast(Vector3 direction)
    {
        RaycastHit hit;
        Vector3 vt = transform.position;
        if (Physics.Raycast(vt, direction, out hit, raycastDistance))
        {
            //Debug.DrawLine(transform.position, hit.point);            
            while (!dC.sameIndx(vt, hit.point))
            {
                dC.addDataToTheMatrix(vt, 0);                         
                vt = Vector3.MoveTowards(vt, hit.point, 1);
            }
            dC.addDataToTheMatrix(vt, 1);

        }
        else
        {
            Vector3 destination = vt + direction * raycastDistance;
            while (!dC.sameIndx(vt, destination))
            {
                dC.addDataToTheMatrix(vt, 0);
                vt = Vector3.MoveTowards(vt, destination, 1);
            }
        }        
    }


    // -- path searching -- //

    // boolean that indicates that the exploration is done.
    bool FINISHED = false;

    bool pathInitialized;    
    LinkedList<Node> path;
    Node actualNode;
    Node actualDestination;
    Node finalDestination;

    /// <summary>
    /// Returns TRUE if the drone has arrived to the destination.
    /// </summary>
    /// <param name="destination">Destination of the drone</param>
    bool isArrived(Vector3 destination) { return Vector3.Distance(transform.position, destination) < 1f; }

    float initialInterval = 5f;

    /// <summary>
    /// Function that manages the exploration through path-finding functions of the tree
    /// and moving the SinglePoint through the AI protected functions
    /// </summary>
    private void pathManager()
    {
        // We wait some time in the beggining, 
        // so the drone moves a little and starts mapping the maze
        if ((initialInterval -= Time.deltaTime) > 0)                            
        {
            setSinglePointcommandResponsive(false);
            setSinglePointPosition(transform.position - Vector3.right * 0.25f);
        }
        // calculate the path to the nearest active node
        else if (!pathInitialized)                                              
        {
            actualNode = myGraph.nearestTo(transform.position, myGraph.getCloseNeighbors(transform.position));
            finalDestination = myGraph.getNearestDestination(actualNode);
            path = myGraph.getPathTo(finalDestination);

            if (path == null)
            {
                // we have finished exploring!
                FINISHED = true;
                return;
            }
            if (path.Count < 1)
            {
                // The root is still active. We keep waiting a little bit
                initialInterval += 0.2f;
                return;
            }
            actualDestination = path.First.Value;
            path.RemoveFirst();

            setSinglePointPosition(transform.position);
            pathInitialized = true;
        }
        // when the drone arrives to the actual destination 
        // we remove it and start moving the SinglePoint to the next one
        else if (isArrived(actualDestination.getCenter())) 
        {
            if (path.Count == 0) { pathInitialized = false; initialInterval = 1; return; }

            actualNode = actualDestination;
            actualDestination = path.First.Value;
            path.RemoveFirst();            
        }
        // if the finalDestination get deactivated we look for another destination
        else if (!finalDestination.isActive())
        {
            pathInitialized = false;
            initialInterval = 1;
        }
        // if these conditions are false we simply move the singlePoint toward the actual destination
        else
        {
            if (getDistanceFromRoutePoint() < 5)
                setSinglePointPosition(Vector3.MoveTowards(getSinglePointPosition(), actualDestination.getCenter(), 2 * Time.deltaTime));

        }

    }

}
