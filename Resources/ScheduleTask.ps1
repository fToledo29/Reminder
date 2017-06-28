# ---------------------------------------------
# By: Fernando Toledo
# Description: Validates if is the time to show the alerts.
# ---------------------------------------------

# -------------------------Global variables-------------------------------------------------
#                          
# Getting the current file system path.
$Global:currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path;
# Importing Modules
Import-Module -DisableNameChecking "$Global:currentDirectory\Alerts" -Force;
# 
# ------------------------------------------------------------------------------------------

#
# Description:                 Gets the current time
#
function ValidateTime([boolean]$popup)
{
    $counter = 0;
    if(((Get-Date).Hour -ge 9) -and ((Get-Date).Minute -ge 15) -and $popup)
    {                
        PopupAlert;
    }
    elseif(((Get-Date).Hour -ge 9) -and ((Get-Date).Minute -ge 25) -and !$popup)
    {                
        WindowsAlert
    }
    else
    {
        $counter++;
        sleep -Seconds 5;
        if($counter -ge 30)
        {
            return;
        }
        else
        {
            ValidateTime -popup $popup;
        }
    }
}

function Start()
{
    try
    {
        $shell = New-Object -ComObject "Shell.Application";
        $shell.MinimizeAll();
        sleep -Seconds 2;
        ValidateTime -popup $false;
        $shell.undominimizeall();
        sleep -Seconds 900;
        $shell.MinimizeAll();
        ValidateTime -popup $true;
        $shell.undominimizeall();
    }
    catch
    {
        Write-Host -ForegroundColor Red $_;
    }
}

Start;
