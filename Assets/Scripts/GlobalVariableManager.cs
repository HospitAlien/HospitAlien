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
    public LeaderBoardIO defaultLeaderBoard;

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
    private LeaderBoardIO leaderBoard;
    private static GlobalVariableManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            // if there is already an instance of this object, destroy this one
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Make sure this object is not destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);

        _gamePlaying = false;
        // Get the file path for the settings file
        settingFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
        leaderBoardFilePath = Path.Combine(Application.persistentDataPath, "LeaderBoard.json");
        // Load the settings from the file
        LoadSettings();
        LoadLeaderBoard();
    }

    // Load the settings if the file exists
    public void LoadSettings()
    {
        if (File.Exists(settingFilePath))
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
        string json = JsonUtility.ToJson(gameSettings, prettyPrint: true);
        File.WriteAllText(settingFilePath, json);
        Debug.Log("Game setting saved to:" + settingFilePath);
    }

    // Load the leaderboard if the file exists
    public void LoadLeaderBoard()
    {
        if (File.Exists(settingFilePath))
        {
            string json = File.ReadAllText(leaderBoardFilePath);
            JsonUtility.FromJsonOverwrite(json, defaultLeaderBoard);
        }
        else
        {
            // copy the default leader board tto avoid change the default
            leaderBoard = Instantiate(defaultLeaderBoard);
            Debug.Log("Using default leader board.");
        }
    }

    // Save the leaderboard to a file
    public void SaveLeaderBoard()
    {
        string json = JsonUtility.ToJson(defaultLeaderBoard, prettyPrint: true);
        File.WriteAllText(leaderBoardFilePath, json);
        Debug.Log("LeaderBoard saved to:" + leaderBoardFilePath);
    }

    public LeaderBoardIO LeaderBoard
    {
        get { return leaderBoard; }
    }

    public void ClearLeaderBoard()
    {
        leaderBoard = Instantiate(defaultLeaderBoard);
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
        SaveLeaderBoard();
    }
}