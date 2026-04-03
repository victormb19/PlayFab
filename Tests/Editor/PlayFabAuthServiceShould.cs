using System;
using GameBackEnd.Auth;
using NSubstitute;
using NUnit.Framework;
using PlayFab.Auth;
using PlayFab.ClientModels;

namespace PlayFab.Tests
{
    [TestFixture]
    public class PlayFabAuthServiceShould
    {
        private IPlayFabClientApi api;
        private PlayFabAuthService authService;

        [SetUp]
        public void SetUp()
        {
            api = Substitute.For<IPlayFabClientApi>();
            authService = new PlayFabAuthService(api);
        }

        [Test]
        public void Call_PlayFab_With_Correct_Custom_Id()
        {
            authService.LoginWithDeviceId("device-123", _ => { });

            api.Received(1).LoginWithCustomID(
                Arg.Is<LoginWithCustomIDRequest>(r =>
                    r.CustomId == "device-123" && r.CreateAccount == true),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void Return_Success_When_Login_With_Device_Id_Succeeds()
        {
            SetupLoginWithCustomIdSuccess("player-001", "ticket-abc", false);

            var received = false;
            var result = default(AuthResult);
            authService.LoginWithDeviceId("device-123", r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("player-001", result.PlayerId);
            Assert.AreEqual("ticket-abc", result.SessionToken);
            Assert.IsFalse(result.IsNewAccount);
        }

        [Test]
        public void Return_Is_New_Account_True_When_Device_Creates_Account()
        {
            SetupLoginWithCustomIdSuccess("player-002", "ticket-def", true);

            var received = false;
            var result = default(AuthResult);
            authService.LoginWithDeviceId("device-123", r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsTrue(result.IsNewAccount);
        }

        [Test]
        public void Return_Failed_When_Login_With_Device_Id_Fails()
        {
            SetupLoginWithCustomIdFailure("Service unavailable");

            var received = false;
            var result = default(AuthResult);
            authService.LoginWithDeviceId("device-123", r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Service unavailable", result.ErrorMessage);
            Assert.IsNull(result.PlayerId);
        }

        [Test]
        public void Return_Failed_When_Device_Id_Is_Null()
        {
            var received = false;
            var result = default(AuthResult);
            authService.LoginWithDeviceId(null, r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsFalse(result.Success);
            api.DidNotReceive().LoginWithCustomID(
                Arg.Any<LoginWithCustomIDRequest>(),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void Return_Failed_When_Device_Id_Is_Empty()
        {
            var received = false;
            var result = default(AuthResult);
            authService.LoginWithDeviceId("", r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsFalse(result.Success);
            api.DidNotReceive().LoginWithCustomID(
                Arg.Any<LoginWithCustomIDRequest>(),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void Call_PlayFab_With_Correct_Credentials()
        {
            authService.LoginWithCredentials("myuser", "mypass123", _ => { });

            api.Received(1).LoginWithPlayFab(
                Arg.Is<LoginWithPlayFabRequest>(r =>
                    r.Username == "myuser" && r.Password == "mypass123"),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void Return_Success_When_Login_With_Credentials_Succeeds()
        {
            SetupLoginWithPlayFabSuccess("player-010", "ticket-xyz");

            var received = false;
            var result = default(AuthResult);
            authService.LoginWithCredentials("myuser", "mypass123", r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("player-010", result.PlayerId);
            Assert.AreEqual("ticket-xyz", result.SessionToken);
        }

        [Test]
        public void Return_Failed_When_Login_With_Credentials_Fails()
        {
            SetupLoginWithPlayFabFailure("Invalid credentials");

            var received = false;
            var result = default(AuthResult);
            authService.LoginWithCredentials("myuser", "wrongpass", r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid credentials", result.ErrorMessage);
        }

        [Test]
        public void Return_Failed_When_Username_Is_Null()
        {
            var received = false;
            var result = default(AuthResult);
            authService.LoginWithCredentials(null, "mypass123", r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsFalse(result.Success);
            api.DidNotReceive().LoginWithPlayFab(
                Arg.Any<LoginWithPlayFabRequest>(),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void Return_Failed_When_Password_Is_Empty()
        {
            var received = false;
            var result = default(AuthResult);
            authService.LoginWithCredentials("myuser", "", r => { result = r; received = true; });

            Assert.IsTrue(received);
            Assert.IsFalse(result.Success);
            api.DidNotReceive().LoginWithPlayFab(
                Arg.Any<LoginWithPlayFabRequest>(),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void Return_False_When_Not_Logged_In()
        {
            api.IsClientLoggedIn().Returns(false);

            Assert.IsFalse(authService.IsLoggedIn);
        }

        [Test]
        public void Return_True_When_Logged_In()
        {
            api.IsClientLoggedIn().Returns(true);

            Assert.IsTrue(authService.IsLoggedIn);
        }

        [Test]
        public void Call_Forget_All_Credentials_On_Logout()
        {
            authService.Logout();

            api.Received(1).ForgetAllCredentials();
        }

        private void SetupLoginWithCustomIdSuccess(string playFabId, string ticket, bool newlyCreated)
        {
            api.When(x => x.LoginWithCustomID(
                    Arg.Any<LoginWithCustomIDRequest>(),
                    Arg.Any<Action<LoginResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<LoginResult>>().Invoke(new LoginResult
                {
                    PlayFabId = playFabId,
                    SessionTicket = ticket,
                    NewlyCreated = newlyCreated
                }));
        }

        private void SetupLoginWithCustomIdFailure(string errorMessage)
        {
            api.When(x => x.LoginWithCustomID(
                    Arg.Any<LoginWithCustomIDRequest>(),
                    Arg.Any<Action<LoginResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError
                {
                    ErrorMessage = errorMessage
                }));
        }

        private void SetupLoginWithPlayFabSuccess(string playFabId, string ticket)
        {
            api.When(x => x.LoginWithPlayFab(
                    Arg.Any<LoginWithPlayFabRequest>(),
                    Arg.Any<Action<LoginResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<LoginResult>>().Invoke(new LoginResult
                {
                    PlayFabId = playFabId,
                    SessionTicket = ticket,
                    NewlyCreated = false
                }));
        }

        private void SetupLoginWithPlayFabFailure(string errorMessage)
        {
            api.When(x => x.LoginWithPlayFab(
                    Arg.Any<LoginWithPlayFabRequest>(),
                    Arg.Any<Action<LoginResult>>(),
                    Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError
                {
                    ErrorMessage = errorMessage
                }));
        }
    }
}
