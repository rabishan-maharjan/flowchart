#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class IOSPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS) return;

        // Path to Info.plist
        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

        // Read plist
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;

        // Add required keys
        rootDict.SetBoolean("UIRequiresFullScreen", true); // fullscreen mode
        rootDict.SetBoolean("UIApplicationSupportsIndirectInputEvents", true);
        rootDict.SetBoolean("UIRequiresPersistentSystemGestures", true); // 👈 disables bottom edge delay

        // Save plist
        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
#endif