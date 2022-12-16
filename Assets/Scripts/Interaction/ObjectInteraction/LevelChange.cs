using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : AInteractable
{
    [SerializeField] private string sceneToGo;

    private void Start()
    {
    }

    private void Update()
    {
    }

    public override void Interact()
    {
        SceneManager.LoadScene(sceneToGo);
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }
}