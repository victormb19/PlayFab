using System;
using GameBackEnd.Auth;
using PlayFab.ClientModels;
using UnityEngine;

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

            Debug.Log($"[PlayFabAuthService] Calling LoginWithCustomID. CustomId={request.CustomId}");
            playFabClientApi.LoginWithCustomID(request,
                result =>
                {
                    Debug.Log($"[PlayFabAuthService] Login SUCCESS. PlayFabId={result.PlayFabId}");
                    onComplete?.Invoke(
                        AuthResult.Succeeded(result.PlayFabId, result.SessionTicket, result.NewlyCreated));
                },
                error =>
                {
                    Debug.LogError($"[PlayFabAuthService] Login FAILED. Error={error.ErrorMessage}");
                    onComplete?.Invoke(
                        AuthResult.Failed(error.ErrorMessage));
                }
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
