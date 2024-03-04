using Godot;

namespace GPGS
{
    public partial class SignInClient : Node
    {
        public delegate void AuthenticationDelegate(bool isAuthenticated);
        /// <summary>
        /// Class for signing in the client
        /// </summary>
        public static SignInClient Instance { get; private set; }

        /// <summary>
        /// Event for when the user is authenticated
        /// </summary>
        public event AuthenticationDelegate UserAuthenticated;

        /// <summary>
        /// Event for when server side access is requested
        /// </summary>
        public event AuthenticationDelegate ServerSideAccessRequested;
  
        public override void _Ready()
        {
            Instance = this;
            // Connects signals from the AndroidPlugin instance to corresponding methods
            GodotPlayGameService.Instance?.Plugin?.Connect("userAuthenticated", new Callable(this, nameof(OnUserAuthenticatedSignalConnected)));
            GodotPlayGameService.Instance?.Plugin?.Connect("serverSideAccessRequested", new Callable(this, nameof(ServerSideAccessRequestedSignalConnected)));
        }
        /// <summary>
        /// Invokes the UserAuthenticated event with the specified authentication status.
        /// </summary>
        /// <param name="isAuthenticated">The authentication status.</param>
        private void OnUserAuthenticatedSignalConnected(bool isAuthenticated)
        {
            UserAuthenticated?.Invoke(isAuthenticated);
        }

        /// <summary>
        /// Invokes the ServerSideAccessRequested event with the specified authentication status.
        /// </summary>
        /// <param name="isAuthenticated">The authentication status.</param>
        private void ServerSideAccessRequestedSignalConnected(bool isAuthenticated)
        {
            ServerSideAccessRequested?.Invoke(isAuthenticated);
        }
        /// <summary>
        /// Check if the user is authenticated.
        /// </summary>
        public void IsAuthenticated()
        {
            GodotPlayGameService.Instance?.Plugin?.Call("isAuthenticated");
        }

        /// <summary>
        /// Sign in the user.
        /// </summary>
        public void SignIn()
        {
            GodotPlayGameService.Instance?.Plugin?.Call("signIn");
        }

        /// <summary>
        /// Request server side access with the specified server client ID and force refresh token flag.
        /// </summary>
        /// <param name="serverClientId">The server client ID to request access for.</param>
        /// <param name="forceRefreshToken">Whether to force refresh the token.</param>
        public void RequestServerSideAccess(string serverClientId, bool forceRefreshToken)
        {
            GodotPlayGameService.Instance?.Plugin?.Call("requestServerSideAccess", serverClientId, forceRefreshToken);
        }

    }
}
