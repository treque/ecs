using System.Collections.Generic;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        List<ISystem> toRegister = new List<ISystem>();

        SpawnerSystem spawnerSystem = new SpawnerSystem();
        toRegister.Add(spawnerSystem);

        ColorSystem colorSystem = new ColorSystem();
        toRegister.Add(colorSystem);

        VelocitySystem velocitySystem = new VelocitySystem();
        toRegister.Add(velocitySystem);

        CollideSystem collideSystem = new CollideSystem();
        toRegister.Add(collideSystem);

        TransformSystem transformSystem = new TransformSystem();
        toRegister.Add(transformSystem);

        TopHalfSystem topHalfSystem = new TopHalfSystem();
        toRegister.Add(topHalfSystem);

        RewindSystem rewindSystem = new RewindSystem();
        toRegister.Add(rewindSystem);

        return toRegister;
    }
}