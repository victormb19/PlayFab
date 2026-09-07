using System;

namespace PlayFab.Server
{
    /// <summary>
    /// La clave secreta no vive en ningun asset: PlayFabSharedSettings esta en
    /// Resources y viaja tambien en la build del jugador, donde se saca del binario.
    /// El servidor la toma del entorno al arrancar.
    /// </summary>
    public static class PlayFabServerCredentials
    {
        public const string SecretKeyVariable = "PLAYFAB_SECRET_KEY";
        public const string TitleIdVariable = "PLAYFAB_TITLE_ID";

        public static bool LoadFromEnvironment()
        {
            return Load(Environment.GetEnvironmentVariable(SecretKeyVariable),
                        Environment.GetEnvironmentVariable(TitleIdVariable));
        }

        public static bool Load(string secretKey, string titleId)
        {
            if (string.IsNullOrEmpty(secretKey)) return false;

            PlayFabSettings.staticSettings.DeveloperSecretKey = secretKey;

            if (!string.IsNullOrEmpty(titleId))
                PlayFabSettings.staticSettings.TitleId = titleId;

            return true;
        }
    }
}
