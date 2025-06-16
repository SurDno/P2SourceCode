using System;
using System.IO;

public class FootageCapture : MonoBehaviour
{
  private const string FolderName = "Capture";
  private const string FileExtension = ".png";
  public int CaptureFramerate = 30;
  public int SuperSize = 0;
  public KeyCode RecordKey = KeyCode.F9;
  public KeyCode ScreenshotKey = KeyCode.F12;
  private bool isCapturing;
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
    if (isCapturing)
    {
      ScreenCapture.CaptureScreenshot(currentClipFolder + currentFrame.ToString("0000") + ".png", SuperSize);
      ++currentFrame;
    }
    else if (Input.GetKeyDown(ScreenshotKey))
    {
      CheckCaptureFolder();
      ScreenCapture.CaptureScreenshot("Capture/" + TimeToString() + ".png", SuperSize);
    }
    if (!Input.GetKeyDown(RecordKey))
      return;
    if (!isCapturing)
    {
      currentFrame = 0;
      CheckCaptureFolder();
      string str = TimeToString();
      currentClipFolder = "Capture/" + str;
      if (!Directory.Exists(currentClipFolder))
        Directory.CreateDirectory(currentClipFolder);
      currentClipFolder = currentClipFolder + "/" + str + " ";
      Time.captureFramerate = CaptureFramerate;
      isCapturing = true;
    }
    else
    {
      isCapturing = false;
      Time.captureFramerate = 0;
    }
  }
}
