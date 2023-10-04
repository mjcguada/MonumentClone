
public interface IReactor
{
    public void React(Reaction reaction);
}

[System.Serializable]
public class Reaction 
{
    public enum ReactionType { Rotation, Movement }

    public float Units = 0;

    public ReactionType Type = ReactionType.Rotation;

    public float TimeToComplete = 0.75f;
}