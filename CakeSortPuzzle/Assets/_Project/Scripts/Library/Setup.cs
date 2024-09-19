using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public static class Setup
{
    [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders()
    {
        Folders.CreateDefault("_Project", "Animation", "Art",
            "Scripts", "Materials", "Prefabs", "ScriptableObjects");

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Setup/Install Open Source Libraries")]
    public static void InstallOpenSourcePackages()
    {
        Packages.InstallPackages(new[]
        {
                "git+https://github.com/KyleBanks/scene-ref-attribute.git"
            });
    }

    [MenuItem("Tools/Setup/Import DoTween")]
    public static void ImportMyFavoriteAssets()
    {
        Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
    }

    static class Folders
    {
        public static void CreateDefault(string root, params string[] folders)
        {
            var fullPath = Path.Combine(Application.dataPath, root);
            foreach (var folder in folders)
            {
                var path = Path.Combine(fullPath, folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }

    static class Assets
    {
        public static void ImportAsset(string asset, string subFolder,
            string folder = "C:/Users/PC/AppData/Roaming/Unity/Asset Store-5.x")
        {
            AssetDatabase.ImportPackage(Path.Combine(folder, subFolder, asset), false);
        }
    }

    static class Packages
    {
        private static AddRequest Request;
        static Queue<string> PackagesToInstall = new Queue<string>();

        public static void InstallPackages(string[] packages)
        {
            foreach (var package in packages)
            {
                PackagesToInstall.Enqueue(package);
            }

            // Start the installation of the first package
            if (PackagesToInstall.Count > 0)
                Request = Client.Add(PackagesToInstall.Dequeue());
            EditorApplication.update += Progress;
        }

        static async void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                {
                    Debug.Log("Installed package: " + Request.Result.packageId);
                }
                else if (Request.Status == StatusCode.Failure)
                {
                    Debug.Log(Request.Error.message);
                }

                EditorApplication.update -= Progress;

                // If there are more packages to install, start the next one
                if (PackagesToInstall.Count > 0)
                {
                    // Add delay before next package install
                    await Task.Delay(1000);
                    Request = Client.Add(PackagesToInstall.Dequeue());
                    EditorApplication.update += Progress;
                }
            }
        }
    }
}