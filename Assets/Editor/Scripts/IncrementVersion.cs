using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class IncrementVersion : IPreprocessBuildWithReport {
  public int callbackOrder => 1;

  public void OnPreprocessBuild(BuildReport report) {
    ReadVersionAndIncrement();
  }

  static void ReadVersionAndIncrement() {
    string[] parts = PlayerSettings.bundleVersion.Trim().Split('.');

    int major = int.Parse(parts[0]);
    int minor = int.Parse(parts[1]);
    int build = int.Parse(parts[2]) + 1;
    int number = major * 10000 + minor * 1000 + build;

    PlayerSettings.Android.bundleVersionCode = number;
    PlayerSettings.iOS.buildNumber = number.ToString();
    PlayerSettings.bundleVersion = string.Format(
      "{0}.{1}.{2}",
      major.ToString("0"),
      minor.ToString("0"),
      build.ToString("000")
    );
  }
}
