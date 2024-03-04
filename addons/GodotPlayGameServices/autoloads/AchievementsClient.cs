using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GPGS
{
    public partial class AchievementsClient : Node
    {
        public delegate void AchievementUnlockedDelegate(bool isUnlocked, string achievementId);
        public delegate void AchievementRevealedDelegate(bool isRevealed, string achievementId);
        public delegate void AchievementLoadedDelegate(List<Achievement_GPGS> achievements);
        /// <summary>
        /// Represents the achievements client.
        /// </summary>
        public static AchievementsClient Instance { get; private set; }

        /// <summary>
        /// Event raised when an achievement is unlocked.
        /// </summary>
        public event AchievementUnlockedDelegate AchievementUnlocked;

        /// <summary>
        /// Event raised when achievements are loaded.
        /// </summary>
        public event AchievementLoadedDelegate AchievementsLoaded;

        /// <summary>
        /// Event raised when an achievement is revealed.
        /// </summary>
        public event AchievementRevealedDelegate AchievementRevealed;
        public override void _Ready()
        {
            Instance = this;
            // Connects signals from the AndroidPlugin instance to corresponding methods
            GodotPlayGameService.Instance?.Plugin?.Connect("achievementUnlocked", new Callable(this, nameof(OnAchievementUnlockedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("achievementsLoaded", new Callable(this, nameof(OnAchievementLoadedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("achievementRevealed", new Callable(this, nameof(OnAchievementRevealedSignalConnected)));
        }
        /// <summary>
        /// Increments the specified achievement by the given amount.
        /// </summary>
        /// <param name="achievementId">The ID of the achievement to increment.</param>
        /// <param name="amount">The amount to increment the achievement by.</param>
        public void IncrementAchievement(string achievementId, long amount)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("incrementAchievement", achievementId, amount);
        }

        /// <summary>
        /// Loads the user's achievements from the server.
        /// </summary>
        /// <param name="forceReload">If true, force a reload of the achievements from the server.</param>
        public void LoadAchievements(bool forceReload)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("loadAchievements", forceReload);
        }

        /// <summary>
        /// Reveals a hidden achievement to the player.
        /// </summary>
        /// <param name="achievementId">The ID of the achievement to reveal.</param>
        public void RevealAchievement(string achievementId)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("revealAchievement", achievementId);
        }

        /// <summary>
        /// Shows the achievements UI to the player.
        /// </summary>
        public void ShowAchievements()
        {
            GodotPlayGameService.Instance?.Plugin?.Call("showAchievements");
        }

        /// <summary>
        /// Unlocks the specified achievement.
        /// </summary>
        /// <param name="achievementId">The ID of the achievement to unlock.</param>
        public void UnlockAchievement(string achievementId)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("unlockAchievement", achievementId);
        }

        /// <summary>
        /// Connects the achievement unlocked signal and invokes the AchievementUnlocked event.
        /// </summary>
        /// <param name="isUnlocked">A boolean indicating whether the achievement is unlocked or not.</param>
        /// <param name="achievementId">The ID of the achievement.</param>
        private void OnAchievementUnlockedSignalConnected(bool isUnlocked, string achievementId)
        {
            AchievementUnlocked?.Invoke(isUnlocked, achievementId);
        }

        /// <summary>
        /// Connects the achievement loaded signal and invokes the AchievementsLoaded event.
        /// </summary>
        /// <param name="jsonMessage">The JSON message containing the achievement data</param>
        private void OnAchievementLoadedSignalConnected(string jsonMessage)
        {
            try
            {
                List<Achievement_GPGS> achievements = JsonSerializer.Deserialize<List<Achievement_GPGS>>(jsonMessage);
                AchievementsLoaded?.Invoke(achievements);
            }
            catch (Exception)
            {
                // Handle the error when deserializing the JSON message
            }
        }

        /// <summary>
        /// Connects the achievement revealed signal and invokes the AchievementRevealed event.
        /// </summary>
        /// <param name="isRevealed">A boolean indicating whether the achievement is revealed</param>
        /// <param name="achievementId">The ID of the revealed achievement</param>
        private void OnAchievementRevealedSignalConnected(bool isRevealed, string achievementId)
        {
            AchievementRevealed?.Invoke(isRevealed, achievementId);
        }
    }
    public class Achievement_GPGS
    {
        public string achievementId { get; set; } // The achievement id.
        public string achievementName { get; set; } // The achievement name.
        public string description { get; set; } // The description of the achievement.
        public Player_GPGS player { get; set; } // The player associated to this achievement.
        public string type { get; set; } // The achievement type.
        public string state { get; set; } // The achievement state.
        public int xpValue { get; set; } // The XP value of this achievement.
        public string revealedImageUri { get; set; } // A URI that can be used to load the achievement's revealed image icon.
        public string unlockedImageUri { get; set; } // A URI that can be used to load the achievement's unlocked image icon.
        // The number of steps this user has gone toward unlocking this achievement;
        // only applicable for TYPE_INCREMENTAL achievement types.
        public int currentSteps { get; set; }
        // Retrieves the total number of steps necessary to unlock this achievement; 
        // only applicable for TYPE_INCREMENTAL achievement types.
        public int totalSteps { get; set; }
        // Retrieves the number of steps this user has gone toward unlocking this
        // achievement, formatted for the user's locale; only applicable for 
        // TYPE_INCREMENTAL achievement types.
        public string formattedCurrentSteps { get; set; }
        // Loads the total number of steps necessary to unlock this achievement,
        // formatted for the user's local; only applicable for TYPE_INCREMENTAL 
        // achievement types.
        public string formattedTotalSteps { get; set; }
        // Retrieves the timestamp (in milliseconds since epoch) at which this achievement 
        // was last updated.
        public int lastUpdatedTimestamp { get; set; }
    }
    public enum AchievementType_GPGS
    {
        TYPE_STANDARD = 0, // A standard achievement.
        TYPE_INCREMENTAL = 1 // An incremental achievement.
    }

    //Achievement state.
    public enum AchievementState_GPGS
    {
        STATE_UNLOCKED = 0, // An unlocked achievement.
        STATE_REVEALED = 1, // A revealed achievement.
        STATE_HIDDEN = 2 // A hidden achievement.
    }
}
