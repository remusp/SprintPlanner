using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;

namespace SprintPlanner.Core
{
    public class JiraWrapper
    {
        private SecureString _password;
        private string _username;
        private readonly IHttpRequester _webRequester;

        public JiraWrapper(IHttpRequester webRequester)
        {
            _webRequester = webRequester;
        }

        public string Url { get; set; }

        public List<Tuple<string, decimal>> GetAllAssigneesAndWorkInSprint(int sprintId)
        {
            var extendedIssues = GetAllIssuesInSprint(sprintId);

            List<Issue> issues = extendedIssues.Item1;

            IEnumerable<IGrouping<string, Issue>> issuesPerAssignee = issues.Where(l => l.fields.assignee != null).GroupBy(i => i.fields.assignee.name);
            var result = new List<Tuple<string, decimal>>();

            foreach (var issuePerAssignee in issuesPerAssignee)
            {
                result.Add(new Tuple<string, decimal>(issuePerAssignee.Key,
                    issuePerAssignee.Sum(ipa => ipa.fields.timetracking.remainingEstimateSeconds)));
            }

            return result;
        }

        public List<string> GetAllAssigneesInSprint(int sprintId)
        {
            var extendedIssues = GetAllIssuesInSprint(sprintId);
            List<Issue> issues = extendedIssues.Item1;
            var persons = issues.Where(l => l.fields.assignee != null).Select(j => j.fields.assignee.name).Distinct().ToList();

            return persons;
        }

        public Tuple<List<Issue>, Dictionary<string, List<JContainer>>> GetAllIssuesInSprint(int sprintId, List<string> mandatoryFields = null, List<string> customFields = null)
        {
            Dictionary<string, List<JContainer>> customData = new Dictionary<string, List<JContainer>>();
            var issues = new List<Issue>();
            int retries = 0;
            const int pageSize = 1000;

            int issueCount;
            do
            {
                string x = _webRequester.HttpGetByWebRequest(GetSprintIssuesPath(sprintId, pageSize, retries, mandatoryFields), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<SprintIssuesDTO>(x);
                var data = JObject.Parse(x);

                if(customFields != null)
                {
                    foreach (var issue in data["issues"])
                    {
                        List<JContainer> issueCustomValues = new List<JContainer>();
                        foreach (var field in customFields)
                        {
                            issueCustomValues.Add(issue["fields"][field]?.Parent);
                        }

                        customData.Add(issue["key"].Value<string>(), issueCustomValues);
                    }
                }

                issues.AddRange(deserializedCall.issues);
                issueCount = deserializedCall.issues.Count;
                retries++;
            } while (issueCount >= pageSize);

            return new Tuple<List<Issue>, Dictionary<string, List<JContainer>>>(issues, customData);
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

        public List<Tuple<int, string>> GetOpenSprints(int boardId)
        {
            // TODO: investigate if this call can be optimized
            List<Value> values = GetOpenSprintsbyBoardId(boardId);
            var result = new List<Tuple<int, string>>();

            foreach (var value in values)
            {
                result.Add(new Tuple<int, string>(value.id, value.name));
            }

            return result;
        }

        public byte[] GetPicture(string uid)
        {
            string uri = new Uri(Url).Append($"/secure/useravatar?ownerId={uid}").AbsoluteUri;
            return _webRequester.HttpGetBinaryByWebRequest(uri, _username, _password);
        }

        public string GetUserDisplayName(string uid)
        {
            string uri = new Uri(Url).Append($"/rest/api/2/user?username={uid}").AbsoluteUri;
            string x = _webRequester.HttpGetByWebRequest(uri, _username, _password);
            Assignee asignee = JsonConvert.DeserializeObject<Assignee>(x);
            return asignee.displayName;
        }

        public bool Login(string username, SecureString password)
        {
            _username = username;
            _password = password;

            return CheckValidLogin(username, password);
        }

        public void Logout()
        {
            _username = string.Empty;
            _password = null;
        }

        private bool CheckValidLogin(string username, SecureString password)
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

        private List<Value> GetAllBoards()
        {
            var boards = new List<Value>();
            int? boardCount = null;

            for (int retries = 0; !boardCount.HasValue || boardCount.Value > 49; retries++)
            {
                string x = _webRequester.HttpGetByWebRequest(GetBoards(retries), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<BoardSprintsDTO>(x);
                boards.AddRange(deserializedCall.values.Where(s => s.state != "closed"));
                boardCount = deserializedCall.values.Select(s => s.id).Count();
            }

            return boards;
        }

        private string GetBoards(int retries)
        {
            string uri = new Uri(Url).Append("/rest/agile/1.0/board").AbsoluteUri;
            return uri + "?startAt=" + (retries * 50) + "&maxResults=" + ((retries + 1) * 50);
        }

        private string GetBoardSprintPath(int retries, int boardId)
        {
            string uri = new Uri(Url).Append("/rest/agile/1.0/board", boardId.ToString(), "sprint").AbsoluteUri;
            return uri + "?startAt=" + (retries * 50) + "&maxResults=" + ((retries + 1) * 50);
        }

        private List<Value> GetOpenSprintsbyBoardId(int boardId)
        {
            var issues = new List<Value>();
            int? issueCount = null;

            for (int retries = 0; !issueCount.HasValue || issueCount.Value > 49; retries++)
            {
                string x = _webRequester.HttpGetByWebRequest(GetBoardSprintPath(retries, boardId), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<BoardSprintsDTO>(x);
                issues.AddRange(deserializedCall.values.Where(s => s.state != "closed"));
                issueCount = deserializedCall.values.Select(s => s.id).Count();
            }

            return issues;
        }

        private string GetSprintIssuesPath(int sprintId, int pageSize, int startPage, List<string> mandatoryFields)
        {
            string fieldsPart = string.Empty;
            if (mandatoryFields != null)
            {
                fieldsPart = $"&fields={string.Join(",", mandatoryFields)}";
            }

            string uri = new Uri(Url).Append($"/rest/api/2/search?jql=Sprint={sprintId}&startAt={startPage * pageSize}&maxResults={pageSize}{fieldsPart}").AbsoluteUri;
            return uri;
        }
    }
}
