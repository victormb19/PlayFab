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
        private IPlayFabClientApi _api;
        private PlayFabAuthService _authService;

        [SetUp]
        public void SetUp()
        {
            _api = Substitute.For<IPlayFabClientApi>();
            _authService = new PlayFabAuthService(_api);
        }

        [Test]
        public void CallPlayFabWithCorrectCustomId_WhenLoginWithDeviceId()
        {
            _authService.LoginWithDeviceId("device-123", _ => { });

            _api.Received(1).LoginWithCustomID(
                Arg.Is<LoginWithCustomIDRequest>(r => r.CustomId == "device-123" && r.CreateAccount == true),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void ReturnSuccessResult_WhenLoginWithDeviceIdSucceeds()
        {
            _api.When(x => x.LoginWithCustomID(Arg.Any<LoginWithCustomIDRequest>(), Arg.Any<Action<LoginResult>>(), Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<LoginResult>>().Invoke(new LoginResult
                {
                    PlayFabId = "player-001",
                    SessionTicket = "ticket-abc",
                    NewlyCreated = false
                }));

            AuthResult result = null;
            _authService.LoginWithDeviceId("device-123", r => result = r);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("player-001", result.PlayerId);
            Assert.AreEqual("ticket-abc", result.SessionToken);
            Assert.IsFalse(result.IsNewAccount);
        }

        [Test]
        public void ReturnIsNewAccountTrue_WhenDeviceIdCreatesNewAccount()
        {
            _api.When(x => x.LoginWithCustomID(Arg.Any<LoginWithCustomIDRequest>(), Arg.Any<Action<LoginResult>>(), Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<LoginResult>>().Invoke(new LoginResult
                {
                    PlayFabId = "player-002",
                    SessionTicket = "ticket-def",
                    NewlyCreated = true
                }));

            AuthResult result = null;
            _authService.LoginWithDeviceId("device-123", r => result = r);

            Assert.IsTrue(result.IsNewAccount);
        }

        [Test]
        public void ReturnFailedResult_WhenLoginWithDeviceIdFails()
        {
            _api.When(x => x.LoginWithCustomID(Arg.Any<LoginWithCustomIDRequest>(), Arg.Any<Action<LoginResult>>(), Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError
                {
                    ErrorMessage = "Service unavailable"
                }));

            AuthResult result = null;
            _authService.LoginWithDeviceId("device-123", r => result = r);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Service unavailable", result.ErrorMessage);
            Assert.IsNull(result.PlayerId);
        }

        [Test]
        public void ReturnFailedResult_WhenDeviceIdIsNull()
        {
            AuthResult result = null;
            _authService.LoginWithDeviceId(null, r => result = r);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            _api.DidNotReceive().LoginWithCustomID(
                Arg.Any<LoginWithCustomIDRequest>(),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void ReturnFailedResult_WhenDeviceIdIsEmpty()
        {
            AuthResult result = null;
            _authService.LoginWithDeviceId("", r => result = r);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            _api.DidNotReceive().LoginWithCustomID(
                Arg.Any<LoginWithCustomIDRequest>(),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        // === LoginWithCredentials ===

        [Test]
        public void CallPlayFabWithCorrectUsernameAndPassword_WhenLoginWithCredentials()
        {
            _authService.LoginWithCredentials("myuser", "mypass123", _ => { });

            _api.Received(1).LoginWithPlayFab(
                Arg.Is<LoginWithPlayFabRequest>(r => r.Username == "myuser" && r.Password == "mypass123"),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void ReturnSuccessResult_WhenLoginWithCredentialsSucceeds()
        {
            _api.When(x => x.LoginWithPlayFab(Arg.Any<LoginWithPlayFabRequest>(), Arg.Any<Action<LoginResult>>(), Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<LoginResult>>().Invoke(new LoginResult
                {
                    PlayFabId = "player-010",
                    SessionTicket = "ticket-xyz",
                    NewlyCreated = false
                }));

            AuthResult result = null;
            _authService.LoginWithCredentials("myuser", "mypass123", r => result = r);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("player-010", result.PlayerId);
            Assert.AreEqual("ticket-xyz", result.SessionToken);
        }

        [Test]
        public void ReturnFailedResult_WhenLoginWithCredentialsFails()
        {
            _api.When(x => x.LoginWithPlayFab(Arg.Any<LoginWithPlayFabRequest>(), Arg.Any<Action<LoginResult>>(), Arg.Any<Action<PlayFabError>>()))
                .Do(c => c.Arg<Action<PlayFabError>>().Invoke(new PlayFabError
                {
                    ErrorMessage = "Invalid credentials"
                }));

            AuthResult result = null;
            _authService.LoginWithCredentials("myuser", "wrongpass", r => result = r);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid credentials", result.ErrorMessage);
        }

        [Test]
        public void ReturnFailedResult_WhenUsernameIsNull()
        {
            AuthResult result = null;
            _authService.LoginWithCredentials(null, "mypass123", r => result = r);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            _api.DidNotReceive().LoginWithPlayFab(
                Arg.Any<LoginWithPlayFabRequest>(),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void ReturnFailedResult_WhenPasswordIsEmpty()
        {
            AuthResult result = null;
            _authService.LoginWithCredentials("myuser", "", r => result = r);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            _api.DidNotReceive().LoginWithPlayFab(
                Arg.Any<LoginWithPlayFabRequest>(),
                Arg.Any<Action<LoginResult>>(),
                Arg.Any<Action<PlayFabError>>()
            );
        }

        [Test]
        public void ReturnFalse_WhenNotLoggedIn()
        {
            _api.IsClientLoggedIn().Returns(false);

            Assert.IsFalse(_authService.IsLoggedIn);
        }

        [Test]
        public void ReturnTrue_WhenLoggedIn()
        {
            _api.IsClientLoggedIn().Returns(true);

            Assert.IsTrue(_authService.IsLoggedIn);
        }

        [Test]
        public void CallForgetAllCredentials_WhenLogout()
        {
            _authService.Logout();

            _api.Received(1).ForgetAllCredentials();
        }
    }
}
