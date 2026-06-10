using GameBackEnd;
using GameBackEnd.Auth;
using GameBackEnd.Friends;
using PlayFab.Auth;
using PlayFab.Friends;

namespace PlayFab
{
    public class PlayFabBackendInstaller : BackendInstaller
    {
        public override IAuthService CreateAuthService()
        {
            var api = new PlayFabClientApiAdapter();
            return new PlayFabAuthService(api);
        }

        public override IFriendsService CreateFriendsService()
        {
            var api = new PlayFabFriendsApiAdapter();
            return new PlayFabFriendsService(api);
        }
    }
}
