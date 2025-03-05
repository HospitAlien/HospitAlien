using UnityEngine;
using System.IO;
using System;

// This script is used to manage global variables that need to be shared between objects and scene.
// It also manages the saving and loading for variable that need to be stored.
// To use this in other object in other scene, use the following code:
// GlobalVariableManager gvm = FindFirstObjectByType<GlobalVariableManager>();
// Note: This script must be attached ONCE ONLY to a GameObject in the start scene!
public class GlobalVariableManager : MonoBehaviour
{
    [Tooltip("The default settings for the game")]
    public GameSettingsIO gameSettings;
    [Tooltip("The default leader board for the game")]
    public LeaderBoardIO leaderBoard;

    private bool _gamePlaying;
    public event Action<bool> OnGamePlayingChangedEvent;
    public bool IsGamePlaying
    {
        get { return _gamePlaying; }
        set
        {
            if (_gamePlaying != value)
            {
                _gamePlaying = value;
                OnGamePlayingChangedEvent?.Invoke(value);
            }
        }
    }


    // Place to save the file
    private string settingFilePath;
    private string leaderBoardFilePath;

    private void Awake()
    {
        _gamePlaying = false;

        // Get the file path for the settings file
        settingFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
        leaderBoardFilePath = Path.Combine(Application.persistentDataPath, "LeaderBoard.json");
        // Load the settings from the file
        LoadSettings();
        LoadLeaderBoard();

        // Make sure this object is not destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    // Load the settings if the file exists
    public void LoadSettings()
    {
        if (!Application.isEditor && File.Exists(settingFilePath))
        {
            string json = File.ReadAllText(settingFilePath);
            JsonUtility.FromJsonOverwrite(json, gameSettings);
        }
        else
        {
            Debug.Log("Using default settings.");
            // copy the default settings to avoid change the default
            gameSettings = Instantiate(gameSettings);
        }
    }


    // Save the settings to a file
    public void SaveSettings()
    {
        if (!Application.isEditor)
        {
            string json = JsonUtility.ToJson(gameSettings, prettyPrint: true);
            File.WriteAllText(settingFilePath, json);
            Debug.Log("Game setting saved to:" + settingFilePath);
        }
    }

    // Load the leaderboard if the file exists
    public void LoadLeaderBoard()
    {
        if (!Application.isEditor && File.Exists(settingFilePath))
        {
            string json = File.ReadAllText(leaderBoardFilePath);
            JsonUtility.FromJsonOverwrite(json, leaderBoard);
        }
        else
        {
            // copy the default leader board tto avoid change the default
            leaderBoard = Instantiate(leaderBoard);
            Debug.Log("Using default leader board.");
        }
    }

    // Save the leaderboard to a file
    public void SaveLeaderBoard()
    {
        if (!Application.isEditor)
        {
            string json = JsonUtility.ToJson(leaderBoard, prettyPrint: true);
            File.WriteAllText(leaderBoardFilePath, json);
            Debug.Log("LeaderBoard saved to:" + leaderBoardFilePath);
        }
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
        SaveLeaderBoard();
    }
}