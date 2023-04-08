### Version 0.0.10 (2023-04-08)
 * Moved to .NET 6
 * Added plan statistics view
 * Fixed TLDR Legal link (broken due to framework update)

### Version 0.0.9 (2021-03-28)
 * Refresh button for sprint list

### Version 0.0.8 (2020-05-01)
 * Update notifications
 * MVVM infrastructure refactoring

### Version 0.0.7 (2020-04-13)
 * JIRA Cloud support

### Version 0.0.6 (2019-11-18)
 * Issues can now be reassigned using context menu
 * Fixed Export to excel crash for story with no SP
 * Sprint data path default to MyDocuments, selectable sprint data path
 * Proper handling for percent availability and no capacity info

### Version 0.0.5 (2019-11-11)
 * Export to excel
 * Fixed selected sprint loss in planning when jumping to other views
 * Planning view remembers collapsed users after data is refreshed
 * Percent availability (needs tweaks) 
 
### Version 0.0.4 (2019-08-09)
 * Application icon
 * Fixed hardcoded links to JIRA 
 * Progress spinner for planning data
 * Optimized sprint data fetching 
 * Fixed some board and sprint selection issues in combos
 * Added story points
 * Logged-in user displayed in title bar
 * Logout button in title bar
 * Fixed bug causing user data to not be loaded when there is no team configured
 * Switched to tabbed views instead of windows
 * Added About view
 * Added Settings view
 * Fixed negative capacity
 * Introduced SecureString for password
 * Planning and capacity views are enabled only if user is logged in
 
### Version 0.0.3 (2019-06-03)
 * Added user pictures
 * Replaced buttons in sprint planning main view with a hamburger menu
 * Switched to Mahapps look for combos, numeric-up-downs, buttons, grids
 * Merged external and unassigned user data with the regular team data
 * Styling updates
 * Expander for stories and tasks section
 * Introduced color coding for planning view:
   * Light Blue: Normal
   * Light Purple: User is not in the team, no capacity info
   * Light Yellow: Low or high load
   * Light Red: Very high load
 * Added Hyperlinks for story and task

### Version 0.0.2 (2019-05-14)
 * MVP
