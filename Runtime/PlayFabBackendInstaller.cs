using GameBackEnd;
using GameBackEnd.Auth;
using PlayFab.Auth;
using UnityEngine;

namespace PlayFab
{
    [CreateAssetMenu(menuName = "GameBackEnd/PlayFab Installer")]
    public class PlayFabBackendInstaller : BackendInstaller
    {
        public override IAuthService CreateAuthService()
        {
            var api = new PlayFabClientApiAdapter();
            return new PlayFabAuthService(api);
        }
    }
}
