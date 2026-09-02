using System.Collections.Generic;
using GameBackEnd.Profile;
using PlayFab.ClientModels;

namespace PlayFab.Profile
{
    public class PlayFabProfileService : ProfileServiceObservable, IProfileService
    {
        private const string AvatarKey = "avatarId";
        private static readonly List<string> AvatarKeys = new List<string> { AvatarKey };

        private readonly IPlayFabProfileApi profileApi;

        public PlayFabProfileService(IPlayFabProfileApi profileApi)
        {
            this.profileApi = profileApi;
        }

        public void LoadProfile()
        {
            profileApi.GetAccountInfo(new GetAccountInfoRequest(),
                account => LoadAvatar(account.AccountInfo),
                error => NotifyProfileLoaded(ProfileResult.Failed(error.ErrorMessage)));
        }

        private void LoadAvatar(UserAccountInfo account)
        {
            profileApi.GetUserData(new GetUserDataRequest { Keys = AvatarKeys },
                data => NotifyProfileLoaded(ProfileResult.Succeeded(Map(account, data))),
                error => NotifyProfileLoaded(ProfileResult.Failed(error.ErrorMessage)));
        }

        private static ProfileData Map(UserAccountInfo account, GetUserDataResult data)
        {
            return new ProfileData(account.PlayFabId, account.TitleInfo?.DisplayName, ReadAvatarId(data));
        }

        private static string ReadAvatarId(GetUserDataResult data)
        {
            return data.Data != null && data.Data.TryGetValue(AvatarKey, out var record)
                ? record.Value
                : string.Empty;
        }

        public void UpdateDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                NotifyDisplayNameUpdated(ProfileOperationResult.Failed("Display name cannot be null or empty."));
                return;
            }

            var request = new UpdateUserTitleDisplayNameRequest { DisplayName = displayName };

            profileApi.UpdateUserTitleDisplayName(request,
                result => NotifyDisplayNameUpdated(ProfileOperationResult.Succeeded()),
                error => NotifyDisplayNameUpdated(ProfileOperationResult.Failed(error.ErrorMessage)));
        }

        public void UpdateAvatar(string avatarId)
        {
            if (string.IsNullOrEmpty(avatarId))
            {
                NotifyAvatarUpdated(ProfileOperationResult.Failed("Avatar id cannot be null or empty."));
                return;
            }

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> { { AvatarKey, avatarId } }
            };

            profileApi.UpdateUserData(request,
                result => NotifyAvatarUpdated(ProfileOperationResult.Succeeded()),
                error => NotifyAvatarUpdated(ProfileOperationResult.Failed(error.ErrorMessage)));
        }
    }
}
