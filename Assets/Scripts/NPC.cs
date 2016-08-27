using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{

    [Header("Sprites")]
    public Sprite[] UnconvertedSprites;
    public Sprite[] ConvertedSprites;

    public bool Converted;
    public float AnimateInterval;

    private int SpriteState;
    private float nextSpriteChange;
    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        nextSpriteChange = Time.time + AnimateInterval;
        SpriteState = 0;
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

    public void Hit()
    {
        Converted = true;
        this.GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer.sprite = ConvertedSprites[SpriteState];
        nextSpriteChange = Time.time + AnimateInterval;
    }
}
