using CI.HttpClient;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class ExampleSceneManagerController : MonoBehaviour
{
    public Text LeftText;
    public Text RightText;
    public Slider ProgressSlider;
    public Image originalImage;

    public void Upload()
    {
        HttpClient client = new HttpClient();

        byte[] buffer = new byte[1000000];
        new System.Random().NextBytes(buffer);

        ByteArrayContent content = new ByteArrayContent(buffer, "application/bytes");

        ProgressSlider.value = 0;

        client.Post(new System.Uri("http://httpbin.org/post"), content, HttpCompletionOption.AllResponseContent, (r) =>
        {           
        }, (u) =>
        {
            LeftText.text = "Upload: " +  u.PercentageComplete.ToString() + "%";
            ProgressSlider.value = u.PercentageComplete;
        });
    }

    public void Download()
    {
        HttpClient client = new HttpClient();

        ProgressSlider.value = 100;

        client.GetByteArray(new System.Uri("http://download.thinkbroadband.com/5MB.zip"), HttpCompletionOption.StreamResponseContent, (r) =>
        {
            RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
            ProgressSlider.value = 100 - r.PercentageComplete;
        });
    }

    public void UploadDownload()
    {
        HttpClient client = new HttpClient();

        byte[] buffer = new byte[1000000];
        new System.Random().NextBytes(buffer);

        ByteArrayContent content = new ByteArrayContent(buffer, "application/bytes");

        ProgressSlider.value = 0;

        client.Post(new System.Uri("http://httpbin.org/post"), content, HttpCompletionOption.StreamResponseContent, (r) =>
        {
            RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
            ProgressSlider.value = 100 - r.PercentageComplete;
        }, (u) =>
        {
            LeftText.text = "Upload: " + u.PercentageComplete.ToString() + "%";
            ProgressSlider.value = u.PercentageComplete;
        });
    }

    public void UploadImage()
    {
        YucoDebugger.instance.Log("Prepare to upload photo for QR", "OnShareQRButtonPressed", "GameRoot");

        string filename = "test";

        // Create HMAC string
        Encoding encoding = Encoding.UTF8;
        var keyByte = encoding.GetBytes("HKSM Children Zone Facial");
        string hmacString = "";
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            hmacsha256.ComputeHash(encoding.GetBytes(filename));
            hmacString = ByteToString(hmacsha256.Hash);
        }

        Byte[] photoBytes = originalImage.sprite.texture.EncodeToJPG(95);

        Dictionary<string, string> formData = new Dictionary<string, string>()
        {
            { "s", hmacString },
            { "d", filename }
        };
        StringContent stringContent = new StringContent("s=" + hmacString + "&d=" + filename, encoding);
        FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(formData);
        ByteArrayContent byteArrayContent = new ByteArrayContent(photoBytes, "application/bytes");
        MultipartContent multipartContent = new MultipartContent("formBoundary", "form-data");
        multipartContent.Add(byteArrayContent);
        multipartContent.Add(stringContent);

        HttpClient httpClient = new HttpClient();
        httpClient.Post(new System.Uri("http://localhost/hksmcz_facial/uploadImage.php"), multipartContent, HttpCompletionOption.AllResponseContent, (r) =>
        {
            YucoDebugger.instance.Log("OriginalRequest = " + r.OriginalRequest.RequestUri);
            YucoDebugger.instance.Log("Data = " + System.Text.Encoding.UTF8.GetString(r.Data));
            YucoDebugger.instance.Log("OrginalResponse = " + r.OriginalResponse);
            YucoDebugger.instance.Log("HTTP status code = " + r.StatusCode);
            YucoDebugger.instance.Log("Exception = " + r.Exception);
        }, (u) =>
        {
            YucoDebugger.instance.Log("Upload: " + u.PercentageComplete.ToString() + "%");
        });
    }
    // ----------------------------------------------------------------
    static string ByteToString(byte[] buff)
    {
        string sbinary = "";
        for (int i = 0; i < buff.Length; i++)
            sbinary += buff[i].ToString("X2"); /* hex format */
        return sbinary.ToLower();
    }



    public void Delete()
    {
        HttpClient client = new HttpClient();
        client.Delete(new System.Uri("http://httpbin.org/delete"), HttpCompletionOption.AllResponseContent, (r) =>
        {
        });
    }

    public void Get()
    {
        HttpClient client = new HttpClient();
        client.GetByteArray(new System.Uri("http://httpbin.org/get"), HttpCompletionOption.AllResponseContent, (r) =>
        {
        });
    }

    public void Patch()
    {
        HttpClient client = new HttpClient();

        StringContent content = new StringContent("Hello World");

        client.Patch(new System.Uri("http://httpbin.org/patch"), content, HttpCompletionOption.AllResponseContent, (r) =>
        {
        });
    }

    public void Post()
    {
        HttpClient client = new HttpClient();

        StringContent content = new StringContent("Hello World");

        client.Post(new System.Uri("http://httpbin.org/post"), content, HttpCompletionOption.AllResponseContent, (r) =>
        {
        });
    }

    public void Put()
    {
        HttpClient client = new HttpClient();

        StringContent content = new StringContent("Hello World");

        client.Put(new System.Uri("http://httpbin.org/put"), content, HttpCompletionOption.AllResponseContent, (r) =>
        {
        });
    }
}