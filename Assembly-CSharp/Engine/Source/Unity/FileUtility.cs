using System.IO;

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
