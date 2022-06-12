using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionBox
{
    [field: SerializeField]
    public Vector2 Center { get; set; }
    
    [field: SerializeField]
    public Vector2 Extents { get; set; }

    public CollisionBox()
    {
        Center = Vector2.zero;
        Extents = Vector2.zero;
    }

    public CollisionBox(Vector2 center, Vector2 size)
    {
        Center = center;
        Extents = size / 2f;
    }

    public bool Overlaps(CollisionBox otherBox)
    {
        if (Center.x - Extents.x < otherBox.Center.x + otherBox.Extents.x && 
            Center.x + Extents.x > otherBox.Center.x - otherBox.Extents.x &&
            Center.y - Extents.y < otherBox.Center.y + otherBox.Extents.y &&
            Center.y + Extents.y > otherBox.Center.y - otherBox.Extents.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
