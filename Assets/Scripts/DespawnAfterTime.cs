using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;

    private void Start()
    {
        StartCoroutine(DestroyAfterLifetime());
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
