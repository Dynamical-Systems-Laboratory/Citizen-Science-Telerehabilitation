using System.IO;
using UnityEditor;

namespace UnityEngine.UDP.Editor
{
#pragma warning disable all
#if !DISABLE_UDP_INSTALLER
    [InitializeOnLoad]
#endif
    // We cannot upload an installer to packman. So, this is for assetstore and unityiap.
    public class UdpInstaller
    {
        private const string Version = "2.0.0"; // Modified with gradle applyPackageVersion
        private static bool UnityIAP_Version = false;
        private const string UdpInstallerFolder = "Assets/UDPInstaller/Editor";
        private const string UdpPackagePath = UdpInstallerFolder + "/UDP.unitypackage";
        private const string UdpInstallerPath = UdpInstallerFolder + "/UdpInstaller.cs";
        private const string UdpImportedPlayerPrefKey = "UdpImportedFromInstaller";
        private static bool packmanUdp = false;
        private static readonly string[] possibleUdpFolder = {"Assets/Plugins/UDP", "Assets/UDP"};

        private static readonly string[] folderAndFiles =
        {
            "Android/", "Editor/", "Sample/", "Resources/", "UDP.dll", "Android.meta",
            "Editor.meta", "Sample.meta", "Resources.meta", "UDP.dll.meta", "UdpSupport/", "UdpSupport.meta"
        };

        static UdpInstaller()
        {
            if (!PlayerPrefs.HasKey(UdpImportedPlayerPrefKey))
            {
                if (UdpHasBeenInstalled())
                {
#pragma warning disable all

                    if (packmanUdp)
                    {
                        if (EditorUtility.DisplayDialog("UDP Installer",
                            "UDP is already installed via Packman. Please uninstall it first and then install this one.",
                            "OK")
                        )
                        {
                            Clean();
                            return;
                        }
                    }

                    if (UnityIApInstalled() && UnityIAP_Version)
                    {
                        RemoveOldUdpFiles();
                        // UnityIAP replaces UnityIAP
                        InstallPackage(false);
                    }
                    else
                    {
                        // UDP replaces UDP
                        string message =
                            string.Format(
                                "UDP is already installed. Do you want to reinstall this version {0} of the UDP package?",
                                Version);

                        if (UnityIApInstalled())
                        {
                            // UDP replaces UnityIAP
                            message =
                                "UDP is already installed via Unity IAP. Do you want to uninstall the UDP component of Unity IAP, so you can use this UDP package instead?";
                        }

                        if (UnityIAP_Version)
                        {
                            // UnityIAP replaces UDP
                            message =
                                "UDP is already installed as a stand-alone package. Do you want to uninstall your current UDP package, so you can use the UDP implementation of Unity IAP?";
                        }

                        // Ask the developer what to do
                        if (EditorUtility.DisplayDialog("UDP Installer", message, "OK", "Cancel"))
                        {
                            RemoveOldUdpFiles();
                            // Install
                            InstallPackage(true);
                        }
                    }
                }
                else
                {
                    InstallPackage(true);
                }
            }

            Clean();
        }

        static void RemoveOldUdpFiles()
        {
            foreach (string folder in possibleUdpFolder)
            {
                foreach (var file in folderAndFiles)
                {
                    string path = string.Format("{0}/{1}", folder, file);
                    if (file.EndsWith("/"))
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }
                    }
                    else
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
            }
        }

        static void InstallPackage(bool interactive)
        {
            // UnityIAP has its own installer. If interactive==true, UDP package won't be imported
            AssetDatabase.ImportPackage(UdpPackagePath, UnityIAP_Version ? false : interactive);
        }

        private const string ManifestFilePath = "Packages/manifest.json";
        private const string UdpPackageName = "com.unity.purchasing.udp";

        static bool UdpHasBeenInstalled()
        {
            foreach (var folder in possibleUdpFolder)
            {
                if (File.Exists(folder + "/UDP.dll"))
                {
                    return true;
                }
            }

            // Check if UDP is installed via Packman
            if (!File.Exists(ManifestFilePath))
            {
                packmanUdp = false;
                return false;
            }

            string manifest = File.ReadAllText(ManifestFilePath);
            if (manifest.IndexOf(UdpPackageName) >= 0)
            {
                packmanUdp = true;
                return true;
            }

            return false;
        }

        static bool UnityIApInstalled()
        {
            if (File.Exists("Assets/Plugins/UnityPurchasing/Bin/Stores.dll"))
            {
                return true;
            }

            return false;
        }

        static void Clean()
        {
            PlayerPrefs.DeleteKey(UdpImportedPlayerPrefKey);
            AssetDatabase.DeleteAsset("Assets/UDPInstaller");
        }
    }
}