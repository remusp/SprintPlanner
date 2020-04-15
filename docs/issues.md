# Known issues
1. Planning view: Executing a 'Read boards' action when the boards drop-down is already populated will crash the application.  
**Workaround:** Close the application, then delete the file `Sprint.spl.json` from folder `C:\Users\[user]\Documents\Sprint Planner`
1. Team capacity view: Unable to add a specific team member to the grid  
**Workaround:** Assign a task to the team member in JIRA and hit `Refresh` / F5
1. SVG avatar images are not supported
1. Title bar avatar of logged-in user is not shown when connected to JIRA Cloud instances
