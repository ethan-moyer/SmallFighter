using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector2 horizontalRange = Vector2.zero;
    [SerializeField] private float floor = 1f;
    [SerializeField] private float smoothTime = 1f;
    private GameObject[] fighters;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        fighters = GameObject.FindGameObjectsWithTag("Fighter");
    }

    private void LateUpdate()
    {
        if (fighters.Length > 0)
        {
            Vector3 target = new Vector3();
            foreach (GameObject fighter in fighters)
            {
                target.x += fighter.transform.position.x;
                target.y += fighter.transform.position.y;
            }
            target.x /= fighters.Length;
            target.y /= fighters.Length;

            target.x = Mathf.Clamp(target.x, horizontalRange.x, horizontalRange.y);
            target.y = Mathf.Max(floor, target.y);
            target.z = transform.position.z;

            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime, Mathf.Infinity, 0.0167f);
        }
    }
}
