rm SprintPlanner.InstallerSfx.exe
.\7zr.exe a .\Temp\SprintPlannerFiles.7z .\Release\*
cmd /c copy /b 7zS2.sfx + bootstrapper.config + .\Temp\SprintPlannerFiles.7z SprintPlanner.InstallerSfx.exe
rm -r Temp