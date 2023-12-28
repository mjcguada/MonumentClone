
namespace Monument.World
{
    [System.Serializable]
    public class Linker
    {
        public NavNode NodeA;
        public NavNode NodeB;

        public bool AreLinked = true;

        public void ApplyConfiguration(bool linkEnabled)
        {
            if (NodeA == null || NodeB == null) return;

            if (linkEnabled)
            {
                NodeA.AddNeighbor(NodeB);
                NodeB.AddNeighbor(NodeA);
            }
            else
            {
                NodeA.RemoveNeighbor(NodeB);
                NodeB.RemoveNeighbor(NodeA);
            }

        }
    }
}
