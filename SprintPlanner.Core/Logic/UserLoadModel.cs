using System.Collections.Generic;

namespace SprintPlanner.Core.Logic
{
    public class UserLoadModel
    {
        public UserLoadModel(UserDetailsModel userDetails, List<IssueModel> issues)
        {
            UserDetails = userDetails;
            Issues = issues;
        }

        public UserDetailsModel UserDetails { get; }

        public decimal Load { get; set; }

        public byte[] PictureData { get; set; }

        public List<IssueModel> Issues { get; set; }

    }
}
