using System;
using System.Collections.Generic;
using GameBackEnd.Server;
using NSubstitute;
using NUnit.Framework;
using PlayFab.ServerModels;

namespace PlayFab.Server.Tests
{
    [TestFixture]
    public class PlayFabMatchReportServiceShould
    {
        private IPlayFabServerApi playFabServerApi;
        private PlayFabMatchReportService playFabMatchReportService;
        private MatchReportResult matchReportResult;

        [SetUp]
        public void SetUp()
        {
            playFabServerApi = Substitute.For<IPlayFabServerApi>();
            playFabMatchReportService = new PlayFabMatchReportService(playFabServerApi);
            matchReportResult = null;
        }

        [Test]
        public void Write_The_Score_Of_Every_Player()
        {
            playFabMatchReportService.ReportMatch(Match(), Remember);

            playFabServerApi.Received().UpdatePlayerStatistics(
                Arg.Is<UpdatePlayerStatisticsRequest>(
                    request => request.PlayFabId == "player-1" && Value(request, PlayFabMatchReportService.ScoreStatistic) == 12),
                Arg.Any<Action<UpdatePlayerStatisticsResult>>(), Arg.Any<Action<PlayFabError>>());

            playFabServerApi.Received().UpdatePlayerStatistics(
                Arg.Is<UpdatePlayerStatisticsRequest>(
                    request => request.PlayFabId == "player-2" && Value(request, PlayFabMatchReportService.ScoreStatistic) == 20),
                Arg.Any<Action<UpdatePlayerStatisticsResult>>(), Arg.Any<Action<PlayFabError>>());
        }

        [Test]
        public void Mark_One_Win_For_The_Winner_And_None_For_The_Rest()
        {
            playFabMatchReportService.ReportMatch(Match(), Remember);

            playFabServerApi.Received().UpdatePlayerStatistics(
                Arg.Is<UpdatePlayerStatisticsRequest>(
                    request => request.PlayFabId == "player-2" && Value(request, PlayFabMatchReportService.WinsStatistic) == 1),
                Arg.Any<Action<UpdatePlayerStatisticsResult>>(), Arg.Any<Action<PlayFabError>>());

            playFabServerApi.Received().UpdatePlayerStatistics(
                Arg.Is<UpdatePlayerStatisticsRequest>(
                    request => request.PlayFabId == "player-1" && Value(request, PlayFabMatchReportService.WinsStatistic) == 0),
                Arg.Any<Action<UpdatePlayerStatisticsResult>>(), Arg.Any<Action<PlayFabError>>());
        }

        [Test]
        public void Hold_The_Answer_Until_Every_Player_Is_Written()
        {
            AnswerOnly("player-1");

            playFabMatchReportService.ReportMatch(Match(), Remember);

            Assert.That(matchReportResult, Is.Null);
        }

        [Test]
        public void Report_Success_When_Every_Player_Is_Written()
        {
            AnswerAll();

            playFabMatchReportService.ReportMatch(Match(), Remember);

            Assert.That(matchReportResult.Success, Is.True);
        }

        [Test]
        public void Keep_The_Error_Of_The_Player_That_Failed()
        {
            AnswerAllButFailing("player-2", "statistic not found");

            playFabMatchReportService.ReportMatch(Match(), Remember);

            Assert.That(matchReportResult.Success, Is.False);
            Assert.That(matchReportResult.ErrorMessage, Is.EqualTo("statistic not found"));
        }

        [Test]
        public void Refuse_A_Report_Without_Players()
        {
            playFabMatchReportService.ReportMatch(MatchReport.Of("match-1", new MatchPlayerScore[0]), Remember);

            Assert.That(matchReportResult.Success, Is.False);
            playFabServerApi.DidNotReceiveWithAnyArgs()
                            .UpdatePlayerStatistics(default, default, default);
        }

        private static MatchReport Match()
        {
            return MatchReport.Of("match-1", new[]
            {
                new MatchPlayerScore("player-1", 12),
                new MatchPlayerScore("player-2", 20)
            });
        }

        private static int Value(UpdatePlayerStatisticsRequest request, string statisticName)
        {
            foreach (StatisticUpdate statisticUpdate in request.Statistics)
                if (statisticUpdate.StatisticName == statisticName)
                    return statisticUpdate.Value;

            return int.MinValue;
        }

        private void AnswerAll()
        {
            playFabServerApi
                .WhenForAnyArgs(api => api.UpdatePlayerStatistics(default, default, default))
                .Do(call => call.Arg<Action<UpdatePlayerStatisticsResult>>()(new UpdatePlayerStatisticsResult()));
        }

        private void AnswerOnly(string playerId)
        {
            playFabServerApi
                .When(api => api.UpdatePlayerStatistics(
                          Arg.Is<UpdatePlayerStatisticsRequest>(request => request.PlayFabId == playerId),
                          Arg.Any<Action<UpdatePlayerStatisticsResult>>(), Arg.Any<Action<PlayFabError>>()))
                .Do(call => call.Arg<Action<UpdatePlayerStatisticsResult>>()(new UpdatePlayerStatisticsResult()));
        }

        private void AnswerAllButFailing(string playerId, string errorMessage)
        {
            AnswerAll();

            playFabServerApi
                .When(api => api.UpdatePlayerStatistics(
                          Arg.Is<UpdatePlayerStatisticsRequest>(request => request.PlayFabId == playerId),
                          Arg.Any<Action<UpdatePlayerStatisticsResult>>(), Arg.Any<Action<PlayFabError>>()))
                .Do(call => call.Arg<Action<PlayFabError>>()(new PlayFabError { ErrorMessage = errorMessage }));
        }

        private void Remember(MatchReportResult result)
        {
            matchReportResult = result;
        }
    }
}
