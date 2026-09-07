using System;
using PlayFab.ServerModels;

namespace PlayFab.Server
{
    public interface IPlayFabServerApi
    {
        void AuthenticateSessionTicket(AuthenticateSessionTicketRequest request,
                                       Action<AuthenticateSessionTicketResult> onSuccess,
                                       Action<PlayFabError> onError);

        void UpdatePlayerStatistics(UpdatePlayerStatisticsRequest request,
                                    Action<UpdatePlayerStatisticsResult> onSuccess,
                                    Action<PlayFabError> onError);
    }
}
