// Decompiled with JetBrains decompiler
// Type: Engine.Source.Unity.FileUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.IO;

#nullable disable
namespace Engine.Source.Unity
{
  public static class FileUtility
  {
    public static void CleanFolder(string folder)
    {
      if (Directory.Exists(folder))
        Directory.Delete(folder, true);
      Directory.CreateDirectory(folder);
    }

    public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
    {
      foreach (DirectoryInfo directory in source.GetDirectories())
        FileUtility.CopyFilesRecursively(directory, target.CreateSubdirectory(directory.Name));
      foreach (FileInfo file in source.GetFiles())
        file.CopyTo(Path.Combine(target.FullName, file.Name));
    }
  }
}
