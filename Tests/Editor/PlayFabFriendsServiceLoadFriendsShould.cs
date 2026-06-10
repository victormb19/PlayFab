using System;
using GameBackEnd.Friends;
using NSubstitute;
using NUnit.Framework;
using PlayFab.ClientModels;
using PlayFab.Friends;

namespace PlayFab.Tests
{
    [TestFixture]
    public class PlayFabFriendsServiceLoadFriendsShould
    {
        private IPlayFabFriendsApi api;
        private IFriendsServiceObserver observer;
        private PlayFabFriendsService friendsService;

        [SetUp]
        public void SetUp()
        {
            api = Substitute.For<IPlayFabFriendsApi>();
            observer = Substitute.For<IFriendsServiceObserver>();
            friendsService = new PlayFabFriendsService(api);
            friendsService.RegisterObserver(observer);
        }

        [Test]
        public void Call_Get_Friends_List_On_Load()
        {
            friendsService.LoadFriends();

            api.Received(1).GetFriendsList(
                Arg.Any<GetFriendsListRequest>(),
                Arg.Any<Action<GetFriendsListResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }

        [Test]
        public void Notify_Loaded_Friends_With_Display_Name()
        {
            SetupSuccess(new FriendInfo { FriendPlayFabId = "friend-001", TitleDisplayName = "Alice" });

            friendsService.LoadFriends();

            observer.Received(1).OnFriendsLoaded(Arg.Is<FriendsResult>(r =>
                r.Success && r.Friends.Count == 1
                && r.Friends[0].Id == "friend-001" && r.Friends[0].DisplayName == "Alice"));
        }

        [Test]
        public void Fall_Back_To_Username_When_Display_Name_Is_Null()
        {
            SetupSuccess(new FriendInfo { FriendPlayFabId = "friend-002", Username = "bob" });

            friendsService.LoadFriends();

            observer.Received(1).OnFriendsLoaded(Arg.Is<FriendsResult>(r =>
                r.Friends[0].DisplayName == "bob"));
        }

        [Test]
        public void Notify_Empty_List_When_Friends_Are_Null()
        {
            SetupSuccess(null);

            friendsService.LoadFriends();

            observer.Received(1).OnFriendsLoaded(Arg.Is<FriendsResult>(r =>
                r.Success && r.Friends.Count == 0));
        }

        [Test]
        public void Notify_Failure_When_Load_Fails()
        {
            api.When(x => x.GetFriendsList(
                    Arg.Any<GetFriendsListRequest>(),
                    Arg.Any<Action<GetFriendsListResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError { ErrorMessage = "Down" }));

            friendsService.LoadFriends();

            observer.Received(1).OnFriendsLoaded(Arg.Is<FriendsResult>(r =>
                !r.Success && r.ErrorMessage == "Down"));
        }

        private void SetupSuccess(FriendInfo friend)
        {
            var result = new GetFriendsListResult();
            if (friend != null)
            {
                result.Friends = new System.Collections.Generic.List<FriendInfo> { friend };
            }

            api.When(x => x.GetFriendsList(
                    Arg.Any<GetFriendsListRequest>(),
                    Arg.Any<Action<GetFriendsListResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<GetFriendsListResult>>().Invoke(result));
        }
    }
}
