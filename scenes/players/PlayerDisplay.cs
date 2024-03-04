using Godot;
using GPGS;
using System.Threading.Tasks;

namespace AndroidTest.scenes
{
    public partial class PlayerDisplay:Control
    {
        [Export] TextureRect AvatarRect;
        [Export] Label IdLabel;
        [Export] Label NameLabel;
        [Export] Label TitleLabel;
        [Export] Label StatusLabel;
        [Export] Label LevelLabel;
        [Export] Label XpLevel;
        [Export] VBoxContainer CompareHolder;
        [Export] Button CompareButton;
        Player_GPGS CurrentPlayer;
        public bool IsComparable = false;

        public override void _Ready()
        {
            GodotPlayGameService.Instance.ImageStored += Instance_ImageStored;
            if(CurrentPlayer != null )
            {
                SetUpDisplay();
                CompareButton.Pressed += CompareButton_Pressed;
            }
        }

        private void CompareButton_Pressed()
        {
            PlayersClient.Instance.CompareProfile(CurrentPlayer.playerId);
        }

        public void SetPlayer(Player_GPGS player)
        {
            CurrentPlayer = player;
        }
        private void Instance_ImageStored(string obj)
        {
            if(obj == CurrentPlayer.hiResImageUri && AvatarRect.Texture == null)
            {
                DisplayAvatar();
            }
               
        }
        public void SetUpDisplay()
        {
            DisplayAvatar();
            IdLabel.Text = CurrentPlayer.playerId;
            NameLabel.Text = CurrentPlayer.displayName;
            TitleLabel.Text = CurrentPlayer.title;
            StatusLabel.Text = CurrentPlayer.friendStatus.ToString();
            LevelLabel.Text = CurrentPlayer.levelInfo.currentLevel.levelNumber.ToString();
            XpLevel.Text = CurrentPlayer.levelInfo.currentXpTotal.ToString();
            CompareHolder.Visible = IsComparable;
        }
        public async Task<Image> LoadAndRetry(string imageUri)
        {
            var image = new Image();
            var retires = 3;
            Error error = Error.FileNotFound;
            while (retires > 0 || error == Error.FileNotFound)
            {
                if(retires !=3)
                {
                    await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
                }
                error = image.LoadPngFromBuffer(Image.LoadFromFile(imageUri).GetData());
                retires -= 1;
                if(retires == 0)
                {
                    GD.Print("Error Loading file!");
                    image = null;
                }
            }
            return image;
        }
        public void DisplayAvatar()
        {
            GodotPlayGameService.Instance.DisplayImageInTextureRect(AvatarRect,CurrentPlayer.hiResImageUri);
        }

    }
}
