using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GPGS
{

    public partial class LeaderboardsClient : Node
    {
        public delegate void ScoreSubmittedDelegate(bool isSubmitted, string leaderboardId);
        public delegate void ScoreLoadedDelegate(string leaderboardId, Score_GPGS score);
        public delegate void AllLeaderboardLoadedDelegate(List<LeaderBoard_GPGS> leaderBoards);
        public delegate void LeaderboardLoadedDelegate(LeaderBoard_GPGS leaderBoard);
        public static LeaderboardsClient Instance { get; private set; }

        /// <summary>
        /// Event raised when a score is submitted.
        /// </summary>
        public event ScoreSubmittedDelegate ScoreSubmitted;

        /// <summary>
        /// Event raised when a score is loaded.
        /// </summary>
        public event ScoreLoadedDelegate ScoreLoaded;

        /// <summary>
        /// Event raised when all leaderboards are loaded.
        /// </summary>
        public event AllLeaderboardLoadedDelegate AllLeaderBoardLoaded;

        /// <summary>
        /// Event raised when a specific leaderboard is loaded.
        /// </summary>
        public event LeaderboardLoadedDelegate LeaderBoardLoaded;

        public override void _Ready()
        {
            Instance = this;
            // Connects signals from the AndroidPlugin instance to corresponding methods
            GodotPlayGameService.Instance?.Plugin?.Connect("scoreSubmitted", new Callable(this, nameof(OnScoreSubmittedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("scoreLoaded", new Callable(this, nameof(OnScoreLoadedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("allLeaderboardsLoaded", new Callable(this, nameof(OnAllLeaderBoardLoadedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("leaderboardLoaded", new Callable(this, nameof(OnLeaderBoardLoadedSignalConnected)));
        }
        /// <summary>
        /// Invokes the ScoreSubmitted event with the provided parameters.
        /// </summary>
        /// <param name="isSubmitted">A boolean indicating whether the score is submitted.</param>
        /// <param name="leaderboardId">The ID of the leaderboard.</param>
        private void OnScoreSubmittedSignalConnected(bool isSubmitted, string leaderboardId)
        {
            ScoreSubmitted?.Invoke(isSubmitted, leaderboardId);
        }

        /// <summary>
        /// Invokes the ScoreLoaded event with the provided parameters after deserializing the JSON message.
        /// </summary>
        /// <param name="leaderboardId">The ID of the leaderboard.</param>
        /// <param name="jsonMessage">The JSON message containing the score information.</param>
        private void OnScoreLoadedSignalConnected(string leaderboardId, string jsonMessage)
        {
            try
            {
                Score_GPGS score = JsonSerializer.Deserialize<Score_GPGS>(jsonMessage);
                ScoreLoaded?.Invoke(leaderboardId, score);
            }
            catch (Exception)
            {
                // do something with the error
            }
        }

        /// <summary>
        /// Invokes the AllLeaderBoardLoaded event with the provided parameters after deserializing the JSON message.
        /// </summary>
        /// <param name="jsonMessage">The JSON message containing the list of leaderboards.</param>
        private void OnAllLeaderBoardLoadedSignalConnected(string jsonMessage)
        {
            try
            {
                List<LeaderBoard_GPGS> leaderBoards = JsonSerializer.Deserialize<List<LeaderBoard_GPGS>>(jsonMessage);
                AllLeaderBoardLoaded?.Invoke(leaderBoards);
            }
            catch (Exception)
            {
                // do something with the error
            }
        }

        /// <summary>
        /// Invokes the LeaderBoardLoaded event with the provided parameters after deserializing the JSON message.
        /// </summary>
        /// <param name="jsonMessage">The JSON message containing the leaderboard information.</param>
        private void OnLeaderBoardLoadedSignalConnected(string jsonMessage)
        {
            try
            {
                LeaderBoard_GPGS leaderBoard = JsonSerializer.Deserialize<LeaderBoard_GPGS>(jsonMessage);
                LeaderBoardLoaded?.Invoke(leaderBoard);
            }
            catch (Exception)
            {
                // do something with the error
            }
        }
        /// <summary>
        /// Show all leaderboards
        /// </summary>
        public void ShowAllLeaderBoards()
        {
            GodotPlayGameService.Instance?.Plugin?.Call("showAllLeaderboards");
        }

        /// <summary>
        /// Show a specific leaderboard
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to show</param>
        public void ShowLeaderBoard(string leaderBoardId)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("showLeaderboard", leaderBoardId);
        }

        /// <summary>
        /// Show a specific leaderboard for a given time span and collection
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to show</param>
        /// <param name="timeSpan">The time span for the leaderboard</param>
        /// <param name="collection">The collection for the leaderboard</param>
        public void ShowLeaderBoardForTimeSpanAndCollection(string leaderBoardId, TimeSpan_GPGS timeSpan, Collection_GPGS collection)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("showLeaderboardForTimeSpanAndCollection", leaderBoardId, (int)timeSpan, (int)collection);
        }

        /// <summary>
        /// Submit a score to a specific leaderboard
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to submit the score to</param>
        /// <param name="score">The score to submit</param>
        public void SubmitScore(string leaderBoardId, long score)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("submitScore", leaderBoardId, score);
        }

        /// <summary>
        /// Load the player's score for a specific leaderboard, time span, and collection
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to load the player's score from</param>
        /// <param name="timeSpan">The time span for the score</param>
        /// <param name="collection">The collection for the score</param>
        public void LoadPlayerScore(string leaderBoardId, TimeSpan_GPGS timeSpan, Collection_GPGS collection)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("loadPlayerScore", leaderBoardId, (int)timeSpan, (int)collection);
        }

        /// <summary>
        /// Load all leaderboards
        /// </summary>
        /// <param name="forceReload">True if the leaderboards should be reloaded</param>
        public void LoadAllLeaderBoards(bool forceReload)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("loadAllLeaderboards", forceReload);
        }

        /// <summary>
        /// Load a specific leaderboard
        /// </summary>
        /// <param name="leaderBoardId">The ID of the leaderboard to load</param>
        /// <param name="forceReload">True if the leaderboard should be reloaded</param>
        public void LoadLeaderBoard(string leaderBoardId, bool forceReload)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("loadLeaderboard", leaderBoardId, forceReload);
        }
    }
    public class Score_GPGS
    {
        public string displayRank { get; set; }
        public string displayScore { get; set; }
        public int rank { get; set; }
        public int rawScore { get; set; }
        public Player_GPGS scoreHolder { get; set; }
        public string scoreHolderDisplayName { get; set; }
        public string scoreHolderHiResImageUri { get; set; }
        public string scoreHolderIconImageUri { get; set; }
        public string scoreTag { get; set; }
        public int timestampMillis { get; set; }

    }

    public class LeaderBoard_GPGS
    {
        public string leaderboardId { get; set; }
        public string displayName { get; set; }
        public string iconImageUri { get; set; }
        public string scoreOrder { get; set; }
        public LeaderboardVariant_GPGS[] variants { get; set; }
    }
    public class LeaderboardVariant_GPGS
    {
        public string displayPlayerRank { get; set; }
        public string displayPlayerScore { get; set; }
        public int numScores { get; set; }
        public int playerRank { get; set; }
        public string playerScoreTag { get; set; }
        public int rawPlayerScore { get; set; }
        public bool hasPlayerInfo { get; set; }
        public string collection { get; set; }
        public string timeSpan { get; set; }
    }
    public enum ScoreOrder_GPGS
    {
        SCORE_ORDER_LARGER_IS_BETTER = 1, // Scores are sorted in descending order.
        SCORE_ORDER_SMALLER_IS_BETTER = 0 // Scores are sorted in ascending order.
    }
    public enum Collection_GPGS
    {
        COLLECTION_PUBLIC = 0, // A public leaderboard.
        COLLECTION_FRIENDS = 3 // A leaderboard only with friends.
    }
    public enum TimeSpan_GPGS
    {
        TIME_SPAN_DAILY = 0, // A leaderboard that resets everyday.
        TIME_SPAN_WEEKLY = 1, // A leaderboard that resets every week.
        TIME_SPAN_ALL_TIME = 2 // A leaderboard that never resets.
    }
}
