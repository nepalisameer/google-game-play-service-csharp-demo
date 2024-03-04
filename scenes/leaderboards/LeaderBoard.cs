using Godot;
using GPGS;
using System;
using System.Collections.Generic;

namespace AndroidTest.scenes
{
    public partial class LeaderBoard : Control
    {
        [Export] Button BackButton;
        [Export] Button ShowLeaderBoards;
        [Export] VBoxContainer LeaderBoardDisplays;
        List<GPGS.LeaderBoard_GPGS> LeaderBoardsCache;
        [Export] PackedScene LeaderBoardDisplay;

        public override void _Ready()
        {
            LeaderboardsClient.Instance.AllLeaderBoardLoaded += Instance_AllLeaderBoardLoaded;
            LeaderboardsClient.Instance.LeaderBoardLoaded += Instance_LeaderBoardLoaded;
            BackButton.Pressed += BackButton_Pressed;
            ShowLeaderBoards.Pressed += ShowLeaderBoards_Pressed;
            if (LeaderBoardsCache == null || LeaderBoardsCache.Count <= 0)
            {
                LeaderboardsClient.Instance.LoadAllLeaderBoards(true);
            }
        }

        private void ShowLeaderBoards_Pressed()
        {
            LeaderboardsClient.Instance.ShowAllLeaderBoards();
        }

        private void BackButton_Pressed()
        {
            GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
        }

        private void Instance_AllLeaderBoardLoaded(List<GPGS.LeaderBoard_GPGS> obj)
        {
            LeaderBoardsCache = obj;
            if(LeaderBoardsCache != null && LeaderBoardsCache.Count >0)
            {
                foreach (var leaderBoard in LeaderBoardsCache)
                {
                    var container = LeaderBoardDisplay.Instantiate<LeaderBoardDisplay>();
                    container.SetLeaderBoard(leaderBoard);
                    LeaderBoardDisplays.AddChild(container);
                }
            }
        }

        private void Instance_LeaderBoardLoaded(GPGS.LeaderBoard_GPGS obj)
        {
            throw new NotImplementedException();
        }
    }
}
