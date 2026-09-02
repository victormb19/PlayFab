using System;
using System.Collections.Generic;
using GameBackEnd.Profile;
using NSubstitute;
using NUnit.Framework;
using PlayFab.ClientModels;
using PlayFab.Profile;

namespace PlayFab.Tests
{
    [TestFixture]
    public class PlayFabProfileServiceLoadProfileShould
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
        public void Ask_The_Api_For_The_Account_Info()
        {
            profileService.LoadProfile();

            api.Received(1).GetAccountInfo(
                Arg.Any<GetAccountInfoRequest>(),
                Arg.Any<Action<GetAccountInfoResult>>(),
                Arg.Any<Action<PlayFabError>>());
        }
        [Test]
        public void Notify_The_Profile_With_The_Display_Name_And_The_Avatar()
        {
            SetupAccountInfo("player-001", "Vic");
            SetupUserData("avatar_07");

            profileService.LoadProfile();

            observer.Received(1).OnProfileLoaded(Arg.Is<ProfileResult>(result =>
                result.Success
                && result.Profile.PlayerId == "player-001"
                && result.Profile.DisplayName == "Vic"
                && result.Profile.AvatarId == "avatar_07"));
        }

        [Test]
        public void Leave_The_Avatar_Empty_When_The_Player_Never_Chose_One()
        {
            SetupAccountInfo("player-001", "Vic");
            SetupEmptyUserData();

            profileService.LoadProfile();

            observer.Received(1).OnProfileLoaded(Arg.Is<ProfileResult>(result =>
                result.Success && result.Profile.AvatarId == string.Empty));
        }

        [Test]
        public void Notify_Failure_When_The_Account_Info_Fails()
        {
            api.When(x => x.GetAccountInfo(
                    Arg.Any<GetAccountInfoRequest>(),
                    Arg.Any<Action<GetAccountInfoResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError { ErrorMessage = "Down" }));

            profileService.LoadProfile();

            observer.Received(1).OnProfileLoaded(Arg.Is<ProfileResult>(result =>
                !result.Success && result.ErrorMessage == "Down"));
        }

        [Test]
        public void Notify_Failure_When_The_User_Data_Fails()
        {
            SetupAccountInfo("player-001", "Vic");
            api.When(x => x.GetUserData(
                    Arg.Any<GetUserDataRequest>(),
                    Arg.Any<Action<GetUserDataResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError { ErrorMessage = "Sin datos" }));

            profileService.LoadProfile();

            observer.Received(1).OnProfileLoaded(Arg.Is<ProfileResult>(result =>
                !result.Success && result.ErrorMessage == "Sin datos"));
        }

        private void SetupEmptyUserData()
        {
            api.When(x => x.GetUserData(
                    Arg.Any<GetUserDataRequest>(),
                    Arg.Any<Action<GetUserDataResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<GetUserDataResult>>().Invoke(new GetUserDataResult()));
        }

        [Test]
        public void Hand_The_Loaded_Profile_To_An_Observer_That_Arrives_Late()
        {
            SetupAccountInfo("player-001", "Vic");
            SetupUserData("avatar_07");
            profileService.LoadProfile();
            var lateObserver = Substitute.For<IProfileServiceObserver>();

            profileService.RegisterObserver(lateObserver);

            lateObserver.Received(1).OnProfileLoaded(Arg.Is<ProfileResult>(result =>
                result.Success && result.Profile.DisplayName == "Vic"));
        }

        private void SetupAccountInfo(string playFabId, string displayName)
        {
            api.When(x => x.GetAccountInfo(
                    Arg.Any<GetAccountInfoRequest>(),
                    Arg.Any<Action<GetAccountInfoResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<GetAccountInfoResult>>().Invoke(new GetAccountInfoResult
                {
                    AccountInfo = new UserAccountInfo
                    {
                        PlayFabId = playFabId,
                        TitleInfo = new UserTitleInfo { DisplayName = displayName }
                    }
                }));
        }

        private void SetupUserData(string avatarId)
        {
            api.When(x => x.GetUserData(
                    Arg.Any<GetUserDataRequest>(),
                    Arg.Any<Action<GetUserDataResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<GetUserDataResult>>().Invoke(new GetUserDataResult
                {
                    Data = new Dictionary<string, UserDataRecord>
                    {
                        { "avatarId", new UserDataRecord { Value = avatarId } }
                    }
                }));
        }
    }
}
