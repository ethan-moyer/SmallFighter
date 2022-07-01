using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpriteRenderer))]
public class LateUpdateTest : MonoBehaviour
{
    public LateUpdateTest other;
    public bool stunned;
    public bool gotHit;
    private PlayerInput input;
    private SpriteRenderer sprite;


    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !stunned)
        {
            other.gotHit = true;
            print($"{gameObject.name} hit {other.gameObject.name}!");
        }
    }

    private void LateUpdate()
    {
        if (gotHit)
            StartCoroutine(SetStun());
    }

    private IEnumerator SetStun()
    {
        stunned = true;
        gotHit = false;
        sprite.color = Color.red;
        yield return new WaitForSeconds(1f);
        stunned = false;
        sprite.color = Color.white;
    }
}
