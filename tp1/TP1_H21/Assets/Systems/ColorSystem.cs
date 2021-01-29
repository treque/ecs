public class ColorSystem : ISystem
{
    public string Name
    {
        get
        {
            return "Color System";
        }
    }
    public void UpdateSystem()
    {
        foreach (EntityComponent entity in World.Entities)
        {
            ColorBehaviour.UpdateColor(entity, World.StateName.Current);
        }
    }

    public static void UpdateColors(EntityComponent entity, World.StateName state)
    {
        ColorBehaviour.UpdateColor(entity, state);
    }
}
