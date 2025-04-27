using System.IO;

namespace SingularityGroup.HotReload.Editor.Util
{
  internal static class StringUtils
  {
    public static string NormalizePath(this string path)
    {
      return path.Replace(Path.DirectorySeparatorChar == '\\'
        ? '/'
        : '\\', Path.DirectorySeparatorChar);
    }
  }
}