using DodgeThemAll.addons.GodotPlayGameServices.autoloads;
using Godot;
using Godot.Collections;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPGS
{

    public partial class SnapShotsClient : Node
    {
        public delegate void GameSavedDelegate(bool isSaved, string uniqueName, string description);
        public delegate void GameLoadedDelegate(SnapShot_GPGS gameInfo);
        public delegate void ConflictEmittedDelgate(SnapshotConflict_GPGS gameInfo);
        public static SnapShotsClient Instance { get; private set; }
        /// <summary>
        /// Event raised when a game is saved
        /// </summary>
        public event GameSavedDelegate GameSaved;
        /// <summary>
        /// Event raised when a game is loaded
        /// </summary>
        public event GameLoadedDelegate GameLoaded;
        /// <summary>
        /// Event raised when a snapshot conflict occurs
        /// </summary>
        public event ConflictEmittedDelgate ConflictEmitted;
        public override void _Ready()
        {
            Instance = this;
            // Connects signals from the AndroidPlugin instance to corresponding methods
            GodotPlayGameService.Instance?.Plugin?.Connect("gameSaved", new Callable(this, nameof(OnGameSavedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("gameLoaded", new Callable(this, nameof(OnGameLoadedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("conflictEmitted", new Callable(this, nameof(OnConflictEmittedSignalConnected)));
        }
        /// <summary>
        /// Handles the signal when a game is saved and invokes the GameSaved event.
        /// </summary>
        /// <param name="isSaved">Indicates whether the game is saved or not.</param>
        /// <param name="uniqueName">The unique name of the game.</param>
        /// <param name="description">The description of the game.</param>
        private void OnGameSavedSignalConnected(bool isSaved, string uniqueName, string description)
        {
            GameSaved?.Invoke(isSaved, uniqueName, description);
        }
        /// <summary>
        /// Handles the signal when a game is Loaded and invokes the GameLoaded event.
        /// </summary>
        /// <param name="gameInfo">A dictionary containing the game information.</param>
        private void OnGameLoadedSignalConnected(string gameInfo)
        {
            SnapShot_GPGS snapShot = new();
            if (gameInfo != null)
            {
                try
                {
                    var deserializeOptions = new JsonSerializerOptions();
                    deserializeOptions.Converters.Add(new ByteArrayConverter());
                    snapShot = JsonSerializer.Deserialize<SnapShot_GPGS>(gameInfo,deserializeOptions);
                }
                catch (Exception)
                {
                    // TODO: Handle the error in a meaningful way
                }
            }
            GameLoaded?.Invoke(snapShot);
        }

        /// <summary>
        /// Handles the conflictEmitted signal and invokes the ConflictEmitted event.
        /// </summary>
        /// <param name="gameInfo">The game information dictionary.</param>
        private void OnConflictEmittedSignalConnected(string gameInfo)
        {
            SnapshotConflict_GPGS conflict = new SnapshotConflict_GPGS();
            if (gameInfo != null)
            {
                try
                {
                    // Deserialize the game information string to a SnapshotConflict object
                    var deserializeOptions = new JsonSerializerOptions();
                    deserializeOptions.Converters.Add(new ByteArrayConverter());
                    conflict = JsonSerializer.Deserialize<SnapshotConflict_GPGS>(gameInfo,deserializeOptions);
                }
                catch (Exception)
                {
                    // Handle the error when deserialization fails
                }
            }
            // Invoke the ConflictEmitted event with the deserialized conflict
            ConflictEmitted?.Invoke(conflict);
        }
        /// <summary>
        /// Shows the saved games using the specified file name, description, saved data, played time in milliseconds, and progress value.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="description">The description of the saved game.</param>
        /// <param name="savedData">The data of the saved game.</param>
        /// <param name="playedTimeMillis">The time played in milliseconds (default is 0).</param>
        /// <param name="progressValue">The progress value (default is 0).</param>
        public void SaveGame(string fileName, string description, byte[] savedData, long playedTimeMillis = 0, long progressValue = 0)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("saveGame", fileName, description, savedData, playedTimeMillis, progressValue);
        }

        /// <summary>
        /// Loads the game with the specified file name.
        /// </summary>
        /// <param name="fileName">The name of the file to load.</param>
        public void LoadGame(string fileName,bool createIfNotFound=false)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("loadGame", fileName,createIfNotFound);
        }
    }
    public class SnapShot_GPGS
    {
        public byte[] content { get; set; }
        public SnapshotMetadata_GPGS metadata { get; set; }
    }
    public class SnapshotConflict_GPGS
    {
        public string conflictId { get; set; }
        public SnapShot_GPGS conflictingSnapshot { get; set; }
        public SnapShot_GPGS serverSnapshot { get; set; }
    }
    public class SnapshotMetadata_GPGS
    {

        public string snapshotId { get; set; } // The ID of the snapshot.
        public string uniqueName { get; set; } // The unique identifier of this snapshot. This is the file_name parameter passed to the save_game method.
        public string description { get; set; } // The description of the snapshot. This is the description parameter passed to the save_game method.
        public float coverImageAspectRatio { get; set; } // The aspect ratio of the cover image for this snapshot.
        public int progressValue { get; set; } // The progress value for this snapshot.
        public long lastModifiedTimestamp { get; set; } // The last time this snapshot was modified, in milliseconds since epoch.
        public long playedTime { get; set; } // The played time of this snapshot in milliseconds.
        public bool hasChangePending { get; set; } // Indicates whether or not this snapshot has any changes pending that have not been uploaded to the server.
        public Player_GPGS owner { get; set; } // The player that owns this snapshot.
        public GameInfo_GPGS game { get; set; } // The game information associated with this snapshot.
        public string deviceName { get; set; } // The name of the device that wrote this snapshot, if known.
        public string coverImageUri { get; set; } // The snapshot cover image.

    }
    public class GameInfo_GPGS
    {
        public bool areSnapshotsEnabled { get; set; } // Indicates whether or not this game supports snapshots.
        public int achievementTotalCount { get; set; } // The number of achievements registered for this game.
        public string applicationId { get; set; } // The application ID for this game.
        public string description { get; set; } // The description of this game.
        public string developerName { get; set; } // The name of the developer of this game.
        public string displayName { get; set; } // The display name for this game.
        public int leaderboardCount { get; set; } // The number of leaderboards registered for this game.
        public string primaryCategory { get; set; } // The primary category of the game.
        public string secondaryCategory { get; set; } // The secondary category of the game.
        public string themeColor { get; set; } // The theme color for this game.
        public bool hasGamepadSupport { get; set; } // Indicates whether or not this game is marked as supporting gamepads.
        public string hiResImageUri { get; set; } // The game's hi-res image.
        public string iconImageUri { get; set; } // The game's icon.
        public string featuredImageUri { get; set; } // The game's featured (banner) image from Google Play.
    }
}
