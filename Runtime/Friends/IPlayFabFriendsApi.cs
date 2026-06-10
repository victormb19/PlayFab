using System;
using PlayFab.ClientModels;

namespace PlayFab.Friends
{
    public interface IPlayFabFriendsApi
    {
        void AddFriend(AddFriendRequest request, Action<AddFriendResult> onSuccess, Action<PlayFabError> onError);
        void GetFriendsList(GetFriendsListRequest request, Action<GetFriendsListResult> onSuccess, Action<PlayFabError> onError);
        void RemoveFriend(RemoveFriendRequest request, Action<RemoveFriendResult> onSuccess, Action<PlayFabError> onError);
    }
}
