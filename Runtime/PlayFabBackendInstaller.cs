using GameBackEnd;
using GameBackEnd.Auth;
using GameBackEnd.Friends;
using GameBackEnd.Profile;
using PlayFab.Auth;
using PlayFab.Friends;
using PlayFab.Profile;

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

        public override IProfileService CreateProfileService()
        {
            var api = new PlayFabProfileApiAdapter();
            return new PlayFabProfileService(api);
        }
    }
}
