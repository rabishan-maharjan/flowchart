#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using Arcube;

public class PostProcessFirebasePlist
{
    //[PostProcessBuild]
    // public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    // {
    //     if (buildTarget != BuildTarget.iOS)
    //         return;

    //     string plistPath = Path.Combine(path, "Info.plist");

    //     // Read plist
    //     PlistDocument plist = new PlistDocument();
    //     plist.ReadFromFile(plistPath);

    //     // Root dictionary
    //     PlistElementDict rootDict = plist.root;

    //     // Set FirebaseMessagingAutoInitEnabled = false
    //     rootDict.SetBoolean("FirebaseMessagingAutoInitEnabled", false);

    //     // Save the file
    //     File.WriteAllText(plistPath, plist.WriteToString());
    //     Log.Add(()=> "✔️ FirebaseMessagingAutoInitEnabled set to false in Info.plist");
    // }
}
#endif
