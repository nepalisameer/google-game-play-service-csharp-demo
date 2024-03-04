using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GPGS
{
    public partial class PlayersClient : Node
    {
        public delegate void FirendsLoadedDelegate(List<Player_GPGS> friends);
        public delegate void PlayerSearchedLoadedDelegate(Player_GPGS player);
        public static PlayersClient Instance { get; private set; }
        /// <summary>
        /// Event raised when the list of friends is loaded.
        /// </summary>
        public event FirendsLoadedDelegate FriendsLoaded;

        /// <summary>
        /// Event raised when a player is searched.
        /// </summary>
        public event PlayerSearchedLoadedDelegate PlayerSearched;

        /// <summary>
        /// Event raised when the current player is loaded.
        /// </summary>
        public event PlayerSearchedLoadedDelegate CurrentPlayerLoaded;
        public override void _Ready()
        {
            Instance = this;
            // Connects signals from the AndroidPlugin instance to corresponding methods
            GodotPlayGameService.Instance?.Plugin?.Connect("friendsLoaded", new Callable(this, nameof(OnFriendsLoadedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("playerSearched", new Callable(this, nameof(OnFriendsLoadedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("currentPlayerLoaded", new Callable(this, nameof(OnCurrentPlayerLoadedSignalConnected)));
        }

        /// <summary>
        /// Loads friends with the specified page size, optionally forcing a reload and asking for permission.
        /// </summary>
        /// <param name="pageSize">The number of friends to load per page.</param>
        /// <param name="forceReload">True to force a reload of friends, false otherwise.</param>
        /// <param name="askForPermission">True to ask for permission, false otherwise.</param>
        public void LoadFriends(int pageSize, bool forceReload, bool askForPermission)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("loadFriends", pageSize, forceReload, askForPermission);
        }

        /// <summary>
        /// Compares the profile with the specified player ID.
        /// </summary>
        /// <param name="otherPlayerId">The ID of the other player to compare the profile with.</param>
        public void CompareProfile(string otherPlayerId)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("compareProfile", otherPlayerId);
        }

        /// <summary>
        /// Compares the profile with alternative name hints for the specified player IDs and names.
        /// </summary>
        /// <param name="otherPlayerId">The ID of the other player to compare the profile with.</param>
        /// <param name="otherPlayerInGameName">The in-game name of the other player.</param>
        /// <param name="currentPlayerInGameName">The in-game name of the current player.</param>
        public void CompareProfileWithAlternativeNameHints(string otherPlayerId, string otherPlayerInGameName, string currentPlayerInGameName)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("compareProfileWithAlternativeNameHints", otherPlayerId, otherPlayerInGameName, currentPlayerInGameName);
        }

        /// <summary>
        /// Initiates a search for players.
        /// </summary>
        public void SearchPlayer()
        {
            GodotPlayGameService.Instance?.Plugin?.Call("searchPlayer");
        }

        /// <summary>
        /// Loads the current player, optionally forcing a reload.
        /// </summary>
        /// <param name="forceReload">True to force a reload of the current player, false otherwise.</param>
        public void LoadCurrentPlayer(bool forceReload)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("loadCurrentPlayer", forceReload);
        }
        /// <summary>
        /// Handle the signal when friends are loaded
        /// </summary>
        /// <param name="playerListJson">The JSON string representing the list of players</param>
        private void OnFriendsLoadedSignalConnected(string playerListJson)
        {
            try
            {
                List<Player_GPGS> players = JsonSerializer.Deserialize<List<Player_GPGS>>(playerListJson);
                FriendsLoaded?.Invoke(players);
            }
            catch (Exception)
            {
                // Handle the error when deserialization fails
            }
        }

        /// <summary>
        /// Handle the signal when a player is searched
        /// </summary>
        /// <param name="playerListJson">The JSON string representing the player</param>
        private void OnPlayerSearchedSignalConnected(string playerListJson)
        {
            try
            {
                Player_GPGS player = JsonSerializer.Deserialize<Player_GPGS>(playerListJson);
                PlayerSearched?.Invoke(player);
            }
            catch (Exception)
            {
                // Handle the error when deserialization fails
            }
        }

        /// <summary>
        /// Handle the signal when the current player is loaded
        /// </summary>
        /// <param name="playerListJson">The JSON string representing the current player</param>
        private void OnCurrentPlayerLoadedSignalConnected(string playerListJson)
        {
            try
            {
                Player_GPGS player = JsonSerializer.Deserialize<Player_GPGS>(playerListJson);
                CurrentPlayerLoaded?.Invoke(player);
            }
            catch (Exception)
            {
                // Handle the error when deserialization fails
            }
        }
    }

    public class Player_GPGS
    {
        public string bannerImageLandscapeUri { get; set; }
        public string bannerImagePortraitUri { get; set; }
        public string friendsListVisibilityStatus { get; set; }
        public string displayName { get; set; }
        public string hiResImageUri { get; set; }
        public string iconImageUri { get; set; }
        public PlayerLevelInfo_GPGS levelInfo { get; set; }
        public string playerId { get; set; }
        public string friendStatus { get; set; }
        public long retrievedTimestamp { get; set; }
        public string title { get; set; }
        public bool hasHiResImage { get; set; }
        public bool hasIconImage { get; set; }
    }
    public class PlayerLevelInfo_GPGS
    {
        public PlayerLevel_GPGS currentLevel { get; set; }
        public int currentXpTotal { get; set; }
        public long lastLevelUpTimestamp { get; set; }
        public PlayerLevel_GPGS nextLevel { get; set; }
        public bool isMaxLevel { get; set; }

    }
    public class PlayerLevel_GPGS
    {
        public int levelNumber { get; set; }
        public int maxXp { get; set; }
        public int minXp { get; set; }
    }
    public enum FriendsListVisibilityStatus_GPGS
    {
        FEATURE_UNAVAILABLE = 3, // The friends list is currently unavailable for the game.
        REQUEST_REQUIRED = 2, // The friends list is not visible to the game, but the game can ask for access.
        UNKNOWN = 0, // Unknown if the friends list is visible to the game, or whether the game can ask for access from the user.
        VISIBLE = 1 // The friends list is currently visible to the game.
    }
    public enum PlayerFriendStatus_GPGS
    {
        FRIEND = 4, // The currently signed in player and this player are friends.
        NO_RELATIONSHIP = 0, // The currently signed in player is not a friend of this player.
        UNKNOWN = -1 // The currently signed in player's friend status with this player is unknown.
    }
}
