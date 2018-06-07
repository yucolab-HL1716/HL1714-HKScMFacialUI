using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerManager : MonoSingleton<StickerManager>
{
    public GameObject prefab_sticker;

    public List<Sticker> stickers;
    public List<StickerButton> stickerButtons;
    public Transform maskStickerHolder;
    public RectTransform maskRT;
    public Rect maskRect;

    public EraseStickerButton eraseButton;

    public GameObject gestureHint_drag;
    public GameObject gestureHint_rotate;

    // SFX
    public AudioSource audioSource_main;
    public AudioClip audio_eraseSticker;

    // ----------------------------------------------------------------
    public override void Init()
    {
        maskRect = GetWorldRectFromRT(maskRT);
    }

    // ----------------------------------------------------------------
    void LateUpdate () {
        // Erase bin animation
        if(stickers.Count > 0)
        {
            bool shouldOpen = stickers.Count(s => s.isInsideEraseButton == true) > 0;
            eraseButton.SetWasteBinOpen(shouldOpen);
        }
        else
        {
            eraseButton.SetWasteBinOpen(false);
        }
    }

    // ----------------------------------------------------------------
    public void ClearAllStickers()
    {
        foreach(Sticker s in stickers)
        {
            Destroy(s.gameObject);
        }
        stickers.Clear();

        SetButtonsInteractable(true);
        gestureHint_drag.SetActive(true);
        gestureHint_rotate.SetActive(false);

        eraseButton.Reset();
    }

    // ----------------------------------------------------------------
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (StickerButton b in stickerButtons)
        {
            b.SetInteractable(interactable);
        }
    }

    // ----------------------------------------------------------------
    public Transform AddSticker(int _id, Vector2 _pos)
    {
        gestureHint_drag.SetActive(false);

        if (stickers.Count >= SettingManager.instance.maxStickerCount)
        {
            return null;
        }
        else
        {
            GameObject stickerObj = Instantiate(prefab_sticker, this.transform, false);
            Sticker sticker = stickerObj.GetComponent<Sticker>();
            sticker.Setup(_id, _pos);
            stickers.Add(sticker);
            
            if (stickers.Count >= SettingManager.instance.maxStickerCount)
            {
                SetButtonsInteractable(false);
            }

            return stickerObj.transform;
        }
        
    }

    // ----------------------------------------------------------------
    public void RemoveSticker(Sticker _s)
    {
        audioSource_main.PlayOneShot(audio_eraseSticker);

        Destroy(_s.gameObject);
        stickers.Remove(_s);
        if (stickers.Count < SettingManager.instance.maxStickerCount)
        {
            SetButtonsInteractable(true);
        }
    }

    // ----------------------------------------------------------------
    public static Rect GetWorldRectFromRT(RectTransform _rt)
    {
        Vector3[] corners = new Vector3[4];
        _rt.GetWorldCorners(corners);
        float minX = corners.Min(c => c.x);
        float minY = corners.Min(c => c.y);
        float maxX = corners.Max(c => c.x);
        float maxY = corners.Max(c => c.y);
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}
