using System;
using PlayFab.ClientModels;

namespace PlayFab.Profile
{
    public interface IPlayFabProfileApi
    {
        void GetAccountInfo(GetAccountInfoRequest request, Action<GetAccountInfoResult> onSuccess, Action<PlayFabError> onError);
        void GetUserData(GetUserDataRequest request, Action<GetUserDataResult> onSuccess, Action<PlayFabError> onError);
        void UpdateUserTitleDisplayName(UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> onSuccess, Action<PlayFabError> onError);
        void UpdateUserData(UpdateUserDataRequest request, Action<UpdateUserDataResult> onSuccess, Action<PlayFabError> onError);
    }
}
