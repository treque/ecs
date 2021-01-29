public static class TopHalfBehaviour
{
    private const int NUMBER_OF_ITERATIONS = 5;

    public static void UpdateTopHalf(EntityComponent entity, World.StateName state)
    {
        if (World.States[state].TopHalf.ContainsKey(entity))
        {
            for (int i = 0; i <= NUMBER_OF_ITERATIONS; ++i)
            {
                TransformSystem.UpdateTransform(entity, state);
                ColorSystem.UpdateColors(entity, state);
                CollideSystem.UpdateCollisions(entity, state);
            }
        }
    }
}
