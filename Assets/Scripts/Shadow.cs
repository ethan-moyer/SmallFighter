using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] private Transform parent;

    private void Update()
    {
        transform.position = new Vector3(parent.position.x, transform.position.y, transform.position.z);
    }
}
