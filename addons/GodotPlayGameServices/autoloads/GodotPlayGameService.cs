using Godot;
namespace GPGS
{
    public partial class GodotPlayGameService : Node
    {
        public delegate void ImageStoredDelegate(string filePath);
        GodotPlayGameService() { }
        public static GodotPlayGameService Instance { get; private set; }
        /// <summary>
        /// Represents the Android plugin for the GodotPlayGameService.
        /// </summary>
        public GodotObject Plugin { get; private set; }

        const string plugin_name = "GodotPlayGameServices";
        /// <summary>
        /// Event that raised when an image is stored.
        /// </summary>
        public event ImageStoredDelegate ImageStored;
        public override void _Ready()
        {
            Instance = this;
            if (Plugin == null)
            {
                if (Engine.HasSingleton(plugin_name))
                {
                    Plugin = Engine.GetSingleton(plugin_name);
                    Plugin.Call("initialize");
                }
                else
                {
                    GD.PrintErr("No plugin found.");
                }
            }
            Plugin?.Connect("imageStored", new Callable(this, nameof(OnImageStoredSignalConnected)));
        }
        /// <summary>
        /// Invokes the ImageStored event with the provided file path.
        /// </summary>
        /// <param name="filePath">The file path of the stored image.</param>
        private void OnImageStoredSignalConnected(string filePath)
        {
            ImageStored?.Invoke(filePath);
        }

        /// <summary>
        /// Displays the image in the specified TextureRect using the provided file path.
        /// </summary>
        /// <param name="textureRect">The TextureRect in which to display the image.</param>
        /// <param name="filePath">The file path of the image to display.</param>
        public void DisplayImageInTextureRect(TextureRect textureRect, string filePath)
        {
            if (FileAccess.FileExists(filePath))
            {
                var image = Image.LoadFromFile(filePath);

                textureRect.Texture = ImageTexture.CreateFromImage(image);
            }
            else
            {
                GD.Print("file doesn't exist");
            }
        }

    }
}
