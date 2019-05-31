using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SprintPlanner.Core
{
    public class JiraWrapper
    {
        private string _username;
        private string _password;
        private IHttpRequester _webRequester;

        public JiraWrapper(IHttpRequester webRequester)
        {
            _webRequester = webRequester;
        }

        public string Url { get; set; }

        public bool Login(string username, string password)
        {
            _username = username;
            _password = password;

            return CheckValidLogin(username, password);
        }

        public List<string> GetAllAssigneesInSprint(int boardId, int sprintId)
        {
            List<Issue> issues = GetAllIssuesBySprint(boardId, sprintId);
            var persons = issues.Where(l => l.fields.assignee != null).Select(j => j.fields.assignee.name).Distinct().ToList();

            return persons;
        }

        public List<Tuple<string, decimal>> GetAllAssigneesAndWorkInSprint(int boardId, int sprintId)
        {
            List<Issue> issues = GetAllIssuesBySprint(boardId, sprintId);

            IEnumerable<IGrouping<string, Issue>> issuesPerAssignee = issues.Where(l => l.fields.assignee != null).GroupBy(i => i.fields.assignee.name);
            var result = new List<Tuple<string, decimal>>();

            foreach (var issuePerAssignee in issuesPerAssignee)
            {
                result.Add(new Tuple<string, decimal>(issuePerAssignee.Key,
                    issuePerAssignee.Sum(ipa => ipa.fields.timetracking.remainingEstimateSeconds)));
            }

            return result;
        }


        public IEnumerable<IGrouping<string, Issue>> GetIssuesPerAssignee(int boardId, int sprintId)
        {
            List<Issue> issues = GetAllIssuesBySprint(boardId, sprintId);
            // Status "6" = Done
            return issues.Where(l => l.fields.subtasks.Count == 0 && l.fields.status.id != "6" && l.fields.assignee != null).GroupBy(i => i.fields.assignee.name);
        }

        public IEnumerable<Issue> GetUnassignedIssues(int boardId, int sprintId)
        {
            List<Issue> issues = GetAllIssuesBySprint(boardId, sprintId);
            // Status "6" = Done
            return issues.Where(l => l.fields.subtasks.Count == 0 && l.fields.status.id != "6" && l.fields.assignee == null);
        }

        public List<Tuple<int, string>> GetOpenSprints(int boardId)
        {
            List<Value> values = GetOpenSprintsbyBoardId(boardId);
            var result = new List<Tuple<int, string>>();

            foreach (var value in values)
            {
                result.Add(new Tuple<int, string>(value.id, value.name));
            }

            return result;
        }

        public Dictionary<int, string> GetBoards()
        {
            List<Value> values = GetAllBoards();
            var result = new Dictionary<int, string>();

            foreach (var value in values)
            {
                result.Add(value.id, value.name);
            }

            return result;
        }

        public string GetUserDisplayName(string uid)
        {
            string uri = new Uri(Url).Append($"/rest/api/2/user?username={uid}").AbsoluteUri;
            string x = _webRequester.HttpGetByWebRequest(uri, _username, _password);
            Assignee asignee = JsonConvert.DeserializeObject<Assignee>(x);
            return asignee.displayName;
        }

        #region private

        private string GetSprintIssuesPath(int retries, int boardId, int sprintId)
        {
            string uri = new Uri(Url).Append("/rest/agile/1.0/board", boardId.ToString(), "sprint", sprintId.ToString(), "issue").AbsoluteUri;
            return uri + "?startAt=" + (retries * 50) + "&maxResults=" + ((retries + 1) * 50);
        }

        private string GetBoardSprintPath(int retries, int boardId)
        {
            string uri = new Uri(Url).Append("/rest/agile/1.0/board", boardId.ToString(), "sprint").AbsoluteUri;
            return uri + "?startAt=" + (retries * 50) + "&maxResults=" + ((retries + 1) * 50);
        }

        private string GetBoards(int retries)
        {
            string uri = new Uri(Url).Append("/rest/agile/1.0/board").AbsoluteUri;
            return uri + "?startAt=" + (retries * 50) + "&maxResults=" + ((retries + 1) * 50);
        }

        private List<Issue> GetAllIssuesBySprint(int boardId, int sprintId)
        {
            var issues = new List<Issue>();
            int? issueCount = null;
            int retries = 0;

            while (!issueCount.HasValue || issueCount.Value > 49)
            {
                string x = _webRequester.HttpGetByWebRequest(GetSprintIssuesPath(retries, boardId, sprintId), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<SprintIssuesDTO>(x);
                issues.AddRange(deserializedCall.issues);
                issueCount = deserializedCall.issues.Count;
                retries++;
            }

            return issues;
        }

        private List<Value> GetOpenSprintsbyBoardId(int boardId)
        {
            var issues = new List<Value>();
            int? issueCount = null;
            int retries = 0;

            while (!issueCount.HasValue || issueCount.Value > 49)
            {
                string x = _webRequester.HttpGetByWebRequest(GetBoardSprintPath(retries, boardId), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<BoardSprintsDTO>(x);
                issues.AddRange(deserializedCall.values.Where(s => s.state != "closed"));
                issueCount = deserializedCall.values.Select(s => s.id).Count();
                retries++;
            }

            return issues;
        }

        private List<Value> GetAllBoards()
        {
            var boards = new List<Value>();
            int? boardCount = null;
            int retries = 0;

            while (!boardCount.HasValue || boardCount.Value > 49)
            {
                string x = _webRequester.HttpGetByWebRequest(GetBoards(retries), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<BoardSprintsDTO>(x);
                boards.AddRange(deserializedCall.values.Where(s => s.state != "closed"));
                boardCount = deserializedCall.values.Select(s => s.id).Count();
                retries++;
            }

            return boards;
        }

        public byte[] GetPicture(string uid)
        {
            string uri = new Uri(Url).Append($"/secure/useravatar?ownerId={uid}").AbsoluteUri;
            return _webRequester.HttpGetBinaryByWebRequest(uri, _username, _password);
        }

        private bool CheckValidLogin(string username, string password)
        {
            try
            {
                string uri = new Uri(Url).Append("/rest/agile/1.0/board/1").AbsoluteUri;
                _webRequester.HttpGetByWebRequest(uri, _username, _password);
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        #endregion

    }
}
