public class VelocitySystem : ISystem
{
    public string Name
    {
        get
        {
            return "Velocity System";
        }
    }

    private bool _areVelocitiesSet = false;

    public void UpdateSystem()
    {
        if (!_areVelocitiesSet)
        {
            InitializeVelocities(World.StateName.Current);
            InitializeVelocities(World.StateName.Previous);
            _areVelocitiesSet = true;
        }
    }

    private void InitializeVelocities(World.StateName state)
    {
        foreach (EntityComponent entity in World.Entities)
        {    
            Config config = ECSManager.Instance.Config;
            if (World.States[state].Dynamics.ContainsKey(entity))
            {
                VelocityComponent velocity = new VelocityComponent(config.allShapesToSpawn[(int)entity.id].initialSpeed);
                World.States[state].Velocities.Add(entity, velocity);
            }
        }
    }
}
