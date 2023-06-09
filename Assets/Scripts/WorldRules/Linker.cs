using UnityEngine;

namespace Monument.World
{
    [System.Serializable]
    public class Linker
    {
        public Walkable NodeA;
        public Walkable NodeB;

        public bool areLinked = true;

        public void ApplyConfiguration()
        {
            NodeA.SetNeighborActive(NodeB, areLinked);
            NodeB.SetNeighborActive(NodeA, areLinked);
        }
    }
}
