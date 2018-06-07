using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using DoozyUI;
using DG.Tweening;

public class Game3Manager : MonoSingleton<Game3Manager>
{

    public GameMode gameMode = GameMode.GAME3;
        
    public Image adjustedImg;
    public Image finalImg;
    public Image eraseStickerBtn;
    public Image eraseBinCoverImg;

    public SideBar sideBar;

    // ----------------------------------------------------------------
    public override void Init()
    {

    }

    // ----------------------------------------------------------------
    void Update()
    {

    }

    // ----------------------------------------------------------------
    public void ResetGame()
    {
        eraseStickerBtn.enabled = true;
        eraseBinCoverImg.enabled = true;
    }

    // ----------------------------------------------------------------
    private Sprite SaveTexture2DToSprite(Texture2D texture)
    {
        Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
        return sp;
    }

    // ----------------------------------------------------------------
    private Sprite SavePhotoToSprite()
    {
        Texture2D tex = SavePhotoToTexture2D();
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
        return sp;
    }

    // ----------------------------------------------------------------
    private Texture2D SavePhotoToTexture2D()
    {
        Texture2D tex = new Texture2D(1280, 650, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(SettingManager.instance.captureAnchorPosition.x, SettingManager.instance.captureAnchorPosition.y, 1280, 650), 0, 0);
        tex.Apply();
        return tex;
    }

    // ----------------------------------------------------------------
    IEnumerator SaveAnimalPhotoAndNextPage()
    {
        //yield return new WaitForSeconds(0.02f);
        yield return new WaitForEndOfFrame();

        // Save photo
        adjustedImg.sprite = SavePhotoToSprite();

        //yield return new WaitForSeconds(0.02f);
        yield return new WaitForEndOfFrame();
        UIManager.HideUiElement("G3_SelectAnimal", "FacialGame");
        UIManager.ShowUiElement("Decoration", "FacialGame");

        sideBar.SetIcon(2);
    }

    // ----------------------------------------------------------------
    IEnumerator SaveFinalPhotoAndNextPage()
    {
        eraseStickerBtn.enabled = false;
        eraseBinCoverImg.enabled = false;
        StickerManager.instance.gestureHint_drag.SetActive(false);
        //yield return new WaitForSeconds(0.02f);
        yield return new WaitForEndOfFrame();

        // Save photo
        Texture2D photoTex2D = SavePhotoToTexture2D();
        finalImg.sprite = SaveTexture2DToSprite(photoTex2D);

        // Save to file
        GameRoot.instance.SavePhotoToFile(photoTex2D);

        //yield return new WaitForSeconds(0.02f);
        yield return new WaitForEndOfFrame();
        UIManager.HideUiElement("Decoration", "FacialGame");
        UIManager.ShowUiElement("ShareOption", "FacialGame");

        sideBar.SetIcon(3);
    }

    // === UITrigger functions ================================================================================================================================
    // ----------------------------------------------------------------
    public void OnSelectAnimalArrowPressed()
    {
        StartCoroutine(SaveAnimalPhotoAndNextPage());
    }

    // ----------------------------------------------------------------
    public void OnDecorationArrowPressed()
    {
        StartCoroutine(SaveFinalPhotoAndNextPage());
    }

    // ----------------------------------------------------------------
    public void OnAnimalButtonClicked(int _id)
    {
        GameRoot.instance.SendUdpMessage("animal",  _id.ToString());
    }
}
