using System;
using GameBackEnd.Friends;
using NSubstitute;
using NUnit.Framework;
using PlayFab.ClientModels;
using PlayFab.Friends;

namespace PlayFab.Tests
{
    [TestFixture]
    public class PlayFabFriendsServiceRemoveFriendShould
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
        public void Call_Remove_Friend_With_Friend_Id()
        {
            friendsService.RemoveFriend("friend-001");

            api.Received(1).RemoveFriend(
                Arg.Is<RemoveFriendRequest>(r => r.FriendPlayFabId == "friend-001"),
                Arg.Any<Action<RemoveFriendResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }

        [Test]
        public void Notify_Success_When_Remove_Succeeds()
        {
            api.When(x => x.RemoveFriend(
                    Arg.Any<RemoveFriendRequest>(),
                    Arg.Any<Action<RemoveFriendResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<RemoveFriendResult>>().Invoke(new RemoveFriendResult()));

            friendsService.RemoveFriend("friend-001");

            observer.Received(1).OnFriendRemoved(Arg.Is<FriendOperationResult>(r => r.Success));
        }

        [Test]
        public void Notify_Failure_When_Remove_Fails()
        {
            api.When(x => x.RemoveFriend(
                    Arg.Any<RemoveFriendRequest>(),
                    Arg.Any<Action<RemoveFriendResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError { ErrorMessage = "Unknown id" }));

            friendsService.RemoveFriend("friend-001");

            observer.Received(1).OnFriendRemoved(Arg.Is<FriendOperationResult>(r =>
                !r.Success && r.ErrorMessage == "Unknown id"));
        }

        [Test]
        public void Notify_Failure_Without_Calling_Api_When_Id_Is_Empty()
        {
            friendsService.RemoveFriend("");

            observer.Received(1).OnFriendRemoved(Arg.Is<FriendOperationResult>(r => !r.Success));
            api.DidNotReceive().RemoveFriend(
                Arg.Any<RemoveFriendRequest>(),
                Arg.Any<Action<RemoveFriendResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }

        [Test]
        public void Not_Notify_Unregistered_Observer()
        {
            friendsService.UnregisterObserver(observer);

            friendsService.RemoveFriend("");

            observer.DidNotReceive().OnFriendRemoved(Arg.Any<FriendOperationResult>());
        }
    }
}
