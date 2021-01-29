public class CollideSystem : ISystem
{
    public string Name
    {
        get
        {
            return "Collide System";
        }
    }

    public void UpdateSystem()
    {
        foreach (EntityComponent entity in World.Entities)
        {
            UpdateCollisions(entity, World.StateName.Current);
        }
    }

    public static void UpdateCollisions(EntityComponent entity, World.StateName state)
    {
       CollisionBehaviour.UpdateCollision(entity, state);
    }
}
