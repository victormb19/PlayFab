using System;
using PlayFab.ClientModels;

namespace PlayFab.Profile
{
    public class PlayFabProfileApiAdapter : IPlayFabProfileApi
    {
        public void GetAccountInfo(GetAccountInfoRequest request, Action<GetAccountInfoResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.GetAccountInfo(request, onSuccess, onError);
        }

        public void GetUserData(GetUserDataRequest request, Action<GetUserDataResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.GetUserData(request, onSuccess, onError);
        }

        public void UpdateUserTitleDisplayName(UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, onSuccess, onError);
        }

        public void UpdateUserData(UpdateUserDataRequest request, Action<UpdateUserDataResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.UpdateUserData(request, onSuccess, onError);
        }
    }
}
