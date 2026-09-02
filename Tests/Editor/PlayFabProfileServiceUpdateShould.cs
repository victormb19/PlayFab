using System;
using GameBackEnd.Profile;
using NSubstitute;
using NUnit.Framework;
using PlayFab.ClientModels;
using PlayFab.Profile;

namespace PlayFab.Tests
{
    [TestFixture]
    public class PlayFabProfileServiceUpdateShould
    {
        private IPlayFabProfileApi api;
        private IProfileServiceObserver observer;
        private PlayFabProfileService profileService;

        [SetUp]
        public void SetUp()
        {
            api = Substitute.For<IPlayFabProfileApi>();
            observer = Substitute.For<IProfileServiceObserver>();
            profileService = new PlayFabProfileService(api);
            profileService.RegisterObserver(observer);
        }

        [Test]
        public void Send_The_Display_Name_To_The_Api()
        {
            profileService.UpdateDisplayName("Vic");

            api.Received(1).UpdateUserTitleDisplayName(
                Arg.Is<UpdateUserTitleDisplayNameRequest>(request => request.DisplayName == "Vic"),
                Arg.Any<Action<UpdateUserTitleDisplayNameResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }

        [Test]
        public void Reject_An_Empty_Display_Name_Without_Calling_The_Api()
        {
            profileService.UpdateDisplayName("");

            api.DidNotReceive().UpdateUserTitleDisplayName(
                Arg.Any<UpdateUserTitleDisplayNameRequest>(),
                Arg.Any<Action<UpdateUserTitleDisplayNameResult>>(),
                Arg.Any<Action<PlayFabError>>());
            observer.Received(1).OnDisplayNameUpdated(Arg.Is<ProfileOperationResult>(result => !result.Success));
        }

        [Test]
        public void Store_The_Avatar_As_User_Data()
        {
            profileService.UpdateAvatar("avatar_07");

            api.Received(1).UpdateUserData(
                Arg.Is<UpdateUserDataRequest>(request => request.Data["avatarId"] == "avatar_07"),
                Arg.Any<Action<UpdateUserDataResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }
    }
}
