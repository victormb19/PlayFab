using System.Collections.Generic;
using GameBackEnd.Profile;

namespace PlayFab.Profile
{
    public abstract class ProfileServiceObservable
    {
        private readonly List<IProfileServiceObserver> observers = new List<IProfileServiceObserver>();

        private ProfileResult lastProfile;

        public void RegisterObserver(IProfileServiceObserver observer)
        {
            if (observers.Contains(observer))
            {
                return;
            }

            observers.Add(observer);

            // El login es asincrono: quien se registra despues de la carga
            // seguiria esperando un aviso que ya paso.
            if (lastProfile != null)
            {
                observer.OnProfileLoaded(lastProfile);
            }
        }

        public void UnregisterObserver(IProfileServiceObserver observer)
        {
            observers.Remove(observer);
        }

        protected void NotifyProfileLoaded(ProfileResult result)
        {
            lastProfile = result;

            foreach (var observer in observers)
            {
                observer.OnProfileLoaded(result);
            }
        }

        protected void NotifyDisplayNameUpdated(ProfileOperationResult result)
        {
            foreach (var observer in observers)
            {
                observer.OnDisplayNameUpdated(result);
            }
        }

        protected void NotifyAvatarUpdated(ProfileOperationResult result)
        {
            foreach (var observer in observers)
            {
                observer.OnAvatarUpdated(result);
            }
        }
    }
}
