using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionWithEdgesBehaviour
{
    public static bool CollidesWithEdges(TransformComponent transform)
    {
        float radius = transform.Size/2;
        float left_x = Camera.main.WorldToScreenPoint(new Vector3(transform.Position.x - radius, 0, 0)).x;
        float right_x = Camera.main.WorldToScreenPoint(new Vector3(transform.Position.x + radius, 0, 0)).x;
        float top_y = Camera.main.WorldToScreenPoint(new Vector3(0, transform.Position.y + radius, 0)).y;
        float bottom_y = Camera.main.WorldToScreenPoint(new Vector3(0, transform.Position.y - radius, 0)).y;

        return (left_x <= 0 || right_x >= Camera.main.pixelWidth || top_y >= Camera.main.pixelHeight || bottom_y <= 0);
    }
}