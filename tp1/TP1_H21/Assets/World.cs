using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private static World _instance;

    public static Dictionary<EntityComponent, TransformComponent> Transforms = new Dictionary<EntityComponent, TransformComponent>();
    public static Dictionary<EntityComponent, ColorComponent> Colors = new Dictionary<EntityComponent, ColorComponent>();
    public static Dictionary<EntityComponent, DynamicComponent> Dynamics = new Dictionary<EntityComponent, DynamicComponent>();
    public static Dictionary<EntityComponent, VelocityComponent> Velocities = new Dictionary<EntityComponent, VelocityComponent>();
    public static Dictionary<EntityComponent, CollisionComponent> Collisions = new Dictionary<EntityComponent, CollisionComponent>();
    public static List<EntityComponent> Entities = new List<EntityComponent>();

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}
