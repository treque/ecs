using System.Collections.Generic;

public static class World
{
    public struct State
    {
        public Dictionary<EntityComponent, TransformComponent> Transforms;
        public Dictionary<EntityComponent, ColorComponent> Colors;
        public Dictionary<EntityComponent, VelocityComponent> Velocities;
        public Dictionary<EntityComponent, CollisionComponent> Collisions;
        public Dictionary<EntityComponent, DynamicComponent> Dynamics;
        public Dictionary<EntityComponent, TopHalfComponent> TopHalf;
    }

    public enum StateName
    {
        Previous,
        Current
    }

    public static Dictionary<StateName, State> States = new Dictionary<StateName, State>();
    public static List<EntityComponent> Entities = new List<EntityComponent>();

    public static void InitializeWorld()
    {
        State previousState = new State
        { 
            Transforms = new Dictionary<EntityComponent, TransformComponent>(),
            Colors = new Dictionary<EntityComponent, ColorComponent>(),
            Collisions = new Dictionary<EntityComponent, CollisionComponent>(),
            Velocities = new Dictionary<EntityComponent, VelocityComponent>(),
            Dynamics = new Dictionary<EntityComponent, DynamicComponent>(),
            TopHalf = new Dictionary<EntityComponent, TopHalfComponent>()
        };

        State currentState = new State
        {
            Transforms = new Dictionary<EntityComponent, TransformComponent>(),
            Colors = new Dictionary<EntityComponent, ColorComponent>(),
            Collisions = new Dictionary<EntityComponent, CollisionComponent>(),
            Velocities = new Dictionary<EntityComponent, VelocityComponent>(),
            Dynamics = new Dictionary<EntityComponent, DynamicComponent>(),
            TopHalf = new Dictionary<EntityComponent, TopHalfComponent>()
        };
    
        States.Add(StateName.Previous, previousState);
        States.Add(StateName.Current, currentState); 
    }
}
