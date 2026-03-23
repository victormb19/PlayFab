using System;
using GameBackEnd.Auth;
using PlayFab.ClientModels;

namespace PlayFab.Auth
{
    public class PlayFabAuthService : IAuthService
    {
        private readonly IPlayFabClientApi playFabClientApi;

        public bool IsLoggedIn => playFabClientApi.IsClientLoggedIn();

        public PlayFabAuthService(IPlayFabClientApi playFabClientApi)
        {
            this.playFabClientApi = playFabClientApi;
        }

        public void LoginWithDeviceId(string deviceId, Action<AuthResult> onComplete)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                onComplete?.Invoke(AuthResult.Failed("Device ID cannot be null or empty."));
                return;
            }

            var request = new LoginWithCustomIDRequest
            {
                CustomId = deviceId,
                CreateAccount = true
            };

            playFabClientApi.LoginWithCustomID(request,
                result => onComplete?.Invoke(
                    AuthResult.Succeeded(result.PlayFabId, result.SessionTicket, result.NewlyCreated)),
                error => onComplete?.Invoke(
                    AuthResult.Failed(error.ErrorMessage))
            );
        }

        public void LoginWithCredentials(string username, string password, Action<AuthResult> onComplete)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                onComplete?.Invoke(AuthResult.Failed("Username and password cannot be null or empty."));
                return;
            }

            var request = new LoginWithPlayFabRequest
            {
                Username = username,
                Password = password
            };

            playFabClientApi.LoginWithPlayFab(request,
                result => onComplete?.Invoke(
                    AuthResult.Succeeded(result.PlayFabId, result.SessionTicket, result.NewlyCreated)),
                error => onComplete?.Invoke(
                    AuthResult.Failed(error.ErrorMessage))
            );
        }

        public void Logout()
        {
            playFabClientApi.ForgetAllCredentials();
        }
    }
}
