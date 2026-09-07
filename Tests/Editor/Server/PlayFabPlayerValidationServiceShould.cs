using System;
using GameBackEnd.Server;
using NSubstitute;
using NUnit.Framework;
using PlayFab.ServerModels;

namespace PlayFab.Server.Tests
{
    [TestFixture]
    public class PlayFabPlayerValidationServiceShould
    {
        private IPlayFabServerApi playFabServerApi;
        private PlayFabPlayerValidationService playFabPlayerValidationService;
        private PlayerValidationResult playerValidationResult;

        [SetUp]
        public void SetUp()
        {
            playFabServerApi = Substitute.For<IPlayFabServerApi>();
            playFabPlayerValidationService = new PlayFabPlayerValidationService(playFabServerApi);
            playerValidationResult = null;
        }

        [Test]
        public void Give_The_Player_Id_Behind_The_Ticket()
        {
            AnswerWith(new AuthenticateSessionTicketResult
            {
                IsSessionTicketExpired = false,
                UserInfo = new UserAccountInfo { PlayFabId = "player-1" }
            });

            playFabPlayerValidationService.Validate("ticket", Remember);

            Assert.That(playerValidationResult.Success, Is.True);
            Assert.That(playerValidationResult.PlayerId, Is.EqualTo("player-1"));
        }

        [Test]
        public void Reject_An_Expired_Ticket()
        {
            AnswerWith(new AuthenticateSessionTicketResult
            {
                IsSessionTicketExpired = true,
                UserInfo = new UserAccountInfo { PlayFabId = "player-1" }
            });

            playFabPlayerValidationService.Validate("ticket", Remember);

            Assert.That(playerValidationResult.Success, Is.False);
            Assert.That(playerValidationResult.PlayerId, Is.Null);
        }

        [Test]
        public void Reject_An_Empty_Ticket_Without_Asking_PlayFab()
        {
            playFabPlayerValidationService.Validate(string.Empty, Remember);

            Assert.That(playerValidationResult.Success, Is.False);
            playFabServerApi.DidNotReceiveWithAnyArgs()
                            .AuthenticateSessionTicket(default, default, default);
        }

        private void AnswerWith(AuthenticateSessionTicketResult result)
        {
            playFabServerApi
                .WhenForAnyArgs(api => api.AuthenticateSessionTicket(default, default, default))
                .Do(call => call.Arg<Action<AuthenticateSessionTicketResult>>()(result));
        }

        private void Remember(PlayerValidationResult result)
        {
            playerValidationResult = result;
        }
    }
}
