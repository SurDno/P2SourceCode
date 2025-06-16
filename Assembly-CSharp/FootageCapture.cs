// Decompiled with JetBrains decompiler
// Type: FootageCapture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;

#nullable disable
public class FootageCapture : MonoBehaviour
{
  private const string FolderName = "Capture";
  private const string FileExtension = ".png";
  public int CaptureFramerate = 30;
  public int SuperSize = 0;
  public KeyCode RecordKey = KeyCode.F9;
  public KeyCode ScreenshotKey = KeyCode.F12;
  private bool isCapturing = false;
  private string currentClipFolder;
  private int currentFrame;

  private string TimeToString() => DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

  private void CheckCaptureFolder()
  {
    if (Directory.Exists("Capture"))
      return;
    Directory.CreateDirectory("Capture");
  }

  private void Update()
  {
    if (this.isCapturing)
    {
      ScreenCapture.CaptureScreenshot(this.currentClipFolder + this.currentFrame.ToString("0000") + ".png", this.SuperSize);
      ++this.currentFrame;
    }
    else if (Input.GetKeyDown(this.ScreenshotKey))
    {
      this.CheckCaptureFolder();
      ScreenCapture.CaptureScreenshot("Capture/" + this.TimeToString() + ".png", this.SuperSize);
    }
    if (!Input.GetKeyDown(this.RecordKey))
      return;
    if (!this.isCapturing)
    {
      this.currentFrame = 0;
      this.CheckCaptureFolder();
      string str = this.TimeToString();
      this.currentClipFolder = "Capture/" + str;
      if (!Directory.Exists(this.currentClipFolder))
        Directory.CreateDirectory(this.currentClipFolder);
      this.currentClipFolder = this.currentClipFolder + "/" + str + " ";
      Time.captureFramerate = this.CaptureFramerate;
      this.isCapturing = true;
    }
    else
    {
      this.isCapturing = false;
      Time.captureFramerate = 0;
    }
  }
}
