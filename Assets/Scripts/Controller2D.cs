using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    private const float SkinWidth = 0.015f;

    public Transform CollidingLeft { get; private set; }
    public Transform CollidingRight { get; private set; }
    public Transform CollidingAbove { get; private set; }
    public Transform CollidingBelow { get; private set; }

    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private BoxCollider2D boxCollider;
    private RaycastOrigins raycastOrigins;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        ResetCollisionInfo();

        VerticalCollisions(ref velocity);
        HorizontalCollisions(ref velocity);

        transform.Translate(velocity);
    }

    private void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(raycastOrigins.bottomLeft + (Vector2.right * verticalRaySpacing * i), Vector2.up * -2f, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;

                CollidingAbove = (directionY == 1) ? hit.transform : null;
                CollidingBelow = (directionY == -1) ? hit.transform : null;
            }
        }
    }

    private void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(raycastOrigins.bottomLeft + (Vector2.up * horizontalRaySpacing * i), Vector2.right * -2f, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - SkinWidth) * directionX;
                rayLength = hit.distance;

                CollidingLeft = (directionX == -1) ? hit.transform : null;
                CollidingRight = (directionX == 1) ? hit.transform : null;
            }
        }
    }

    private void ResetCollisionInfo()
    {
        CollidingLeft = null;
        CollidingRight = null;
        CollidingAbove = null;
        CollidingBelow = null;
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(SkinWidth * -2f);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(SkinWidth * -2f);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    private struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
