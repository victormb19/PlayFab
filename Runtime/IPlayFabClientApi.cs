using System;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFab.Auth
{
    public interface IPlayFabClientApi
    {
        void LoginWithCustomID(LoginWithCustomIDRequest request, Action<LoginResult> onSuccess, Action<PlayFabError> onError);
        void LoginWithPlayFab(LoginWithPlayFabRequest request, Action<LoginResult> onSuccess, Action<PlayFabError> onError);
        bool IsClientLoggedIn();
        void ForgetAllCredentials();
    }
}
