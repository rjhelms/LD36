using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{

    [Header("Sprites")]
    public Sprite[] UnconvertedSprites;
    public Sprite[] ConvertedSprites;

    public bool Converted;
    public bool HasComeOnScreen;
    public float AnimateInterval;

    private int SpriteState;
    private float nextSpriteChange;
    private SpriteRenderer spriteRenderer;
    private GameController gameController;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        gameController = FindObjectOfType<GameController>();

        nextSpriteChange = Time.time + AnimateInterval;
        SpriteState = 0;
        HasComeOnScreen = false;
    }

    // Update is called once per frame
    void Update()
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
        if (HasComeOnScreen && !Converted)
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
    }

    public void Hit()
    {
        Converted = true;
        this.GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer.sprite = ConvertedSprites[SpriteState];
        nextSpriteChange = Time.time + AnimateInterval;
    }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bottom")
            HasComeOnScreen = true;
        else if (coll.gameObject.tag == "Bounds" || coll.gameObject.tag == "NPC")
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, 1, 1);
        }
    }
}
