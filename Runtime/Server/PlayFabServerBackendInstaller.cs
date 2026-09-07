using GameBackEnd.Server;

namespace PlayFab.Server
{
    public class PlayFabServerBackendInstaller : ServerBackendInstaller
    {
        public override IMatchReportService CreateMatchReportService()
        {
            var playFabServerApi = new PlayFabServerApiAdapter();
            return new PlayFabMatchReportService(playFabServerApi);
        }

        public override IPlayerValidationService CreatePlayerValidationService()
        {
            var playFabServerApi = new PlayFabServerApiAdapter();
            return new PlayFabPlayerValidationService(playFabServerApi);
        }
    }
}
