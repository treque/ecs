public class TopHalfSystem : ISystem
{
    public string Name
    {
        get
        {
            return "TopHalfSystem";
        }
    }

    public void UpdateSystem()
    {
        foreach (EntityComponent entity in World.Entities)
        {
            TopHalfBehaviour.UpdateTopHalf(entity, World.StateName.Current);
        }
    }
}
