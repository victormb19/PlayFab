using System.Collections.Generic;
using GameBackEnd.Profile;

namespace PlayFab.Profile
{
    public abstract class ProfileServiceObservable
    {
        private readonly List<IProfileServiceObserver> observers = new List<IProfileServiceObserver>();

        public void RegisterObserver(IProfileServiceObserver observer)
        {
            if (observers.Contains(observer))
            {
                return;
            }

            observers.Add(observer);
        }

        public void UnregisterObserver(IProfileServiceObserver observer)
        {
            observers.Remove(observer);
        }

        protected void NotifyProfileLoaded(ProfileResult result)
        {
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
