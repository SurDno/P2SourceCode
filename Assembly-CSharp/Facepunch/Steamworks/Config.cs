using System;
using SteamNative;

namespace Facepunch.Steamworks
{
  public static class Config
  {
    public static void ForUnity(string platform)
    {
      if (platform == "WindowsEditor" || platform == "WindowsPlayer")
      {
        if (IntPtr.Size == 4)
          UseThisCall = false;
        ForcePlatform(OperatingSystem.Windows, IntPtr.Size == 4 ? Architecture.x86 : Architecture.x64);
      }
      if (platform == "OSXEditor" || platform == "OSXPlayer" || platform == "OSXDashboardPlayer")
        ForcePlatform(OperatingSystem.Osx, IntPtr.Size == 4 ? Architecture.x86 : Architecture.x64);
      if (platform == "LinuxPlayer" || platform == "LinuxEditor")
        ForcePlatform(OperatingSystem.Linux, IntPtr.Size == 4 ? Architecture.x86 : Architecture.x64);
      Console.WriteLine("Facepunch.Steamworks Unity: " + platform);
      Console.WriteLine("Facepunch.Steamworks Os: " + (object) Platform.Os);
      Console.WriteLine("Facepunch.Steamworks Arch: " + (object) Platform.Arch);
    }

    public static bool UseThisCall { get; set; } = true;

    public static void ForcePlatform(OperatingSystem os, Architecture arch)
    {
      Platform.Os = os;
      Platform.Arch = arch;
    }
  }
}
