
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class TakeScene : EditorWindow
{
    public List<string> scene;
    [MenuItem("Custom/Scene")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TakeScene));
    }

    #region Function
    public void StartGame()
    {
        if (!UnityEngine.Application.isPlaying)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Scenes/Home.unity");
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }
        }
        else EditorApplication.ExecuteMenuItem("Edit/Play");

    }
    public void PLay()
    {
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
    public void PlayScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_Scenes/Play.unity");
        }
    }
    public void LoadingScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_Scenes/Loading.unity");
        }
    }
    public void LoadScene(string scenename)
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Scenes/" + scenename + ".unity");
            }
        }
        else
        {
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }


    }
    private string[] ReadNames()
    {
        List<string> temp = new List<string>();
        //foreach (EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        foreach (EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
            {
                string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - 6);
                temp.Add(name);
            }
        }
        return temp.ToArray();
    }
    private string[] ReadScene()
    {
        List<string> temp = new List<string>();
        return temp.ToArray();
    }
    public void ClearAllData()
    {
        
        PlayerPrefs.DeleteAll();
    }
    int startNumber;
    string path= "/Data/Projects/D2M/";
    public void takeScreenShot(string _path)
    {
            int number = startNumber;
            string name = "" + number;

            while (System.IO.File.Exists(_path + "/" + name + ".png"))
            //while (System.IO.File.Exists("/Data/Projects/D2M/" + name + ".png"))
            {
                number++;
                name = "" + number;
            }
            startNumber = number + 1;
            //ScreenCapture.CaptureScreenshot("/Data/Projects/D2M/" + name + ".png");
            ScreenCapture.CaptureScreenshot(_path + "/" + name + ".png");
    }
    #endregion
    //private DefaultAsset targetFolder = null;
    string txtBtnHack= "Stop Hack level";
    string exp = "1";
    string gold = "1";
    Vector2 scrollPos;
    private void OnGUI()
    {
        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos);
        if (GUILayout.Button("START GAME"))
        {
            StartGame();
        }
        GUILayout.Label(">>>  DATA<<<");
        if (GUILayout.Button("CLEAR ALL DATA"))
        {
            ClearAllData();
        }
     
        GUILayout.Label(">>> CHANGE SCENE<<<");
        EditorGUILayout.BeginVertical();
        var allscene = ReadNames();
        for (int i = 0; i < allscene.Length; i++)
        {
            if (GUILayout.Button("change to " + allscene[i] + " scene"))
            {

                LoadScene(allscene[i]);
            }
        }
        EditorGUILayout.EndVertical();
        GUILayout.Label(">>> Take screen shot<<<");
        path = GUILayout.TextField(path);
        if (GUILayout.Button("Screen shot"))
        {
            takeScreenShot(path);
        }
        EditorGUILayout.EndScrollView();
    }
}

#endif