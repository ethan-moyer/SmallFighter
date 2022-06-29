using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Projectile> ProjectileDestroyed;
    [HideInInspector]public NewFighter owner;
    [HideInInspector] public BoxCollider2D boxCollider;
    public ActionData action;
    [SerializeField] private Vector3 velocity;

    public void Init(NewFighter owner)
    {
        this.owner = owner;
    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        transform.Translate(velocity * 0.0167f);

        float leftBound = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, Mathf.Abs(Camera.main.transform.position.z))).x;
        float rightBound = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, Mathf.Abs(Camera.main.transform.position.z))).x;
        if (boxCollider.bounds.max.x < leftBound || boxCollider.bounds.min.x > rightBound)
        {
            ProjectileDestroyed.Invoke(this);
            Destroy(this.gameObject);
        }
    }
}
