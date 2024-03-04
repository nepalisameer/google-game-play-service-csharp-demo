using Godot;
using GPGS;
using GPGS.GooglePlayGameService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidTest.scenes
{
    public partial class LeaderBoardDisplay : Control
    {
        [Export] TextureRect IconRect;
        [Export] Label IdLabel;
        [Export] Label NameLabel;
        [Export] Label PlayerRankLabel;
        [Export] Label PlayerScoreLabel;
        [Export] LineEdit NewScoreLineEdit;
        [Export] Button SubmitScoreButton;
        [Export] OptionButton TimeSpanOption;
        [Export] OptionButton CollectionOption;
        [Export] Button ShowVariantButton;
        GPGS.LeaderBoard_GPGS CurrentLeaderBoard;
        long EmptyScore = -1;
        Score_GPGS CurrentScore;
        long NewRawScore = -1;
        GPGS.TimeSpan_GPGS SelectedTimeSpan;
        Collection_GPGS SelectedCollection;

        public override void _Ready()
        {
            LeaderboardsClient.Instance.ScoreLoaded += Instance_ScoreLoaded;
            LeaderboardsClient.Instance.ScoreSubmitted += Instance_ScoreSubmitted;
            NewScoreLineEdit.TextChanged += NewScoreLineEdit_TextChanged;
            SubmitScoreButton.Pressed += SubmitScoreButton_Pressed;
            TimeSpanOption.ItemSelected += TimeSpanOption_ItemSelected;
            CollectionOption.ItemSelected += CollectionOption_ItemSelected;
            ShowVariantButton.Pressed += ShowVariantButton_Pressed;
            if (CurrentLeaderBoard != null)
            {
                GodotPlayGameService.Instance.ImageStored += Instance_ImageStored;
                IdLabel.Text = CurrentLeaderBoard.leaderboardId;
                NameLabel.Text = CurrentLeaderBoard.displayName;
                SetUpDisplayScore();
                SetUpSubmitScore();
                SetUpVariants();
            }
        }
        public void SetLeaderBoard(GPGS.LeaderBoard_GPGS leaderBoard)
        {
            CurrentLeaderBoard = leaderBoard;
        }
        private void ShowVariantButton_Pressed()
        {
            LeaderboardsClient.Instance.ShowLeaderBoardForTimeSpanAndCollection(CurrentLeaderBoard.leaderboardId,SelectedTimeSpan,SelectedCollection);
        }

        private void CollectionOption_ItemSelected(long index)
        {
            SelectedCollection = (GPGS.Collection_GPGS)(int)index;
            GD.Print(SelectedCollection.ToString());

        }

        private void TimeSpanOption_ItemSelected(long index)
        {
            SelectedTimeSpan = (GPGS.TimeSpan_GPGS)(int)index;
        }

        private void SubmitScoreButton_Pressed()
        {
            if(NewRawScore != EmptyScore)
            {
                LeaderboardsClient.Instance.SubmitScore(CurrentLeaderBoard.leaderboardId, NewRawScore);
            }
        }
        private void SetUpVariants()
        {
            foreach (GPGS.TimeSpan_GPGS item in Enum.GetValues<GPGS.TimeSpan_GPGS>())
            {
                TimeSpanOption.AddItem(item.ToString());
            }
            foreach (GPGS.Collection_GPGS item in Enum.GetValues<GPGS.Collection_GPGS>())
            {
                CollectionOption.AddItem(item.ToString());
            }
            SelectedTimeSpan = (GPGS.TimeSpan_GPGS)TimeSpanOption.Selected;
            SelectedCollection= (GPGS.Collection_GPGS)CollectionOption.Selected;
            ShowVariantButton.Disabled = false;

        }
        private void NewScoreLineEdit_TextChanged(string newText)
        {
            if(long.TryParse(newText,out long value))
            {
                NewRawScore = value;
            }else
            {
                NewRawScore = EmptyScore;
            }
            RefreshSubmitScoreButton();
        }

        private void RefreshSubmitScoreButton()
        {
            if(NewRawScore ==  EmptyScore)
            {
                SubmitScoreButton.Text = "Type a Score";
                SubmitScoreButton.Disabled = true;
            }
            else
            {
                SubmitScoreButton.Text =$"Submit {NewRawScore} to score";
                SubmitScoreButton.Disabled = false;
            }
        }

        private void Instance_ScoreSubmitted(bool arg1, string arg2)
        {
            if(arg1 && arg2 == CurrentLeaderBoard.leaderboardId)
            {
                LoadPlayerScore();
            }
        }

        private void SetUpSubmitScore()
        {

        }

        private void SetUpDisplayScore()
        {
            if(CurrentScore is null)
            {
                LoadPlayerScore();
            }
        }

        private void Instance_ScoreLoaded(string arg1, Score_GPGS arg2)
        {
            if(arg1 == CurrentLeaderBoard.leaderboardId )
            {
                CurrentScore = arg2;
                RefreshScoreData();
            }
        }

        private void RefreshScoreData()
        {
            if(CurrentScore != null)
            {
                PlayerRankLabel.Text = CurrentScore.displayRank;
                PlayerScoreLabel.Text = CurrentScore.displayScore;
            }
        }

        private void LoadPlayerScore()
        {
            LeaderboardsClient.Instance.LoadPlayerScore(CurrentLeaderBoard.leaderboardId,GPGS.TimeSpan_GPGS.TIME_SPAN_ALL_TIME,Collection_GPGS.COLLECTION_PUBLIC);
        }

        private void Instance_ImageStored(string obj)
        {
            if (obj == CurrentLeaderBoard.iconImageUri)
            {
                GodotPlayGameService.Instance.DisplayImageInTextureRect(IconRect, obj);
            }
        }
    }
}
