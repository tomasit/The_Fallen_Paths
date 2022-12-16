using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LevelChange : AInteractable
{
    [SerializeField] string [] _scenes;
    [SerializeField] string sceneToGo;
    [SerializeField] string scenesDirectory = "Assets/Scenes/";
    [SerializeField] string [] scenesSubDirectories = {"Levels/"};
    [SerializeField] EditorBuildSettingsScene [] _editorScenes;

    private void Start()
    {
        EditorBuildSettingsScene [] _editorScenes =  EditorBuildSettings.scenes;
        int idx = 0;

        _scenes = new string [_editorScenes.Length];

        foreach (var editorScene in _editorScenes)
        {
            string parsedPath = editorScene.path;
            
            parsedPath = parsedPath.Replace(scenesDirectory, "");
            foreach (var sceneSubDir in scenesSubDirectories)
                parsedPath = parsedPath.Replace(sceneSubDir, "");
            parsedPath = parsedPath.Replace(".unity", "");

            _scenes[idx] = parsedPath;
            ++idx;
        }
    }

    private void Update()
    {
    }

    public override void Interact()
    {
        foreach (var scene in _scenes)
        {
            if (scene == sceneToGo) {
                SceneManager.LoadScene(scene);
            }
        }
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }
}