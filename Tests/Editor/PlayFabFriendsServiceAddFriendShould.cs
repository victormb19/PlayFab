using System;
using GameBackEnd.Friends;
using NSubstitute;
using NUnit.Framework;
using PlayFab.ClientModels;
using PlayFab.Friends;

namespace PlayFab.Tests
{
    [TestFixture]
    public class PlayFabFriendsServiceAddFriendShould
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
        public void Call_Add_Friend_With_Display_Name()
        {
            friendsService.AddFriendByDisplayName("Alice");

            api.Received(1).AddFriend(
                Arg.Is<AddFriendRequest>(r => r.FriendTitleDisplayName == "Alice"),
                Arg.Any<Action<AddFriendResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }

        [Test]
        public void Notify_Success_When_Add_Succeeds()
        {
            SetupAddSuccess();

            friendsService.AddFriendByDisplayName("Alice");

            observer.Received(1).OnFriendAdded(Arg.Is<FriendOperationResult>(r => r.Success));
        }

        [Test]
        public void Notify_Failure_When_Add_Fails()
        {
            api.When(x => x.AddFriend(
                    Arg.Any<AddFriendRequest>(),
                    Arg.Any<Action<AddFriendResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError { ErrorMessage = "Not found" }));

            friendsService.AddFriendByDisplayName("Alice");

            observer.Received(1).OnFriendAdded(Arg.Is<FriendOperationResult>(r =>
                !r.Success && r.ErrorMessage == "Not found"));
        }

        [Test]
        public void Notify_Failure_Without_Calling_Api_When_Name_Is_Null()
        {
            friendsService.AddFriendByDisplayName(null);

            observer.Received(1).OnFriendAdded(Arg.Is<FriendOperationResult>(r => !r.Success));
            api.DidNotReceive().AddFriend(
                Arg.Any<AddFriendRequest>(),
                Arg.Any<Action<AddFriendResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }

        [Test]
        public void Notify_Failure_Without_Calling_Api_When_Name_Is_Empty()
        {
            friendsService.AddFriendByDisplayName("");

            observer.Received(1).OnFriendAdded(Arg.Is<FriendOperationResult>(r => !r.Success));
            api.DidNotReceive().AddFriend(
                Arg.Any<AddFriendRequest>(),
                Arg.Any<Action<AddFriendResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }

        private void SetupAddSuccess()
        {
            api.When(x => x.AddFriend(
                    Arg.Any<AddFriendRequest>(),
                    Arg.Any<Action<AddFriendResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<AddFriendResult>>().Invoke(new AddFriendResult()));
        }
    }
}
