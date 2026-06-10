using System;
using PlayFab.ClientModels;

namespace PlayFab.Friends
{
    public class PlayFabFriendsApiAdapter : IPlayFabFriendsApi
    {
        public void AddFriend(AddFriendRequest request, Action<AddFriendResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.AddFriend(request, onSuccess, onError);
        }

        public void GetFriendsList(GetFriendsListRequest request, Action<GetFriendsListResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.GetFriendsList(request, onSuccess, onError);
        }

        public void RemoveFriend(RemoveFriendRequest request, Action<RemoveFriendResult> onSuccess, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.RemoveFriend(request, onSuccess, onError);
        }
    }
}
