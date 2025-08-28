#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostBuildIOS {
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {
        if (buildTarget != BuildTarget.iOS) return;

        var projPath = PBXProject.GetPBXProjectPath(path);
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);

        string target = proj.GetUnityFrameworkTargetGuid();

        // Force C++17
        proj.SetBuildProperty(target, "CLANG_CXX_LANGUAGE_STANDARD", "c++17");
        proj.SetBuildProperty(target, "CLANG_CXX_LIBRARY", "libc++");

        File.WriteAllText(projPath, proj.WriteToString());
    }
}
#endif
