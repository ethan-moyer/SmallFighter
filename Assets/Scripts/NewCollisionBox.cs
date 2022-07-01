using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class NewCollisionBox : MonoBehaviour
{
    public UnityEvent Collided;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Color color;
    private BoxCollider2D boxCollider;


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

        if (colliders.Length > 0)
        {
            Collided.Invoke();
            print("Collided");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        if (boxCollider != null)
            Gizmos.DrawCube(transform.position, boxCollider.size);
    }
}
