using UnityEngine;

enum BowmanState
{
    WANDER,
    READY_FIRE,
    FIRE,
    COOLDOWN,
}

class Bowman : NPC
{
    [Header("Specific Sprites")]
    public Sprite[] FiringSprites;

    [Header("Prefabs")]
    public GameObject ProjectilePrefab;

    [Header("Balance")]
    public float StateChangeTimeMultiplier = 1.0f;
    public float FireChance = 0.4f;

    public Transform ArrowSpawnPoint;
    public BowmanState State;

    private float? nextStateChangeTime = null;

    protected override void Update()
    {
        if (gameController.IsRunning)
        { 
        if (nextStateChangeTime == null && HasComeOnScreen)
        {
            nextStateChangeTime = Time.fixedTime + (gameController.NPCBaseStateChangeTime * StateChangeTimeMultiplier);
        }

        if (Converted)
        {
            State = BowmanState.WANDER;
        }
            switch (State)
            {
                case BowmanState.WANDER:
                    if (this.State == BowmanState.WANDER)
                    {
                        if (Time.time > nextSpriteChange)
                        {
                            SpriteState++;
                            if (Converted && SpriteState == ConvertedSprites.GetLength(0))
                                SpriteState = 0;
                            if (!Converted && SpriteState == UnconvertedSprites.GetLength(0))
                                SpriteState = 0;
                            if (Converted)
                            {
                                spriteRenderer.sprite = ConvertedSprites[SpriteState];
                            }
                            else
                            {
                                spriteRenderer.sprite = UnconvertedSprites[SpriteState];
                            }
                            nextSpriteChange += AnimateInterval;
                        }
                    }
                    break;
                case BowmanState.READY_FIRE:
                    spriteRenderer.sprite = FiringSprites[0];
                    break;
                case BowmanState.FIRE:
                    spriteRenderer.sprite = FiringSprites[1];
                    break;
                case BowmanState.COOLDOWN:
                    spriteRenderer.sprite = FiringSprites[1];
                    break;
            }
        }
    }

    protected override void DoFrameAction()
    {
        switch (State)
        {
            case BowmanState.WANDER:
                this.Wander();
                if (Time.fixedTime >= nextStateChangeTime)
                {
                    State = FireOrWander();
                    nextStateChangeTime = Time.fixedTime + (gameController.NPCBaseStateChangeTime * StateChangeTimeMultiplier);
                }
                break;
            case BowmanState.READY_FIRE:
                if (Time.fixedTime >= nextStateChangeTime)
                {
                    State++;
                    nextStateChangeTime = Time.fixedTime + (gameController.NPCBaseStateChangeTime * StateChangeTimeMultiplier);
                }
                break;
            case BowmanState.FIRE:
                GameObject projectile = (GameObject)Instantiate(ProjectilePrefab, ArrowSpawnPoint.transform.position, Quaternion.identity);
                projectile.transform.localScale = this.transform.localScale;
                State++;
                break;
            case BowmanState.COOLDOWN:
                if (Time.fixedTime >= nextStateChangeTime)
                {
                    State = FireOrWander();
                    nextStateChangeTime = Time.fixedTime + (gameController.NPCBaseStateChangeTime * StateChangeTimeMultiplier);
                }
                break;
        }
    }

    private BowmanState FireOrWander()
    {
        float fireRoll = Random.value;
        if (fireRoll <= this.FireChance)
        {
            if (this.transform.position.x > gameController.PlayerTransform.position.x)
            {
                this.transform.localScale = new Vector3(-1, 1, 1);
            } else if (this.transform.position.x < gameController.PlayerTransform.position.x)
            {
                this.transform.localScale = new Vector3(1, 1, 1);
            }
            return BowmanState.READY_FIRE;
        }
        else
        {
            return BowmanState.WANDER;
        }
    }
}