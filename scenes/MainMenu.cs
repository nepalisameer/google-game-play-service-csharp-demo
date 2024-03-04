using Godot;

namespace GPGS.GooglePlayGameService
{
    public partial class MainMenu:Control
    {
        [Export] Label TitleLabel;
        [Export] Button Achievements;
        [Export] Button Leaderboards;
        [Export] Button Players;
        [Export] Button Snapshots;
        int SignInRetries = 5;
        public override void _Ready()
        {
            if (GodotPlayGameService.Instance.Plugin == null)
            {
                TitleLabel.Text = "Plugin not found";

            }
            SignInClient.Instance.UserAuthenticated += Instance_UserAuthenticated;
            Achievements.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/achievements/Achievements.tscn");
            Leaderboards.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/leaderboards/Leaderboards.tscn");
            Players.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/players/Players.tscn");
            Snapshots.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/snapshots/Snapshots.tscn");
        }

        private void Instance_UserAuthenticated(bool obj)
        {
            if(SignInRetries >0 && !obj)
            {
                TitleLabel.Text = "trying to signIn";
                SignInClient.Instance.SignIn();
                SignInRetries--;
            }
            if (SignInRetries == 0)
            {
                TitleLabel.Text = "Sign in attemps expired!";

            }


        if (obj)
            {
                TitleLabel.Text = "Main Menu";

            }
        }
    }
}
