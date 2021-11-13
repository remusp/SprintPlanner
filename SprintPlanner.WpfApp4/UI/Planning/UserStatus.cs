namespace SprintPlanner.WpfApp.UI.Planning
{
    public enum UserStatus
    {
        /// <summary>
        /// Everything OK
        /// </summary>
        Normal,

        /// <summary>
        /// User does not belong to the configured team. No capacity data available.
        /// </summary>
        External,

        /// <summary>
        /// Work assigned to the user is close to their capacity
        /// </summary>
        Warning,

        /// <summary>
        /// Work assigned to the user is over their capacity
        /// </summary>
        Danger
    }
}
