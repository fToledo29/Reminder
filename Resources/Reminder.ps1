# ---------------------------------------------
# By: Fernando Toledo
# Description: Creates a windows schedule task
# ---------------------------------------------

# -------------------------Global variables-------------------------------------------------
#                          
# Getting the current file system path.
$Global:currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path;
# 
# ------------------------------------------------------------------------------------------

#
# Description:                 Gets the date
#
function GetDateTiem()
{
    $currentDate =  Get-Date;
    $month = $currentDate.Month
    $day = $currentDate.Day;
    $year = $currentDate.Year;
    $date = Get-Date "$month/$day/$year 9:15 AM";
    return $date;
}

#
# Description:                 Creates the schedule tasks.
# $takName                     The Name which will be assigned to the task.
# 
function CreateSchedule([string]$takName)
{
    try
    {
        $description = "This task shows an alert when it's your turn to make the coffee";
        # Getting date time.
        $at = GetDateTiem;
        # Getting current user.
        $user =  Get-WMIObject -class Win32_ComputerSystem | select username;
        # Getting Actions
        $taskAction = New-ScheduledTaskAction -Execute "powershell" -Argument "powershell -WindowStyle Hidden ""$($Global:currentDirectory)\ScheduleTask.ps1"" -ExecutionPolicy ByPass";        
        # Getting Triggers
        $taskTrigger = New-ScheduledTaskTrigger -Daily -At $at -DaysInterval 2;
        # Getting Settings.
        $taskSettings = New-ScheduledTaskSettingsSet -WakeToRun -MultipleInstances Parallel -StartWhenAvailable;
        # Setting Principal.
        $taskPrincipal = New-ScheduledTaskPrincipal -UserId $($user.username) -LogonType Interactive -RunLevel Highest;  
		# Creatting the Schedule Task.  
        Write-Host -ForegroundColor Yellow "Creating chedule task.."
        Register-ScheduledTask -TaskName $takName -Action $taskAction -Principal $taskPrincipal -Description $description -Trigger $taskTrigger -Force -Settings $taskSettings -ErrorAction Stop;
        Write-Host -ForegroundColor Green "Done!";
    }
    catch
    {
        Write-Host -ForegroundColor Red $_;
    }
} 

CreateSchedule -takName "Coffee Reminder";