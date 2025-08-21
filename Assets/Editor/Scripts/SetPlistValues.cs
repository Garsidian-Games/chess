using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class SetPlistValues : IPostprocessBuildWithReport {
  public int callbackOrder => 0;

  public void OnPostprocessBuild(BuildReport report) {
    if (report.summary.platform == BuildTarget.iOS) {
#if UNITY_IOS
      string plistPath = $"{report.summary.outputPath}/Info.plist";

      PlistDocument plist = new();
      plist.ReadFromString(File.ReadAllText(plistPath));

      PlistElementDict rootDict = plist.root;
      rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

      File.WriteAllText(plistPath, plist.WriteToString());
#endif
    }
  }
}