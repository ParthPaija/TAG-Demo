using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskInteraction : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderersForInside;
    public SpriteRenderer[] spriteRenderersForOutside;
    public GameObject[] spriteMask;

    //set mask interaction to none
    public void SetMaskInteractionNone()
    {
        for (int i = 0; i < spriteRenderersForInside.Length; i++)
        {
            spriteRenderersForInside[i].maskInteraction = SpriteMaskInteraction.None;
        }
        for (int i = 0; i < spriteRenderersForOutside.Length; i++)
        {
            spriteRenderersForOutside[i].maskInteraction = SpriteMaskInteraction.None;
        }
    }

    //set mask interaction to visible inside mask
    public void SetMaskInteractionVisibleInsideMask()
    {
        for (int i = 0; i < spriteRenderersForInside.Length; i++)
        {
            spriteRenderersForInside[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    //set mask interaction to visible outside mask
    public void SetMaskInteractionVisibleOutsideMask()
    {
        for (int i = 0; i < spriteRenderersForOutside.Length; i++)
        {
            spriteRenderersForOutside[i].maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }

    //turn off sprite mask
    public void TurnOffSpriteMask()
    {
        for (int i = 0; i < spriteMask.Length; i++)
        {
            spriteMask[i].SetActive(false);
        }
    }

    //turn on sprite mask
    public void TurnOnSpriteMask()
    {
        for (int i = 0; i < spriteMask.Length; i++)
        {
            spriteMask[i].SetActive(true);
        }
    }
}
