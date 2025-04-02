using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase;
using Firebase.Extensions;
using System.Collections;
using Firebase.Auth;

[Serializable]
public class ScoreEntry
{
    public string name;
    public long score;

    public ScoreEntry(string name, long score)
    {
        this.name = name;
        this.score = score;
    }
}

// This script is used to manage global variables that need to be shared between objects and scene.
// It also manages the saving and loading for variable that need to be stored.
// To use this in other object in other scene, use the following code:
// GlobalVariableManager gvm = FindFirstObjectByType<GlobalVariableManager>();
// Note: This script must be attached ONCE ONLY to a GameObject in the start scene!
public class GlobalVariableManager : MonoBehaviour
{
    [Tooltip("The default settings for the game")]
    public GameSettingsIO gameSettings;
    [Tooltip("When true, in editor it will always use default settings.")]
    public bool UseDefaultSettingsInEditor = true;

    private bool _gamePlaying;
    public int MaxLeaderBoardSize = 10;
    public List<ScoreEntry> LeaderBoardData = new List<ScoreEntry>();
    public bool isFirebaseInitialized = false;
    protected FirebaseApp app;
    public FirebaseFirestore db;
    protected FirebaseAuth auth;
    public FirebaseUser currentUser;
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
        // Load the settings from the file
        if (!Application.isEditor || !UseDefaultSettingsInEditor)
        {
            LoadSettings();
        }
        else
        {
            // If in editor, use the default settings
            gameSettings = Instantiate(gameSettings);
            DontDestroyOnLoad(gameSettings);
        }
        // Initialize Firebase and Firestore
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                currentUser = auth.CurrentUser;
                isFirebaseInitialized = true;
                Debug.Log("Firebase initialized successfully.");
                StartCoroutine(LoadLeaderboardData());
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                isFirebaseInitialized = false;
            }
        });
    }

    void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != currentUser)
        {
            bool signedIn = currentUser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && currentUser != null)
            {
                Debug.Log("Signed out " + currentUser.UserId);
            }
            currentUser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + currentUser.UserId);
                // You could potentially trigger UI changes or data loading here
            }
        }
    }

    // Load the settings if the file exists
    public void LoadSettings()
    {
        gameSettings = Instantiate(gameSettings);
        DontDestroyOnLoad(gameSettings);
        if (File.Exists(settingFilePath))
        {
            string json = File.ReadAllText(settingFilePath);
            JsonUtility.FromJsonOverwrite(json, gameSettings);
        }
    }


    // Save the settings to a file
    public void SaveSettings()
    {
        if (!UseDefaultSettingsInEditor && gameSettings != null)
        {
            string json = JsonUtility.ToJson(gameSettings, prettyPrint: true);
            File.WriteAllText(settingFilePath, json);
            Debug.Log("Game setting saved to:" + settingFilePath);
        }
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }



    private class LeaderBoardRawData
    {
        public string name;
        public long score;
        public Timestamp timestamp;
        public string documentId;

        public LeaderBoardRawData(string name, long score, Timestamp timestamp, string documentId)
        {
            this.name = name;
            this.score = score;
            this.timestamp = timestamp;
            this.documentId = documentId;
        }
    }

    public async Task<List<ScoreEntry>> GetLeaderboardDataAsync()
    {
        if (!isFirebaseInitialized || db == null)
        {
            Debug.LogError("Firebase Firestore is not initialized.");
            return null;
        }

        Debug.Log("Start loading data...");
        List<ScoreEntry> LoadedData = new List<ScoreEntry>();

        try
        {
            CollectionReference leaderboardRef = db.Collection("leaderboard");
            Query query = leaderboardRef
                .OrderByDescending("score")
                .Limit(MaxLeaderBoardSize + 5);

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            Debug.Log($"Firestore query returned {querySnapshot.Count} documents.");

            List<LeaderBoardRawData> dbData = new List<LeaderBoardRawData>();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    try
                    {
                        string name = documentSnapshot.GetValue<string>("name");
                        long score = documentSnapshot.GetValue<long>("score");
                        Timestamp timestamp = documentSnapshot.GetValue<Timestamp>("time");
                        string documentId = documentSnapshot.Id;

                        if (!string.IsNullOrEmpty(name) && timestamp != null)
                        {
                            dbData.Add(new LeaderBoardRawData(name, score, timestamp, documentId)); //, documentSnapshot.Id));
                        }
                        else
                        {
                            Debug.LogWarning($"Document {documentSnapshot.Id} has invalid data: Name or Timestamp is null/empty.");
                        }
                    }
                    catch (Exception Error)
                    {
                        Debug.LogWarning($"Error occurred when processing {documentSnapshot.Id}: {Error.Message}");
                    }
                }
            }

            // Same sorting logic with web leaderboard
            dbData.Sort((a, b) =>
            {
                int scoreComparison = b.score.CompareTo(a.score);
                if (scoreComparison != 0)
                {
                    return scoreComparison;
                }
                return a.timestamp.CompareTo(b.timestamp);
            });

            for (int i = 0; i < dbData.Count; i++)
            {
                if (i < 10)
                {
                    LoadedData.Add(new ScoreEntry(dbData[i].name, dbData[i].score));
                }
            }
            Debug.Log("successfully loaded data from Firestore.");
            return LoadedData;

        }
        catch (Exception Error)
        {
            Debug.LogError($"Error occurred when loading data: {Error}");
            return null;
        }
    }

    public void LoginUser(string email, string password)
    {
        if (auth == null)
        {
            Debug.LogError("Firebase Auth not initialized!");
            return;
        }

        Debug.Log($"Attempting to login with email: {email}");

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"SignInWithEmailAndPasswordAsync encountered an error: {task.Exception}");
                return;
            }

            // Login successful
            currentUser = task.Result.User;
            Debug.Log($"User signed in successfully: {currentUser.DisplayName} ({currentUser.UserId})");
        });
    }

    // Uploads data to Firestore
    public bool UploadScoreToFirestore(string playerName, long playerScore)
    {
        if (!isFirebaseInitialized)
        {
            Debug.LogError("Firebase Firestore is not initialized.");
            OnUploadScoreEvent?.Invoke(false);
            return false;
        }

        CollectionReference scoresRef = db.Collection("leaderboard");

        Dictionary<string, object> scoreData = new Dictionary<string, object>
        {
            { "name", playerName },
            { "score", playerScore },
            { "time", FieldValue.ServerTimestamp }
        };

        Debug.Log($"Attempting to add score data");

        scoresRef.AddAsync(scoreData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Score data added successfully to Firestore. Document ID: " + task.Result.Id);
                OnUploadScoreEvent?.Invoke(true);
                StartCoroutine(LoadLeaderboardData());
            }
            else if (task.IsFaulted)
            {
                Debug.LogError($"Error adding score data to Firestore: {task.Exception}");
                OnUploadScoreEvent?.Invoke(false);
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("AddAsync was canceled.");
                OnUploadScoreEvent?.Invoke(false);
            }
        });
        return true;
    }

    public IEnumerator LoadLeaderboardData()
    {
        if (!isFirebaseInitialized || db == null)
        {
            Debug.LogError("Firebase Firestore is not initialized.");
            yield break;
        }
        Task.Run(() => GetLeaderboardDataAsync()).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result != null)
            {
                LeaderBoardData = task.Result;
                OnLeaderboardLoadedEvent?.Invoke(LeaderBoardData);
            }
            else
            {
                Debug.LogError("Failed to load leaderboard data.");
            }
        });
    }

    public event Action<bool> OnUploadScoreEvent;

    public event Action<List<ScoreEntry>> OnLeaderboardLoadedEvent;


    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
        // app, auth, db might be null if initialization failed
        auth = null;
        db = null;
        app = null;
    }
}