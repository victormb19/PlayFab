using NUnit.Framework;
using PlayFab.Auth;
using UnityEngine;

namespace PlayFab.Tests
{
    [TestFixture]
    public class PlayFabBackendInstallerShould
    {
        private PlayFabBackendInstaller installer;

        [SetUp]
        public void SetUp()
        {
            installer = ScriptableObject.CreateInstance<PlayFabBackendInstaller>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(installer);
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
    }
}
