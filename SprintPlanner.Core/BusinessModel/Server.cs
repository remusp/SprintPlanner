using System;

namespace SprintPlanner.Core.BusinessModel
{
    public class Server
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string StoryPointsField { get; set; }

        public bool StoreCredentials { get; set; }

        public string UserName { get; set; }

        public string Pass { get; set; }

        public string UserDisplayName { get; set; }

        public string UserEmail { get; set; }

        public bool IsLoggedIn { get; set; }
        public byte[] UserPictureData { get; set; }

        public void Logout()
        {
            Pass = string.Empty;
            UserDisplayName = string.Empty;
            UserEmail = string.Empty;
            UserPictureData = null;
            IsLoggedIn = false;
        }
    }
}
