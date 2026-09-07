using System;
using System.Collections.Generic;
using GameBackEnd.Server;
using PlayFab.ServerModels;

namespace PlayFab.Server
{
    /// <summary>
    /// Sube el parte de partida como estadisticas de cada jugador. PlayFab escribe
    /// por jugador, asi que la llamada se reparte: el parte no se da por bueno hasta
    /// que responden todos, y el primer fallo se queda como el fallo del parte.
    /// </summary>
    public class PlayFabMatchReportService : IMatchReportService
    {
        public const string ScoreStatistic = "Score";
        public const string WinsStatistic = "Wins";

        private readonly IPlayFabServerApi playFabServerApi;

        public PlayFabMatchReportService(IPlayFabServerApi playFabServerApi)
        {
            this.playFabServerApi = playFabServerApi;
        }

        public void ReportMatch(MatchReport matchReport, Action<MatchReportResult> onComplete)
        {
            if (matchReport == null || matchReport.Players == null || matchReport.Players.Count == 0)
            {
                onComplete?.Invoke(MatchReportResult.Failed("Match report carries no players."));
                return;
            }

            int pending = matchReport.Players.Count;
            string firstError = null;

            void Answered(string errorMessage)
            {
                firstError ??= errorMessage;

                if (--pending > 0) return;

                onComplete?.Invoke(firstError == null
                                       ? MatchReportResult.Succeeded()
                                       : MatchReportResult.Failed(firstError));
            }

            foreach (MatchPlayerScore matchPlayerScore in matchReport.Players)
            {
                var request = new UpdatePlayerStatisticsRequest
                {
                    PlayFabId = matchPlayerScore.PlayerId,
                    Statistics = StatisticsOf(matchReport, matchPlayerScore)
                };

                playFabServerApi.UpdatePlayerStatistics(request,
                    result => Answered(null),
                    error => Answered(error.ErrorMessage));
            }
        }

        private static List<StatisticUpdate> StatisticsOf(MatchReport matchReport, MatchPlayerScore matchPlayerScore)
        {
            bool won = matchPlayerScore.PlayerId == matchReport.WinnerPlayerId;

            return new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = ScoreStatistic, Value = matchPlayerScore.Score },
                new StatisticUpdate { StatisticName = WinsStatistic, Value = won ? 1 : 0 }
            };
        }
    }
}
