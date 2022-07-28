using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class NewCollisionBox : MonoBehaviour
{
    public UnityEvent<Collider2D> Collided;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Color color;
    public BoxCollider2D boxCollider { get; private set; }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Init(Vector2 offset, Vector2 size)
    {
        transform.localPosition = offset;
        boxCollider.size = size;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0f, layerMask);

        foreach (Collider2D collider in colliders)
        {
            if (collider != boxCollider)
            {
                Collided.Invoke(collider);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        if (boxCollider != null)
            Gizmos.DrawCube(transform.position, boxCollider.size);
    }
}
