using System.IO;

namespace Engine.Source.Unity;

public static class FileUtility {
	public static void CleanFolder(string folder) {
		if (Directory.Exists(folder))
			Directory.Delete(folder, true);
		Directory.CreateDirectory(folder);
	}

	public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target) {
		foreach (var directory in source.GetDirectories())
			CopyFilesRecursively(directory, target.CreateSubdirectory(directory.Name));
		foreach (var file in source.GetFiles())
			file.CopyTo(Path.Combine(target.FullName, file.Name));
	}
}