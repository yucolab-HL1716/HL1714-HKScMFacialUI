using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using QRCoder;
using DG.Tweening;

public class QRCodeManager : MonoBehaviour {

    public RawImage rawImageQR;

	// === Core functions =================================================================
	// ----------------------------------------------------------------
	void Start () {
        ResetQR();
    }

    // ----------------------------------------------------------------
    public void ResetQR()
    {
        rawImageQR.texture = null;
        rawImageQR.DOFade(0, 0);
    }

    // ----------------------------------------------------------------
    public void GenerateQR(string url)
    {
        QRCoder.PayloadGenerator.Url generator = new QRCoder.PayloadGenerator.Url(url);
        string payload = generator.ToString();

        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(20);
        rawImageQR.texture = qrCodeAsTexture2D;
        rawImageQR.DOFade(1, 0.3f);
    }
    
}
