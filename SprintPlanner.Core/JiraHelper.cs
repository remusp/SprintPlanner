using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SprintPlanner.Core
{
    public class JiraHelper
    {
        private static string _username;
        private static string _password;

        public bool Login(string username, string password)
        {
            _username = username;
            _password = password;

            return CheckValidLogin(username, password);
        }

        public List<string> GetAllAssigneesInSprint(int boardId, int sprintId)
        {
            var issues = GetAllIssuesBySprint(boardId, sprintId);
            var persons = issues.Where(l => l.fields.assignee != null).Select(j => j.fields.assignee.name).Distinct().ToList();

            return persons;
        }

        public List<Tuple<string, decimal>> GetAllAssigneesAndWorkInSprint(int boardId, int sprintId)
        {
            var issues = GetAllIssuesBySprint(boardId, sprintId);

            var topLevelIssues = issues.Where(i => i.fields.parent is null).ToList();
            IEnumerable<IGrouping<string, Issue>> issuesPerAssignee = issues.Where(l => l.fields.assignee != null).GroupBy(i => i.fields.assignee.name);
            List<Tuple<string, decimal>> result = new List<Tuple<string, decimal>>();

            foreach (var issuePerAssignee in issuesPerAssignee)
            {
                result.Add(new Tuple<string, decimal>(issuePerAssignee.Key,
                    issuePerAssignee.Sum(ipa => ipa.fields.timetracking.remainingEstimateSeconds)));
            }

            return result;
        }

        public Dictionary<int,string> GetOpenSprints(int boardId)
        {
            var values = GetOpenSprintsbyBoardId(boardId);
            Dictionary<int, string> result = new Dictionary<int, string>();

            foreach (var value in values)
            {
                result.Add(value.id, value.name);
            }

            return result;
        }

        public Dictionary<int, string> GetBoards()
        {
            List<Value> values = GetAllBoards();
            Dictionary<int, string> result = new Dictionary<int, string>();

            foreach (var value in values)
            {
                result.Add(value.id, value.name);
            }

            return result;
        }

        #region private

        private static string GetSprintIssuesPath(int retries, int boardId, int sprintId)
        {
            return "https://jira.sdl.com/rest/agile/1.0/board/"+ boardId +"/sprint/" + sprintId + "/issue?startAt=" + retries * 50 + "&maxResults=" + (retries + 1) * 50;
        }

        private static string GetBoardSprintPath(int retries, int boardId)
        {
            return "https://jira.sdl.com/rest/agile/1.0/board/" + boardId + "/sprint?startAt=" + retries * 50 + "&maxResults=" + (retries + 1) * 50;
        }

        private static string GetBoards(int retries)
        {
            return "https://jira.sdl.com/rest/agile/1.0/board?startAt=" + retries * 50 + "&maxResults=" + (retries + 1) * 50;
        }

        private static string HttpGetByWebRequest(string uri, string username, string password)
        {
            //For Basic Authentication
            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Accept = "application/json; charset=utf-8";

            request.Headers["Authorization"] = "Basic " + authInfo;

            var response = (HttpWebResponse)request.GetResponse();

            string strResponse = "";
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                strResponse = sr.ReadToEnd();
            }

            return strResponse;
        }

        private List<Issue> GetAllIssuesBySprint(int boardId, int sprintId)
        {
            List<Issue> issues = new List<Issue>();
            int? issueCount = null;
            int retries = 0;

            while (!issueCount.HasValue || issueCount.Value > 49)
            {
                string x = HttpGetByWebRequest(GetSprintIssuesPath(retries, boardId, sprintId), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<SprintIssuesDTO>(x);
                issues.AddRange(deserializedCall.issues);
                issueCount = deserializedCall.issues.Count;
                retries++;
            }

            return issues;
        }

        private List<Value> GetOpenSprintsbyBoardId(int boardId)
        {
            List<Value> issues = new List<Value>();
            int? issueCount = null;
            int retries = 0;

            while (!issueCount.HasValue || issueCount.Value > 49)
            {
                string x = HttpGetByWebRequest(GetBoardSprintPath(retries, boardId), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<BoardSprintsDTO>(x);
                issues.AddRange(deserializedCall.values.Where(s=>s.state != "closed"));
                issueCount = deserializedCall.values.Select(s => s.id).Count();
                retries++;
            }

            return issues;
        }

        private List<Value> GetAllBoards()
        {
            List<Value> boards = new List<Value>();
            int? boardCount = null;
            int retries = 0;

            while (!boardCount.HasValue || boardCount.Value > 49)
            {
                string x = HttpGetByWebRequest(GetBoards(retries), _username, _password);
                var deserializedCall = JsonConvert.DeserializeObject<BoardSprintsDTO>(x);
                boards.AddRange(deserializedCall.values.Where(s => s.state != "closed"));
                boardCount = deserializedCall.values.Select(s => s.id).Count();
                retries++;
            }

            return boards;
        }


        private bool CheckValidLogin(string username, string password)
        {
            try
            {
                HttpGetByWebRequest("https://jira.sdl.com/rest/agile/1.0/board/1", _username, _password);
                return true;
            }
            catch (System.Net.WebException e)
            {
                return false;
            }
        }

        #endregion

    }
}
