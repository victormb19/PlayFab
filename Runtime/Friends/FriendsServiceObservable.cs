using System.Collections.Generic;
using GameBackEnd.Friends;

namespace PlayFab.Friends
{
    public abstract class FriendsServiceObservable
    {
        private readonly List<IFriendsServiceObserver> observers = new List<IFriendsServiceObserver>();

        public void RegisterObserver(IFriendsServiceObserver observer)
        {
            if (observers.Contains(observer))
            {
                return;
            }

            observers.Add(observer);
        }

        public void UnregisterObserver(IFriendsServiceObserver observer)
        {
            observers.Remove(observer);
        }

        protected void NotifyFriendsLoaded(FriendsResult result)
        {
            foreach (var observer in observers)
            {
                observer.OnFriendsLoaded(result);
            }
        }

        protected void NotifyFriendAdded(FriendOperationResult result)
        {
            foreach (var observer in observers)
            {
                observer.OnFriendAdded(result);
            }
        }

        protected void NotifyFriendRemoved(FriendOperationResult result)
        {
            foreach (var observer in observers)
            {
                observer.OnFriendRemoved(result);
            }
        }
    }
}
