using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monument.World
{
    public static class NavNodePathFinder
    {
        public static List<NavNode> FindPathFrom(NavNode originNode, NavNode target)
        {
            Queue<NavNode> queue = new Queue<NavNode>();
            queue.Enqueue(originNode);

            Dictionary<NavNode, NavNode> parentMap = new Dictionary<NavNode, NavNode>();
            parentMap[originNode] = null;

            bool pathFound = false;

            // Inspect every neighbor and their children recursively
            while (queue.Count > 0)
            {
                NavNode current = queue.Dequeue();

                if (current == target)
                {
                    pathFound = true;
                    break;
                }

                foreach (NavNode neighbor in current.Neighbors)
                {
                    if (!parentMap.ContainsKey(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        parentMap[neighbor] = current;
                    }
                }
            }

            if (pathFound)
            {
                List<NavNode> path = BuildPathFromParentMap(parentMap, target);
                return path;
            }
            return null;
        }

        private static List<NavNode> BuildPathFromParentMap(Dictionary<NavNode, NavNode> parentMap, NavNode target)
        {
            List<NavNode> path = new List<NavNode>();
            NavNode current = target;

            while (current != null)
            {
                path.Insert(0, current);
                current = parentMap[current];
            }

            return path;
        }

        public static List<NavNode> FindReachableNodesFrom(NavNode originNode) 
        {
            List<NavNode> reachableNodes = new List<NavNode>();

            Queue<NavNode> queue = new Queue<NavNode>();
            HashSet<NavNode> visitedNodes = new HashSet<NavNode>();

            queue.Enqueue(originNode);
            visitedNodes.Add(originNode);

            while (queue.Count > 0)
            {
                NavNode currentNode = queue.Dequeue();
                reachableNodes.Add(currentNode);

                foreach (NavNode neighbor in currentNode.Neighbors)
                {
                    // if the node has not been visited and is not occupied (by another walker)
                    if (!visitedNodes.Contains(neighbor) && !neighbor.IsOccupied)
                    {
                        queue.Enqueue(neighbor);
                        visitedNodes.Add(neighbor);
                    }
                }
            }

            return reachableNodes;
        }

        public static List<NavNode> FindLongestPathFrom(NavNode originNode)
        {
            Queue<NavNode> queue = new Queue<NavNode>();
            queue.Enqueue(originNode);

            Dictionary<NavNode, NavNode> parentMap = new Dictionary<NavNode, NavNode>();
            parentMap[originNode] = null;

            NavNode lastNode = null;

            // Inspect every neighbor and their children recursively
            while (queue.Count > 0)
            {
                NavNode current = queue.Dequeue();

                foreach (NavNode neighbor in current.Neighbors)
                {
                    if (!parentMap.ContainsKey(neighbor) && !neighbor.IsOccupied)
                    {
                        queue.Enqueue(neighbor);
                        parentMap[neighbor] = current;
                        lastNode = neighbor; // Actualiza el último nodo visitado
                    }
                }
            }

            if (lastNode != null)
            {
                List<NavNode> path = BuildPathFromParentMap(parentMap, lastNode);
                return path;
            }
            return null;
        }
    }
}
