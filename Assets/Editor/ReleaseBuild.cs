using System;
using System.Collections.Generic;
using System.IO;
using HybridCLR;
using HybridCLR.Commands;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Build release Android (APK để cài máy + AAB cho Google Play): không GAME_DEBUG/DEVELOPER_LOG, ký bằng keystore thật.
// Mật khẩu keystore và alias đọc từ biến môi trường WC_KEYSTORE_PASS lúc chạy, chỉ gán vào PlayerSettings trong bộ nhớ (không ghi file, không log).
// Biến môi trường tuỳ chọn: WC_KEYSTORE_PATH (mặc định tìm wordCross.keystore ở thư mục cha của project), WC_RELEASE_OUT (thư mục xuất).
// Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
public class ReleaseBuild
{
    private const string PasswordVariable = "WC_KEYSTORE_PASS";
    private const string KeystoreFile = "wordCross.keystore";
    private const string KeyAlias = "wordcross";

    public static void Build()
    {
        EditorApplication.Exit(new ReleaseBuild().Run() ? 0 : 1);
    }

    private bool Run()
    {
        var password = Environment.GetEnvironmentVariable(PasswordVariable);
        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("[ReleaseBuild] thiếu biến môi trường " + PasswordVariable);
            return false;
        }
        var keystore = FindKeystore();
        if (keystore == null)
        {
            Debug.LogError("[ReleaseBuild] không tìm thấy " + KeystoreFile);
            return false;
        }

        var target = NamedBuildTarget.Android;
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = keystore;
        PlayerSettings.Android.keystorePass = password;
        PlayerSettings.Android.keyaliasName = KeyAlias;
        PlayerSettings.Android.keyaliasPass = password;

        PlayerSettings.GetScriptingDefineSymbols(target, out string[] defines);
        var list = new List<string>(defines);
        list.RemoveAll(item => item == "GAME_DEBUG" || item == "DEVELOPER_LOG" || item == "DEVELOPMENT");
        PlayerSettings.SetScriptingDefineSymbols(target, list.ToArray());
        Debug.Log("[ReleaseBuild] defines=" + string.Join(";", list) + " keystore=" + keystore);

        RefreshAotMetadata();

        var outDir = Environment.GetEnvironmentVariable("WC_RELEASE_OUT");
        if (string.IsNullOrEmpty(outDir)) outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../Build/release"));
        Directory.CreateDirectory(outDir);
        var name = "wordcrossy_" + PlayerSettings.bundleVersion + "_" + PlayerSettings.Android.bundleVersionCode + "_RELEASE";

        EditorUserBuildSettings.buildAppBundle = false;
        if (!BuildPackage(Path.Combine(outDir, name + ".apk"))) return false;
        EditorUserBuildSettings.buildAppBundle = true;
        return BuildPackage(Path.Combine(outDir, name + ".aab"));
    }

    // Strip lại AOT theo link.xml đã commit (đầy đủ), rồi làm mới bản metadata nhúng trong StreamingAssets/clr.
    private void RefreshAotMetadata()
    {
        StripAOTDllCommand.GenerateStripedAOTDlls(BuildTarget.Android);
        MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper(BuildTarget.Android);
        var stripDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(BuildTarget.Android);
        var copied = 0;
        foreach (var file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/clr", "*.dll.bytes"))
        {
            var source = Path.Combine(stripDir, Path.GetFileName(file).Replace(".dll.bytes", ".dll"));
            if (!File.Exists(source)) continue;
            File.Copy(source, file, true);
            copied++;
        }
        Debug.Log("[ReleaseBuild] metadata refreshed=" + copied);
    }

    private bool BuildPackage(string path)
    {
        var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.Android, BuildOptions.None);
        Debug.Log("[ReleaseBuild] " + path + " result=" + report.summary.result + " size=" + report.summary.totalSize);
        return report.summary.result == BuildResult.Succeeded;
    }

    private string FindKeystore()
    {
        var explicitPath = Environment.GetEnvironmentVariable("WC_KEYSTORE_PATH");
        if (!string.IsNullOrEmpty(explicitPath)) return File.Exists(explicitPath) ? explicitPath : null;
        var directory = new DirectoryInfo(Path.Combine(Application.dataPath, ".."));
        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, KeystoreFile);
            if (File.Exists(candidate)) return candidate;
            directory = directory.Parent;
        }
        return null;
    }
}
