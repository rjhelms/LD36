using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{

    [Header("Generic Sprites")]
    public Sprite[] UnconvertedSprites;
    public Sprite[] ConvertedSprites;

    public bool Converted;
    public bool HasComeOnScreen;
    public float AnimateInterval;

    protected int SpriteState;
    protected float nextSpriteChange;
    protected SpriteRenderer spriteRenderer;
    protected GameController gameController;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        gameController = FindObjectOfType<GameController>();

        nextSpriteChange = Time.time + AnimateInterval;
        SpriteState = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (gameController.IsRunning)
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
    }

    void FixedUpdate()
    {
        if (gameController.IsRunning && HasComeOnScreen && !Converted)
            this.DoFrameAction();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bottom")
            HasComeOnScreen = true;
        else if (coll.gameObject.tag == "Bounds" || coll.gameObject.tag == "NPC"
            || coll.gameObject.tag == "AICollision")
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, 1, 1);
        }
    }

    public void Hit()
    {
        Converted = true;
        this.GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer.sprite = ConvertedSprites[SpriteState];
        nextSpriteChange = Time.time + AnimateInterval;
    }

    protected void Wander()
    {
        if (this.transform.localScale.x > 0)
        {
            this.transform.position += new Vector3(gameController.NPCWalkSpeed, 0);
        }
        else
        {
            this.transform.position -= new Vector3(gameController.NPCWalkSpeed, 0);
        }
    }

    protected virtual void DoFrameAction()
    {
        this.Wander();
    }
}
