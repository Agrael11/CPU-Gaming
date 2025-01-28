using Microsoft.Win32.TaskScheduler;
using System;

class SchedulerHelper
{
    public static void AddToTaskScheduler(string taskName, string exePath)
    {
        using var ts = new TaskService();
        // Check if the task already exists
        var existingTask = ts.GetTask(taskName);
        if (existingTask != null)
        {
            return;
        }

        // Create the task definition
        var td = ts.NewTask();
        td.RegistrationInfo.Description = "Automatically starts CPU Gaming at logon.";
        td.Principal.RunLevel = TaskRunLevel.Highest; // Run with admin rights

        // Set the trigger: On logon
        td.Triggers.Add(new LogonTrigger());

        // Set the action: Run the EXE
        var startDir = exePath.Substring(0, exePath.LastIndexOf('\\'));
        td.Actions.Add(new ExecAction(exePath, null, startDir));

        // Register the task
        ts.RootFolder.RegisterTaskDefinition(taskName, td);
    }
}
