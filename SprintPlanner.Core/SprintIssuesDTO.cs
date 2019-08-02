using System;
using System.Collections.Generic;

namespace SprintPlanner.Core
{
    public class StatusCategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }

    public class Status
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public StatusCategory statusCategory { get; set; }
    }

    public class Priority
    {
        public string self { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Issuetype
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
        public int avatarId { get; set; }
    }

    public class Parent
    {
        public string id { get; set; }
        public string key { get; set; }
        public string self { get; set; }
        public Fields fields { get; set; }
    }

    public class Customfield10630
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

    public class AvatarUrls
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class Assignee
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class Creator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class Subtask
    {
        public string id { get; set; }
        public string key { get; set; }
        public string self { get; set; }
        public Fields fields { get; set; }
    }

    public class Reporter
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class Aggregateprogress
    {
        public int progress { get; set; }
        public int total { get; set; }
        public int percent { get; set; }
    }

    public class Progress
    {
        public int progress { get; set; }
        public int total { get; set; }
        public int percent { get; set; }
    }

    public class Votes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    public class Author
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class UpdateAuthor
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class Worklog
    {
        public string self { get; set; }
        public Author author { get; set; }
        public UpdateAuthor updateAuthor { get; set; }
        public string comment { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public DateTime started { get; set; }
        public string timeSpent { get; set; }
        public int timeSpentSeconds { get; set; }
        public string id { get; set; }
        public string issueId { get; set; }
    }

    public class Worklogs
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public IList<Worklog> worklogs { get; set; }
    }

    public class Sprint
    {
        public int id { get; set; }
        public string self { get; set; }
        public string state { get; set; }
        public string name { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int originBoardId { get; set; }
        public string goal { get; set; }
    }

    public class ProjectCategory
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
    }

    public class Project
    {
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public ProjectCategory projectCategory { get; set; }
    }

    public class Watches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }

    public class Customfield13532
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

    public class Timetracking
    {
        public string originalEstimate { get; set; }
        public string remainingEstimate { get; set; }
        public string timeSpent { get; set; }
        public int originalEstimateSeconds { get; set; }
        public int remainingEstimateSeconds { get; set; }
        public int timeSpentSeconds { get; set; }
    }

    public class Comment
    {
        public IList<object> comments { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public int startAt { get; set; }
    }

    public class Fields
    {
        public Parent parent { get; set; }
        public object customfield_10070 { get; set; }
        public object customfield_12130 { get; set; }
        public object customfield_14031 { get; set; }
        public IList<object> fixVersions { get; set; }
        public object customfield_14832 { get; set; }
        public object resolution { get; set; }
        public Customfield10630 customfield_10630 { get; set; }
        public object customfield_14830 { get; set; }
        public object customfield_13856 { get; set; }
        public object customfield_13855 { get; set; }
        public object customfield_13858 { get; set; }
        public DateTime? lastViewed { get; set; }
        public object customfield_15231 { get; set; }
        public object customfield_15232 { get; set; }
        public object customfield_11030 { get; set; }
        public object customfield_15230 { get; set; }
        public object customfield_15631 { get; set; }
        public object customfield_13330 { get; set; }
        public object customfield_15632 { get; set; }
        public object customfield_13333 { get; set; }
        public object customfield_13850 { get; set; }
        public object customfield_15233 { get; set; }
        public object customfield_13332 { get; set; }
        public object customfield_15630 { get; set; }
        public Priority priority { get; set; }
        public object customfield_13852 { get; set; }
        public object customfield_13854 { get; set; }
        public object customfield_15633 { get; set; }
        public IList<object> labels { get; set; }
        public object customfield_13853 { get; set; }
        public object customfield_11942 { get; set; }
        public object customfield_13845 { get; set; }
        public object customfield_11941 { get; set; }
        public object customfield_13844 { get; set; }
        public object customfield_11944 { get; set; }
        public object customfield_11943 { get; set; }
        public object customfield_13846 { get; set; }
        public object customfield_11946 { get; set; }
        public object customfield_13849 { get; set; }
        public int? aggregatetimeoriginalestimate { get; set; }
        public int? timeestimate { get; set; }
        public object customfield_11945 { get; set; }
        public IList<object> versions { get; set; }
        public object customfield_13848 { get; set; }
        public object customfield_11947 { get; set; }
        public IList<object> issuelinks { get; set; }
        public Assignee assignee { get; set; }
        public Status status { get; set; }
        public object customfield_16031 { get; set; }
        public IList<object> components { get; set; }
        public object customfield_16035 { get; set; }
        public object customfield_14132 { get; set; }
        public object customfield_16034 { get; set; }
        public object customfield_16032 { get; set; }
        public object customfield_14135 { get; set; }
        public object customfield_14531 { get; set; }
        public object customfield_12230 { get; set; }
        public object customfield_14133 { get; set; }
        public object customfield_14134 { get; set; }
        public string customfield_12631 { get; set; }
        public object customfield_14535 { get; set; }
        public object customfield_14932 { get; set; }
        public object customfield_11931 { get; set; }
        public int? aggregatetimeestimate { get; set; }
        public object customfield_11937 { get; set; }
        public Creator creator { get; set; }
        public string customfield_15330 { get; set; }
        public IList<Subtask> subtasks { get; set; }
        public Reporter reporter { get; set; }
        public Aggregateprogress aggregateprogress { get; set; }
        public object customfield_13830 { get; set; }
        public object customfield_13831 { get; set; }
        public object customfield_10830 { get; set; }
        public Progress progress { get; set; }
        public Votes votes { get; set; }
        public Worklog worklog { get; set; }
        public Issuetype issuetype { get; set; }
        public object customfield_14230 { get; set; }
        public object customfield_14231 { get; set; }
        public int? timespent { get; set; }
        public Sprint sprint { get; set; }
        public object customfield_14630 { get; set; }
        public object customfield_14631 { get; set; }
        public Project project { get; set; }
        public int? aggregatetimespent { get; set; }
        public object customfield_13537 { get; set; }
        public object customfield_13933 { get; set; }
        public object customfield_13536 { get; set; }
        public object customfield_11633 { get; set; }
        public string customfield_13539 { get; set; }
        public object customfield_15835 { get; set; }
        public object customfield_13934 { get; set; }
        public object resolutiondate { get; set; }
        public long workratio { get; set; }
        public object customfield_15030 { get; set; }
        public Watches watches { get; set; }
        public object customfield_15033 { get; set; }
        public object customfield_15034 { get; set; }
        public object customfield_15430 { get; set; }
        public object customfield_15031 { get; set; }
        public DateTime created { get; set; }
        public object customfield_13130 { get; set; }
        public object customfield_15032 { get; set; }
        public object customfield_14740 { get; set; }
        public object customfield_14741 { get; set; }
        public object customfield_13531 { get; set; }
        public object customfield_13135 { get; set; }
        public object customfield_15035 { get; set; }
        public string customfield_12442 { get; set; }
        public object customfield_15431 { get; set; }
        public string customfield_11630 { get; set; }
        public object customfield_15833 { get; set; }
        public string customfield_12443 { get; set; }
        public IList<Customfield13532> customfield_13532 { get; set; }
        public object customfield_13136 { get; set; }
        public object customfield_15834 { get; set; }
        public object customfield_13535 { get; set; }
        public object customfield_11632 { get; set; }
        public object customfield_14742 { get; set; }
        public object customfield_13534 { get; set; }
        public IList<string> customfield_11631 { get; set; }
        public string customfield_15832 { get; set; }
        public object customfield_12437 { get; set; }
        public object customfield_14737 { get; set; }
        public object customfield_10930 { get; set; }
        public object customfield_14738 { get; set; }
        public object customfield_14735 { get; set; }
        public object customfield_14736 { get; set; }
        public object customfield_14739 { get; set; }
        public DateTime updated { get; set; }
        public int? timeoriginalestimate { get; set; }
        public string description { get; set; }
        public object customfield_10010 { get; set; }
        public object customfield_10011 { get; set; }
        public object customfield_14733 { get; set; }
        public object customfield_14731 { get; set; }
        public Timetracking timetracking { get; set; }
        public IList<object> attachment { get; set; }
        public bool flagged { get; set; }
        public string summary { get; set; }
        public object customfield_15132 { get; set; }
        public object customfield_13230 { get; set; }
        public object customfield_15130 { get; set; }
        public object customfield_15131 { get; set; }
        public object customfield_13231 { get; set; }
        public object customfield_13630 { get; set; }
        public object customfield_12541 { get; set; }
        public object customfield_10120 { get; set; }
        public object customfield_15530 { get; set; }
        public object customfield_10121 { get; set; }
        public object customfield_15531 { get; set; }
        public object customfield_15932 { get; set; }
        public object customfield_15933 { get; set; }
        public object customfield_15931 { get; set; }
        public Comment comment { get; set; }
        public double? customfield_10013 { get; set; }
        public object customfield_10014 { get; set; }
    }

    public class Issue
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public Fields fields { get; set; }
    }

    public class SprintIssuesDTO
    {
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<Issue> issues { get; set; }
    }
}
