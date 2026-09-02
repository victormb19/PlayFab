using NUnit.Framework;
using PlayFab.Auth;
using PlayFab.Friends;
using PlayFab.Profile;

namespace PlayFab.Tests
{
    [TestFixture]
    public class PlayFabBackendInstallerShould
    {
        private PlayFabBackendInstaller installer;

        [SetUp]
        public void SetUp()
        {
            installer = new PlayFabBackendInstaller();
        }

        [Test]
        public void Create_Auth_Service_Not_Null()
        {
            var authService = installer.CreateAuthService();

            Assert.That(authService, Is.Not.Null);
        }

        [Test]
        public void Create_Auth_Service_Of_Correct_Type()
        {
            var authService = installer.CreateAuthService();

            Assert.That(authService, Is.TypeOf<PlayFabAuthService>());
        }

        [Test]
        public void Create_Friends_Service_Not_Null()
        {
            var friendsService = installer.CreateFriendsService();

            Assert.That(friendsService, Is.Not.Null);
        }

        [Test]
        public void Create_Friends_Service_Of_Correct_Type()
        {
            var friendsService = installer.CreateFriendsService();

            Assert.That(friendsService, Is.TypeOf<PlayFabFriendsService>());
        }

        [Test]
        public void Create_Profile_Service_Of_Correct_Type()
        {
            var profileService = installer.CreateProfileService();

            Assert.That(profileService, Is.TypeOf<PlayFabProfileService>());
        }
    }
}
