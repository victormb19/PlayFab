using System;
using GameBackEnd.Server;
using PlayFab.ServerModels;

namespace PlayFab.Server
{
    public class PlayFabPlayerValidationService : IPlayerValidationService
    {
        private readonly IPlayFabServerApi playFabServerApi;

        public PlayFabPlayerValidationService(IPlayFabServerApi playFabServerApi)
        {
            this.playFabServerApi = playFabServerApi;
        }

        public void Validate(string sessionTicket, Action<PlayerValidationResult> onComplete)
        {
            if (string.IsNullOrEmpty(sessionTicket))
            {
                onComplete?.Invoke(PlayerValidationResult.Failed("Session ticket cannot be null or empty."));
                return;
            }

            var request = new AuthenticateSessionTicketRequest { SessionTicket = sessionTicket };

            playFabServerApi.AuthenticateSessionTicket(request,
                result => onComplete?.Invoke(Validated(result)),
                error => onComplete?.Invoke(PlayerValidationResult.Failed(error.ErrorMessage)));
        }

        private static PlayerValidationResult Validated(AuthenticateSessionTicketResult result)
        {
            if (result.IsSessionTicketExpired == true)
                return PlayerValidationResult.Failed("Session ticket is expired.");

            if (result.UserInfo == null || string.IsNullOrEmpty(result.UserInfo.PlayFabId))
                return PlayerValidationResult.Failed("Session ticket carries no account.");

            return PlayerValidationResult.Succeeded(result.UserInfo.PlayFabId);
        }
    }
}
