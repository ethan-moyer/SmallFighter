using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ObliqueFrustum : MonoBehaviour
{
    public Vector3 startingPos;
    public float horizObl;
    public float vertObl;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        transform.position = startingPos;
    }

    private void Update()
    {
        Matrix4x4 mat = cam.projectionMatrix;
        mat[0, 2] = horizObl;
        mat[1, 2] = vertObl;
        cam.projectionMatrix = mat;
    }
}
