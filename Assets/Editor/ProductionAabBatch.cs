using System;
using UnityEditor;
using UnityEngine;

// Batch: Setup production (BuildDatabase.SetupAndroidPrd) rồi build AAB production (BuildAndroid.BuildAAB) như menu của project.
// Mật khẩu keystore/alias lấy từ biến môi trường WC_KEYSTORE_PASS lúc chạy (config để trống, không ghi file).
// Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
public class ProductionAabBatch
{
    public static void Build()
    {
        EditorApplication.Exit(new ProductionAabBatch().Run() ? 0 : 1);
    }

    private bool Run()
    {
        var password = Environment.GetEnvironmentVariable("WC_KEYSTORE_PASS");
        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("[ProductionAab] thiếu biến môi trường WC_KEYSTORE_PASS");
            return false;
        }
        Util.Editor.BuildDatabase.SetupAndroidPrd();
        PlayerSettings.Android.keystorePass = password;
        PlayerSettings.Android.keyaliasPass = password;
        var started = DateTime.Now;
        Util.Editor.Android.BuildAndroid.BuildAAB();
        // BuildAAB ghi vào <project>/Build/*.aab: lấy file mới nhất tạo sau lúc bắt đầu.
        var folder = System.IO.Path.GetFullPath(Application.dataPath + "/../Build");
        string newest = null;
        foreach (var file in System.IO.Directory.GetFiles(folder, "*.aab"))
        {
            if (System.IO.File.GetLastWriteTime(file) < started) continue;
            if (newest == null || System.IO.File.GetLastWriteTime(file) > System.IO.File.GetLastWriteTime(newest)) newest = file;
        }
        Debug.Log("[ProductionAab] " + (newest != null ? "OK " + newest : "FAILED"));
        return newest != null;
    }
}
