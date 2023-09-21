
/**
 * Class that represents an array of active links between nodes
 * Used by rotative platforms to define different states
 */

namespace Monument.World
{
    [System.Serializable]
    public class PlatformConfiguration
    {
        public Linker[] Linkers;
    }
}
