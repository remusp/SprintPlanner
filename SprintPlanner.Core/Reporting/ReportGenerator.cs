using ExcelLibrary.SpreadSheet;
using Newtonsoft.Json.Linq;
using SprintPlanner.Core.Logic;
using System.Collections.Generic;
using System.Linq;

namespace SprintPlanner.Core.Reporting
{
    /*
     ExcelLibrary usage examples:
            worksheet.Cells[0, 1] = new Cell((short)1);
            worksheet.Cells[2, 0] = new Cell(9999999);
            worksheet.Cells[3, 3] = new Cell((decimal)3.45);
            worksheet.Cells[2, 2] = new Cell("Text string");
            worksheet.Cells[2, 4] = new Cell("Second string");
            worksheet.Cells[4, 0] = new Cell(32764.5, "#,##0.00");
            worksheet.Cells[5, 1] = new Cell(DateTime.Now, @"YYYY-MM-DD"); worksheet.Cells.ColumnWidth[0, 1] = 3000;
         */
    public class ReportGenerator
    {
        private IEnumerable<Issue> _issues;
        private IEnumerable<UserLoadModel> _userData;
        private IEnumerable<KeyValuePair<string, List<JContainer>>> _customFields;

        public string StoryPointsField { get; set; }

        public void SetReportData(IEnumerable<Issue> issues, IEnumerable<UserLoadModel> userData, IEnumerable<KeyValuePair<string, List<JContainer>>> customFields)
        {
            _issues = issues;
            _userData = userData;
            _customFields = customFields;
        }

        public bool HasData()
        {
            return _issues != null && _issues.Any();
        }

        public void GenerateReport(string filePath)
        {
            Workbook workbook = new Workbook();
            Worksheet cw = CreateCommitmentWorksheet();
            workbook.Worksheets.Add(cw);

            foreach (var ud in _userData)
            {
                Worksheet w = CreateUserWorksheet(ud);
                workbook.Worksheets.Add(w);

            }

            workbook.Save(filePath);
        }

        private Worksheet CreateUserWorksheet(UserLoadModel m)
        {
            Worksheet w = new Worksheet(m.UserDetails.UserName);
            const int startRow = 3;
            w.Cells[startRow, 0] = new Cell("Story ID");
            w.Cells[startRow, 1] = new Cell("Task ID");
            w.Cells[startRow, 2] = new Cell("Hours");
            w.Cells[startRow, 3] = new Cell("Summary");
            w.Cells[startRow, 4] = new Cell("Task summary");

            int i = startRow + 1;
            foreach (var issue in m.Issues)
            {
                w.Cells[i, 0] = new Cell(issue.StoryId);
                w.Cells[i, 1] = new Cell(issue.TaskId);
                w.Cells[i, 2] = new Cell(issue.Hours);
                w.Cells[i, 3] = new Cell(issue.ParentName);
                w.Cells[i, 4] = new Cell(issue.Name);
                i++;
            }

            return w;
        }

        private Worksheet CreateCommitmentWorksheet()
        {
            var w = new Worksheet("Commitment");
            PreventExcelFormatCrash(w);

            w.Cells[0, 0] = new Cell("Issue Key");
            w.Cells[0, 1] = new Cell("SP");
            w.Cells[0, 2] = new Cell("Summary");

            var issues = _issues.Where(i => !i.fields.issuetype.subtask).ToArray();
            for (int i = 1; i <= issues.Length; i++)
            {
                var spField = GetCustomFieldValueFor(issues[i - 1].key, StoryPointsField);
                w.Cells[i, 0] = new Cell(issues[i - 1].key);
                w.Cells[i, 1] = new Cell(spField.Value<double>());
                w.Cells[i, 2] = new Cell(issues[i - 1].fields.summary);
            }

            return w;
        }

        private JToken GetCustomFieldValueFor(string key, string customFieldName)
        {
            var issueCustomFields = GetFieldsForIssue(key);
            return issueCustomFields.First(icf => icf.Name == customFieldName).Value;
        }

        private IEnumerable<JProperty> GetFieldsForIssue(string key)
        {
            return _customFields.Where(issue => issue.Key == key).Select(kvp => kvp.Value).SelectMany(j => j).Select(p => p as JProperty);
        }

        private static void PreventExcelFormatCrash(Worksheet worksheet)
        {
            for (int i = 0; i < 100; i++)
            {
                worksheet.Cells[i, 0] = new Cell("");
            }
        }
    }
}
