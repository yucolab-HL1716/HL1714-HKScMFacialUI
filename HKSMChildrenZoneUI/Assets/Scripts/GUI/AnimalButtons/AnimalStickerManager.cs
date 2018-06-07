using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStickerManager : MonoSingleton<AnimalStickerManager>
{
    public GameObject prefab_sticker;

    public List<AnimalSticker> stickers;
    public List<AnimalButton> animalButtons;

    public Transform maskStickerHolder;
    public RectTransform maskRT;
    public Rect maskRect;
    
    // ----------------------------------------------------------------
    public override void Init()
    {
        maskRect = GetWorldRectFromRT(maskRT);
    }
    
    // ----------------------------------------------------------------
    public void ClearAllStickers()
    {
        foreach(AnimalSticker s in stickers)
        {
            Destroy(s.gameObject);
        }
        stickers.Clear();        
    }

    // ----------------------------------------------------------------
    public Transform AddSticker(int _id, Vector2 _pos)
    {
        GameObject stickerObj = Instantiate(prefab_sticker, this.transform, false);
        AnimalSticker sticker = stickerObj.GetComponent<AnimalSticker>();
        sticker.Setup(_id, _pos);
        stickers.Add(sticker);

        return stickerObj.transform;
        
    }

    // ----------------------------------------------------------------
    public void RemoveSticker(AnimalSticker _s)
    {
        Destroy(_s.gameObject);
        stickers.Remove(_s);
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
