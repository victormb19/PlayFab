using System;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFab.Auth
{
    public class PlayFabClientApiAdapter : IPlayFabClientApi
    {
        public void LoginWithCustomID(LoginWithCustomIDRequest request, Action<LoginResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.LoginWithCustomID(request, onSuccess, onError);
        }

        public void LoginWithPlayFab(LoginWithPlayFabRequest request, Action<LoginResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.LoginWithPlayFab(request, onSuccess, onError);
        }

        public bool IsClientLoggedIn()
        {
            return PlayFabClientAPI.IsClientLoggedIn();
        }

        public void ForgetAllCredentials()
        {
            PlayFabClientAPI.ForgetAllCredentials();
        }
    }
}
