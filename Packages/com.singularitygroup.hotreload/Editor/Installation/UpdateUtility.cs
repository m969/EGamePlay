using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.Editor.Cli;
using SingularityGroup.HotReload.RuntimeDependencies;
using UnityEditor;
#if UNITY_EDITOR_WIN
using System.Diagnostics;
using Debug = UnityEngine.Debug;
#endif

namespace SingularityGroup.HotReload.Editor {
    static class UpdateUtility {
        public static async Task<string> Update(string version, IProgress<float> progress, CancellationToken cancellationToken) {
            await ThreadUtility.SwitchToThreadPool();

            string serverDir;
            if(!CliUtils.TryFindServerDir(out serverDir)) {
                progress?.Report(1);
                return "unable to locate hot reload package";
            }
            var packageDir = Path.GetDirectoryName(Path.GetFullPath(serverDir));
            var cacheDir = Path.GetFullPath(PackageConst.LibraryCachePath);
            if(Path.GetPathRoot(packageDir) != Path.GetPathRoot(cacheDir)) {
                progress?.Report(1);
                return "unable to update package because it is located on a different drive than the unity project";
            }
            var updatedPackageCopy = BackupPackage(packageDir, version);
            
            var key = $"{DownloadUtility.GetPackagePrefix(version)}/HotReload.zip";
            var url = DownloadUtility.GetDownloadUrl(key);
            var targetFileName = $"HotReload{version.Replace('.', '-')}.zip";
            var targetFilePath = CliUtils.GetTempDownloadFilePath(targetFileName);
            var proxy = new Progress<float>(f => progress?.Report(f * 0.7f));
            var result = await DownloadUtility.DownloadFile(url, targetFilePath, proxy, cancellationToken).ConfigureAwait(false);
            if(result.error != null) {
                progress?.Report(1);
                return result.error;
            }
            
            PackageUpdater.UpdatePackage(targetFilePath, updatedPackageCopy); 
            progress?.Report(0.8f);
            
            var packageRecycleBinDir = PackageConst.LibraryCachePath + $"/PackageArchived-{version}-{Guid.NewGuid():N}";
            try {
                Directory.Move(packageDir, packageRecycleBinDir);
                Directory.Move(updatedPackageCopy, packageDir);
            } catch {
                // fallback to replacing files individually if access to the folder is denied
                PackageUpdater.UpdatePackage(targetFilePath, packageDir); 
            }
            try {
                Directory.Delete(packageRecycleBinDir, true);
            } catch (IOException) {
                //ignored
            }
            
            progress?.Report(1);
            return null;
        }
        
        static string BackupPackage(string packageDir, string version) {
            var backupPath = PackageConst.LibraryCachePath + $"/PackageBackup-{version}";
            if(Directory.Exists(backupPath)) {
                Directory.Delete(backupPath, true);
            }
            DirectoryCopy(packageDir, backupPath);
            return backupPath;
        }
        
        static void DirectoryCopy(string sourceDirPath, string destDirPath) {
            var rootSource = new DirectoryInfo(sourceDirPath);

            var sourceDirs = rootSource.GetDirectories();
            // ensure destination directory exists
            Directory.CreateDirectory(destDirPath);

            // Get the files in the directory and copy them to the new destination
            var files = rootSource.GetFiles();
            foreach (var file in files) {
                string temppath = Path.Combine(destDirPath, file.Name);
                var copy = file.CopyTo(temppath);
                copy.LastWriteTimeUtc = file.LastWriteTimeUtc;
            }

            // copying subdirectories, and their contents to destination
            foreach (var subdir in sourceDirs) {
                string subDirDestPath = Path.Combine(destDirPath, subdir.Name);
                DirectoryCopy(subdir.FullName, subDirDestPath);
            }
        }
    }
}