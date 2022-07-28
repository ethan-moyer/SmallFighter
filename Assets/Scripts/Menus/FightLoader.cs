using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FightLoader : MonoBehaviour
{
    private const string fightingBaseScene = "Scenes/FightingBase";

    public static FightLoader instance;
    public string stageScenePath;
    public GameObject[] fighterPrefabs = new GameObject[2];
    public InputDevice[] fighterDevices = new InputDevice[2];
    public string[] controlSchemes = new string[2];

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void LoadStage()
    {
        SceneManager.LoadScene(fightingBaseScene);
        SceneManager.LoadScene(stageScenePath, LoadSceneMode.Additive);
    }
}
