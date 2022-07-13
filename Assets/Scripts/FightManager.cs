using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightManager : MonoBehaviour
{
    private const float HorizontalOverlapBoxWidth = 0.1f;
    private const float VerticalOverlapBoxHeight = 0.1f;

    public static FightManager instance;
    [SerializeField] private NewFighter[] fighters = new NewFighter[2];
    [SerializeField] private GameObject hitParticlePrefab;
    [SerializeField] private GameObject blockParticlePrefab;
    [SerializeField] private GameObject breakParticlePrefab;

    [Header("GUI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image[] fighterHealthBars;
    [SerializeField] private GameObject[] fighterRoundIcons;

    private int hitstopTimer;
    private List<Projectile> projectiles;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        foreach (NewFighter fighter in fighters)
        {
            fighter.BreakThrow.AddListener(OnBreakThrow);
            fighter.TookDamage.AddListener(OnTookDamage);
        }
        projectiles = new List<Projectile>();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (hitstopTimer > 0)
        {
            fighters[0].PauseFighter();
            fighters[1].PauseFighter();
            hitstopTimer -= 1;
            if (hitstopTimer == 0)
            {
                fighters[0].UnpauseFighter();
                fighters[1].UnpauseFighter();
            }
        }
        else
        {
            UpdateSides();
            Push();
        }
    }

    private IEnumerator Timer()
    {
        int timeRemaining = 60;

        while (timeRemaining > 0)
        {
            timeRemaining -= 1;
            timerText.text = timeRemaining.ToString();

            for (int i = 0; i < 60; i++)
                yield return null;            
        }
    }

    private IEnumerator ShakeCamera(int duration, float strength)
    {
        Vector3 startingPos = Camera.main.transform.localPosition;
        int elapsedFrames = 0;

        while (elapsedFrames < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * strength;
            float yOffset = Random.Range(-1f, 1f) * strength;

            Camera.main.transform.localPosition = new Vector3(xOffset, yOffset, startingPos.z);

            elapsedFrames += 1;
            yield return null;
        }

        Camera.main.transform.localPosition = startingPos;
    }

    private IEnumerator Hitstop(int numOfFrames)
    {
        foreach (NewFighter fighter in fighters)
            fighter.PauseFighter();

        for (int i = 0; i < numOfFrames; i++)
        {
            yield return null;
        }

        foreach (NewFighter fighter in fighters)
            fighter.UnpauseFighter();
    }

    public void OnFighterHit(NewFighter hitFighter, HitData hitData, bool attackWasBlocked)
    {
        Vector3 side = hitFighter.IsOnLeftSide ? Vector3.left : Vector3.right;

        // Push fighters back
        if (hitData.action.type != ActionData.Type.Projectile)
        {
            RaycastHit2D wallHit = Physics2D.Raycast(hitFighter.boxCollider.bounds.center + side * hitFighter.boxCollider.bounds.extents.x * 0.95f, side, hitData.action.pushback);
            if (wallHit)
                hitData.hitbox.transform.parent.GetComponent<NewFighter>().controller.Move(side * -1f * hitData.action.pushback);
            else
                hitFighter.controller.Move(side * hitData.action.pushback);
        }
        else
        {
            hitFighter.controller.Move(side * hitData.action.pushback);
        }

        // Spawn particle effect
        Vector3 particlePos = hitFighter.boxCollider.bounds.center - side * hitFighter.boxCollider.bounds.extents.x;
        particlePos.y = hitData.hitbox.boxCollider.bounds.center.y;

        if (attackWasBlocked)
        {
            Instantiate(blockParticlePrefab, particlePos, Quaternion.Euler(0f, hitFighter.IsOnLeftSide ? -66f : 66f, 0f));
        }
        else
        {
            Instantiate(hitParticlePrefab, particlePos, Quaternion.identity);
        }

        StartCoroutine(Hitstop(3));

        if (hitData.action.hitAnim == ActionData.HitAnim.Light || attackWasBlocked)
            StartCoroutine(ShakeCamera(5, 0.015f));
        else
            StartCoroutine(ShakeCamera(5, 0.03f));
    }

    private void OnTookDamage(NewFighter fighter)
    {
        if (fighter == fighters[0])
        {
            fighterHealthBars[0].fillAmount = Mathf.Max((float)fighter.currentHealth / fighter.maxHealth, 0f);
        }
        else if (fighter == fighters[1])
        {
            fighterHealthBars[1].fillAmount = Mathf.Max((float)fighter.currentHealth / fighter.maxHealth, 0f);
        }
    }

    public void OnBreakThrow(NewFighter fighter, NewFighter opponent)
    {
        print("Throw break!");

        float offset = fighter.boxCollider.bounds.extents.x + opponent.boxCollider.bounds.extents.x;
        fighter.controller.Move(Vector3.right * (fighter.IsOnLeftSide ? -offset : offset));

        fighter.controller.Move(Vector3.right * (fighter.IsOnLeftSide ? -1.5f : 1.5f));
        fighter.animator.Play("Base Layer.HitLight", -1, 0f);
        fighter.SwitchState(new Stunned(fighter, 20));

        opponent.controller.Move(Vector3.right * (opponent.IsOnLeftSide ? -1.5f : 1.5f));
        opponent.animator.Play("Base Layer.HitLight", -1, 0f);
        opponent.SwitchState(new Stunned(opponent, 20));

        Vector3 particlePos = (fighter.transform.position + opponent.transform.position) / 2f;
        Instantiate(breakParticlePrefab, particlePos, Quaternion.identity);

        fighter.ClearHitThisFrame();
        opponent.ClearHitThisFrame();
    }

    public void ThrowFighter(NewFighter fighter, NewFighter opponent, ActionData action)
    {
        opponent.currentThrow = action.throwData;
        opponent.SwitchState(new Throwing(opponent));

        Vector3 offset = opponent.transform.position - fighter.transform.position;
        StartCoroutine(DelayedThrowInitialOffset(fighter, offset));
        fighter.controller.Move(offset);
        fighter.beingThrown = true;
        fighter.currentThrow = action.throwData;
        fighter.throwOpponent = opponent;
        fighter.SwitchState(new Throwing(fighter));
    }

    private IEnumerator DelayedThrowInitialOffset(NewFighter fighter, Vector3 offset)
    {
        offset = !fighter.IsOnLeftSide ? offset : -offset;

        fighter.model.transform.Translate(fighter.model.transform.InverseTransformVector(offset));
        yield return null;
        fighter.model.transform.Translate(fighter.model.transform.InverseTransformVector(-offset));
    }

    private void UpdateSides()
    {
        if (fighters.Length != 2)
            return;

        if (fighters[0].transform.position.x - fighters[0].boxCollider.bounds.extents.x + 0.015f > fighters[1].transform.position.x)
        {
            if (fighters[0].currentState is Walking)
                fighters[0].SwitchSide(false, true);
            if (fighters[1].currentState is Walking)
                fighters[1].SwitchSide(true, true);
        }
        else if (fighters[0].transform.position.x + fighters[0].boxCollider.bounds.extents.x - 0.015f < fighters[1].transform.position.x)
        {
            if (fighters[0].currentState is Walking)
                fighters[0].SwitchSide(true, true);
            if (fighters[1].currentState is Walking)
                fighters[1].SwitchSide(false, true);
        }
    }    

    private void Push()
    {
        if (fighters.Length != 2)
            return;

        for (int i = 0; i < 2; i++)
        {
            // Horizontal Collision Checking
            Vector2 hBoxCenter = fighters[i].boxCollider.bounds.center;
            hBoxCenter.x += (fighters[i].boxCollider.bounds.extents.x + HorizontalOverlapBoxWidth / 2f) * (fighters[i].IsOnLeftSide ? 1 : -1);

            Collider2D[] horizontalHits = Physics2D.OverlapBoxAll(hBoxCenter, new Vector2(HorizontalOverlapBoxWidth, fighters[i].boxCollider.bounds.extents.y * 2f), 0);
            foreach (Collider2D col in horizontalHits)
            {
                if (col.transform == fighters[1 - i].transform && Mathf.Sign(fighters[1 - i].boxCollider.bounds.center.x - fighters[i].boxCollider.bounds.center.x) != Mathf.Sign(fighters[1 - i].velocity.x))
                {
                    fighters[i].controller.Move(fighters[1 - i].velocity.x * (1f / 60f) * Vector3.right);
                }
            }

            // Vertical Collision Checking
            if (fighters[i].velocity.y < 0f)
            {
                Vector2 vBoxCenter = fighters[i].boxCollider.bounds.center;
                vBoxCenter.y -= fighters[i].boxCollider.bounds.extents.y + VerticalOverlapBoxHeight / 2f;
                Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(vBoxCenter, new Vector2(fighters[i].boxCollider.bounds.extents.x * 2f + 0.015f, VerticalOverlapBoxHeight), 0);
                foreach (Collider2D col in overlappingColliders)
                {
                    if (col.transform == fighters[1 - i].transform)
                    {
                        if (fighters[i].boxCollider.bounds.center.x >= fighters[1 - i].boxCollider.bounds.center.x)
                        {
                            float overlap = (fighters[1 - i].boxCollider.bounds.center.x + fighters[1 - i].boxCollider.bounds.extents.x) - (fighters[i].boxCollider.bounds.center.x - fighters[i].boxCollider.bounds.extents.x);
                            if (overlap != 0)
                            {
                                RaycastHit2D wallHit = Physics2D.Raycast(fighters[1 - i].boxCollider.bounds.center + Vector3.left * fighters[1 - i].boxCollider.bounds.extents.x * 0.9f, Vector2.left, overlap * 1.5f);
                                if (wallHit)
                                {
                                    fighters[i].controller.Move(1.5f * overlap * Vector3.right);
                                }
                                else
                                {
                                    fighters[1 - i].controller.Move(1.5f * overlap * Vector3.left);
                                }
                            }

                            if (fighters[i].velocity.x < 0f)
                                fighters[i].velocity = new Vector2(0f, fighters[i].velocity.y);
                        }
                        else
                        {
                            float overlap = (fighters[i].boxCollider.bounds.center.x + fighters[i].boxCollider.bounds.extents.x) - (fighters[1 - i].boxCollider.bounds.center.x - fighters[1 - i].boxCollider.bounds.extents.x);
                            if (overlap != 0)
                            {
                                RaycastHit2D wallHit = Physics2D.Raycast(fighters[1 - i].boxCollider.bounds.center + Vector3.right * fighters[1 - i].boxCollider.bounds.extents.x * 0.9f, Vector2.right, overlap * 1.5f);
                                if (wallHit)
                                {
                                    fighters[i].controller.Move(1.5f * overlap * Vector3.left);
                                }
                                else
                                {
                                    fighters[1 - i].controller.Move(1.5f * overlap * Vector3.right);
                                }
                            }

                            if (fighters[i].velocity.x > 0f)
                                fighters[i].velocity = new Vector2(0f, fighters[i].velocity.y);
                        }
                    }
                }
            }
        }

        UpdateSides();
    }
}
