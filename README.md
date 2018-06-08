# **HL1714 HKScM Facial Symmetry** - Unity3D GUI program

* **Author**: Eric Koo (eric.koo@yucolab.com)

## Application Description
* Frontend Unity3D GUI program in HKScM Children Zone: Facial Symmetry (3 kiosks)
* 32" Elo touchscreen program, 1920x1080 single monitor
* Face detection, animal face decoration, photo capture and countdown are handled by background openFrameworks program ([HL1714-HKScMFacialCam](https://github.com/yucolab/HL1714-HKScMFacialCam))
* Communicate with OF app via TCP (as server) and share rendering frames using Spout protocol
* Communicate with local hosted XAMPP server () to save game records, upload photos for downloading via QR, and email photos to visitors

## Development Environment

* **Windows 10**
* **Unity 2017.4.1f1 (64-bit)**

## Main 3rd-party Frameworks/Libraries

* [Demigiant DOTween Pro](http://dotween.demigiant.com/pro.php): Tween Library
* [DoozyUI](https://assetstore.unity.com/packages/tools/gui/doozyui-complete-ui-management-system-47352): UI management system
* [TextMesh Pro](http://digitalnativestudios.com/): UI text and Text mesh solution
* [TouchScript](https://assetstore.unity.com/packages/tools/input-management/touchscript-7394): Multitouch gesture framework
* [REST Client for Unity](https://github.com/Unity3dAzure/RESTClient)
* [QRCoder](https://github.com/codebude/QRCoder): Generate QR code with .NET
* [LitJSON](https://github.com/LitJSON/litjson)
* [KlakSpout](https://github.com/keijiro/KlakSpout): Share render frames with openFrameworks application using Spout protocol.

## Folder Structure of Main Components
```
HKSMChildrenZoneUI/                             --- Main Unity3D project folder
    Assets/
        Animations/                             --- Animations and Controllers
        Editor/YucoUtils/
        Fonts/
        Plugins/                                
        Prefabs/
        RenderTextures/                         --- RenderTexture for Receiving Spout from OF
        Resources/
        SFX/                                    --- Audio SFX files
        Scenes/                                 --- Unity3D .scene folder
        Scripts/
        Sprites/                                --- All images and sprites of the program
        StreamingAssets/                        --- Config files, copied to "_Data" folder after built
    Build/                                      --- Build folder
        HKSMCZ_FacialUI_Data/                   --- Assets and runtime libraries for execuatble
            StreamingAssets/
                facialUI_cms/                   --- Contains CMS configurations, shared folder to HKScM staff network
                    settings.ini
                internal/                       --- Contains internal configurations
                    settings.ini
        HKSMCZ_FacialUI.exe/                    --- Main executable
```

## Setup Guide
