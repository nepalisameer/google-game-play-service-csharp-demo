using Godot;
using GPGS;
using System.Collections.Generic;
using System.Linq;

namespace AndroidTest.scenes
{
    public partial class Player:Control
    {
        [Export] Button BackButton;
        [Export] Button SearchButton;
        [Export] VBoxContainer SearchDisplay;
        [Export] VBoxContainer CurrentPlayerDisplay;
        [Export] VBoxContainer FriendDisplay;
        Player_GPGS CurrentPlayer;

        List<Player_GPGS> FriendsCache;
        [Export] PackedScene PlayerDisplay;
        public override void _Ready()
        {
            BackButton.Pressed += BackButton_Pressed;
            SearchButton.Pressed += SearchButton_Pressed;
            PlayersClient.Instance.CurrentPlayerLoaded += Instance_CurrentPlayerLoaded;  
            if(CurrentPlayer == null)
            {
                PlayersClient.Instance.LoadCurrentPlayer(true);

            }
            PlayersClient.Instance.FriendsLoaded += Instance_FriendsLoaded;
            if(FriendsCache == null)
            {
                PlayersClient.Instance.LoadFriends(10, true, true);
            }
            PlayersClient.Instance.PlayerSearched += Instance_PlayerSearched;
        }

        private void SearchButton_Pressed()
        {
            PlayersClient.Instance.SearchPlayer();
        }

        private void BackButton_Pressed()
        {
            GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
        }

        private void Instance_PlayerSearched(Player_GPGS obj)
        {
            var childerns = SearchDisplay.GetChildren();
            if (childerns !=null && childerns.Count()> 0)
            {
                foreach (var child in childerns)
                {
                    child.QueueFree();
                }
            }
            var container = PlayerDisplay.Instantiate<PlayerDisplay>();
            container.SetPlayer(obj);
            SearchDisplay.AddChild(container);
        }

        private void Instance_FriendsLoaded(List<Player_GPGS> obj)
        {
            FriendsCache = obj;
            if(FriendsCache!=null && FriendsCache.Count > 0)
            {
                foreach (var item in FriendsCache)
                {
                    var container = PlayerDisplay.Instantiate<PlayerDisplay>();
                    container.SetPlayer(item);
                    FriendDisplay.AddChild(container);
                }
            }
        }

        private void Instance_CurrentPlayerLoaded(Player_GPGS obj)
        {
            var container = PlayerDisplay.Instantiate<PlayerDisplay>();
            container.SetPlayer(obj);
            CurrentPlayerDisplay.AddChild(container);
        }
    }
}
