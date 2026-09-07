using System;
using PlayFab.ServerModels;

namespace PlayFab.Server
{
    public class PlayFabServerApiAdapter : IPlayFabServerApi
    {
        public void AuthenticateSessionTicket(AuthenticateSessionTicketRequest request,
                                              Action<AuthenticateSessionTicketResult> onSuccess,
                                              Action<PlayFabError> onError)
        {
            PlayFabServerAPI.AuthenticateSessionTicket(request, onSuccess, onError);
        }

        public void UpdatePlayerStatistics(UpdatePlayerStatisticsRequest request,
                                           Action<UpdatePlayerStatisticsResult> onSuccess,
                                           Action<PlayFabError> onError)
        {
            PlayFabServerAPI.UpdatePlayerStatistics(request, onSuccess, onError);
        }
    }
}
