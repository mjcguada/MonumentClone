
namespace Monument.World
{
    [System.Serializable]
    public class Linker
    {
        public NavNode NodeA;
        public NavNode NodeB;

        public bool areLinked = true;

        public void ApplyConfiguration()
        {
            if(NodeA == null || NodeB == null) return;

            NodeA.SetNeighborActive(NodeB, areLinked);
            NodeB.SetNeighborActive(NodeA, areLinked);
        }
    }
}
