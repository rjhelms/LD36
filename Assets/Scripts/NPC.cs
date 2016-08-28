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
    public bool PlaceInWater = false;

    public int PointsValue = 100;

    protected int SpriteState;
    protected float nextSpriteChange;
    protected SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        nextSpriteChange = Time.time + AnimateInterval;
        SpriteState = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
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

    void FixedUpdate()
    {
        if (ScoreManager.Instance.GameController.IsRunning && HasComeOnScreen && !Converted)
            this.DoFrameAction();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (ScoreManager.Instance.GameController.IsRunning)
        {
            if (coll.gameObject.tag == "Bottom")
                HasComeOnScreen = true;
            else if (coll.gameObject.tag == "Bounds" || coll.gameObject.tag == "NPC"
                || coll.gameObject.tag == "AICollision" || coll.gameObject.tag == "Terrain")
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x * -1, 1, 1);
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (ScoreManager.Instance.GameController.IsRunning)
        {
            if (coll.gameObject.tag == "Bottom")
                HasComeOnScreen = true;
        }
    }

    public void Hit()
    {
        if (HasComeOnScreen)
        {
            Converted = true;
            this.GetComponent<BoxCollider2D>().enabled = false;
            spriteRenderer.sprite = ConvertedSprites[SpriteState];
            nextSpriteChange = Time.time + AnimateInterval;
            ScoreManager.Instance.GameController.RegisterConversion(PointsValue);
        }
    }

    protected void Wander()
    {
        if (this.transform.localScale.x > 0)
        {
            this.transform.position += new Vector3(ScoreManager.Instance.GameController.NPCWalkSpeed, 0);
        }
        else
        {
            this.transform.position -= new Vector3(ScoreManager.Instance.GameController.NPCWalkSpeed, 0);
        }
    }

    protected virtual void DoFrameAction()
    {
        this.Wander();
    }
}
