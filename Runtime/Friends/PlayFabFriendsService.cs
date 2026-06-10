using System.Collections.Generic;
using GameBackEnd.Friends;
using PlayFab.ClientModels;

namespace PlayFab.Friends
{
    public class PlayFabFriendsService : FriendsServiceObservable, IFriendsService
    {
        private readonly IPlayFabFriendsApi friendsApi;

        public PlayFabFriendsService(IPlayFabFriendsApi friendsApi)
        {
            this.friendsApi = friendsApi;
        }

        public void LoadFriends()
        {
            friendsApi.GetFriendsList(new GetFriendsListRequest(),
                result => NotifyFriendsLoaded(FriendsResult.Succeeded(Map(result.Friends))),
                error => NotifyFriendsLoaded(FriendsResult.Failed(error.ErrorMessage)));
        }

        public void AddFriendByDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                NotifyFriendAdded(FriendOperationResult.Failed("Display name cannot be null or empty."));
                return;
            }

            SendAddFriendRequest(displayName);
        }

        public void RemoveFriend(string friendId)
        {
            if (string.IsNullOrEmpty(friendId))
            {
                NotifyFriendRemoved(FriendOperationResult.Failed("Friend ID cannot be null or empty."));
                return;
            }

            SendRemoveFriendRequest(friendId);
        }

        private void SendAddFriendRequest(string displayName)
        {
            var request = new AddFriendRequest { FriendTitleDisplayName = displayName };

            friendsApi.AddFriend(request,
                result => NotifyFriendAdded(FriendOperationResult.Succeeded()),
                error => NotifyFriendAdded(FriendOperationResult.Failed(error.ErrorMessage)));
        }

        private void SendRemoveFriendRequest(string friendId)
        {
            var request = new RemoveFriendRequest { FriendPlayFabId = friendId };

            friendsApi.RemoveFriend(request,
                result => NotifyFriendRemoved(FriendOperationResult.Succeeded()),
                error => NotifyFriendRemoved(FriendOperationResult.Failed(error.ErrorMessage)));
        }

        private static IReadOnlyList<FriendData> Map(List<FriendInfo> friends)
        {
            var mapped = new List<FriendData>();

            if (friends == null)
            {
                return mapped;
            }

            foreach (var friend in friends)
            {
                mapped.Add(new FriendData(friend.FriendPlayFabId, friend.TitleDisplayName ?? friend.Username));
            }

            return mapped;
        }
    }
}
