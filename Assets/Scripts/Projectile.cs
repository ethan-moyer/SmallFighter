using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NewCollisionBox))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Projectile> ProjectileDestroyed;
    public ActionData action;
    [SerializeField] private Vector3 velocity;
    private NewCollisionBox hitbox;
    private bool shouldBeDestroyed;
    public NewFighter owner { get; private set; }


    public void Init(NewFighter owner)
    {
        this.owner = owner;
    }

    private void Awake()
    {
        hitbox = GetComponent<NewCollisionBox>();
        hitbox.Collided.AddListener(OnHitboxCollides);
    }

    private void Update()
    {
        transform.Translate(velocity * 0.0167f);

        float leftBound = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, Mathf.Abs(Camera.main.transform.position.z))).x;
        float rightBound = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, Mathf.Abs(Camera.main.transform.position.z))).x;
        if (hitbox.boxCollider.bounds.max.x < leftBound || hitbox.boxCollider.bounds.min.x > rightBound)
        {
            shouldBeDestroyed = true;
        }
    }

    private void LateUpdate()
    {
        if (shouldBeDestroyed)
        {
            ProjectileDestroyed.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void Pause()
    {
        velocity = Vector3.zero;
    }

    private void OnHitboxCollides(Collider2D col)
    {
        if (col.tag == "ProjectileBox")
        {
            col.GetComponent<Projectile>().shouldBeDestroyed = true;
            shouldBeDestroyed = true;
        }
        else if (col.tag == "FighterBox" && col.transform.parent != owner.transform)
        {
            shouldBeDestroyed = true;
        }
    }
}
