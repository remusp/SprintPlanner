
# SprintPlanner
**Sprint Planner** is a desktop application that simplifies the planning process by showing you an overview of the estimated effort vs. team availability for a **JIRA sprint**.
![MainPlanner](https://user-images.githubusercontent.com/7755563/79144099-0c818480-7dc7-11ea-8d9e-d66e16ea18eb.png)
## Features
- Shows JIRA sprint status as estimated effort vs. team availability
- Easy status update when hours estimation is changed on stories/tasks/bugs
- Quickly assign tasks to other team members to balance capacity
- Story point count (configurable)
- Support for **JIRA Server** and **JIRA Cloud**
- Excel reports
- Installer
## Get started
- Quick start
- [Contributing and upcoming features](./docs/wishlist.md)

## TODOs:
1. Export holidays in excel report
1. Add load in excel report, difficult to see free days/availability 
1. Fix hardcoded issue request
1. Story points field in settings (needed bc technical reasons)
1. Demo recording how to use
1. User name and picture are read from server in each request
1. GO PUBLIC :D \\:D/
1. Dev / QA / other capacity
1. Demo view
1. Multiselect combo for sprint list (just for search feature)
1. Logging support: nlog vs log4net
1. Remove the need for backing fields in VMs AND run CodeMaid \\:D/
1. Hardcoded and duplicated styling properties
1. Decouple window launching functionality from VMs
1. Team Configuration window 
  * Add users from current sprint
  * Search user 
  * "/rest/api/2/user/assignable/search?project=PROJECT_NAME_HERE"
  * https://community.atlassian.com/t5/Answers-Developer-Questions/rest-api-2-user-search-not-returning-user/qaq-p/464379
  * https://community.atlassian.com/t5/Jira-questions/Any-way-to-get-all-users-list-using-JIRA-REST-API/qaq-p/518530
  
## Version History
See [Version History](VersionHistory.md)
    

