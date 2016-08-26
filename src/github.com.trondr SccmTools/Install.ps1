Set-PSDebug -Strict

function Init
{    
    $global:logsDirectory =  [System.IO.Path]::GetFullPath([System.IO.Path]::Combine([System.Environment]::GetEnvironmentVariable("Public"),"Logs"))
    if((Test-Path $logsDirectory) -eq $false)
    {
        [System.IO.Directory]::CreateDirectory($logsDirectory)
    }
    $global:systemDirectory = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::System)
    $global:msiexecExe = [System.IO.Path]::Combine($systemDirectory, "msiexec.exe")
    $global:vendorInstallFolder = [System.IO.Path]::Combine($scriptFolder,"VendorInstall")
    $global:vendorInstallIni = [System.IO.Path]::Combine($vendorInstallFolder,"VendorInstall.ini")
    if((Test-Path $vendorInstallIni) -eq $false)
    {
        Write-Host -ForegroundColor Red "VendorInstall.ini file not found: $vendorInstallIni"
        return 1
    }
    $global:msiFileName = ""
    $global:msiFileName = [Script.Install.Tools.Library.IniFileOperations]::Read($vendorInstallIni,"VendorInstall","MsiFile")
    Write-Verbose "MsiFileName=$msiFileName"
    $global:msiFilePath = [System.IO.Path]::Combine($vendorInstallFolder, $msiFileName)
    Write-Verbose "MsiFilePath=$msiFilePath"    
    if((Test-Path $msiFilePath) -eq $false)
    {
        Write-Host -ForegroundColor Red "Msi file not found: $msiFilePath"
        return 1
    }
    Write-Verbose "SystemDirectory=$systemDirectory"
    return 0
}

function Install
{
    $exitCode = 0
    Write-Host "Installling..."
    $exitCode = StartProcess "$msiexecExe" "/i`"$msiFilePath`" /qn REBOOT=REALLYSUPPRESS /lv! `"$logsDirectory\Install_$msiFileName.log`"" "$vendorInstallFolder" $true       
    return $exitCode
}


function UnInstall
{
    $exitCode = 0
    Write-Host "UnInstalling..."
    $exitCode = StartProcess "$msiexecExe" "/x`"$msiFilePath`" /qn REBOOT=REALLYSUPPRESS /lv! `"$logsDirectory\UnInstall_$msiFileName.log`"" "$vendorInstallFolder" $true
    return $exitCode
}

###############################################################################
#
#   Logging preference
#
###############################################################################
$global:VerbosePreference = "SilentlyContinue"
$global:DebugPreference = "SilentlyContinue"
$global:WarningPreference = "Continue"
$global:ErrorActionPreference = "Continue"
$global:ProgressPreference = "Continue"
###############################################################################
#
#   Start: Main Script - Do not change
#
###############################################################################
$global:script = $MyInvocation.MyCommand.Definition
Write-Verbose "Script=$script"
$global:scriptFolder = Split-Path -parent $script
Write-Verbose "ScriptFolder=$scriptFolder"

###############################################################################
#   Loading script library
###############################################################################
$scriptLibrary = [System.IO.Path]::Combine($scriptFolder ,"Library.ps1")
if((Test-Path $scriptLibrary) -eq $false)
{
    Write-Host -ForegroundColor Red "Script library '$scriptLibrary' not found."
    EXIT 1
}
Write-Verbose "ScriptLibrary=$scriptLibrary"
Write-Verbose "Loading script library '$scriptLibrary'..."
. $scriptLibrary
If ($? -eq $false) 
{ 
    Write-Host -ForegroundColor Red "Failed to load library '$scriptLibrary'. Error: $($error[0])"; break 
    EXIT 1
};
###############################################################################
#   Loading script install library
###############################################################################
$scriptInstallLibraryScript = [System.IO.Path]::Combine($scriptFolder , "Tools","Script.Install.Library.ps1")
if((Test-Path $scriptInstallLibraryScript) -eq $false)
{
    Write-Host -ForegroundColor Red "Script library '$scriptInstallLibraryScript' not found."
    EXIT 1
}
Write-Verbose "ScriptInstallLibraryScript=$scriptInstallLibraryScript"
Write-Verbose "Loading install library script '$scriptInstallLibraryScript'..."
. $scriptInstallLibraryScript
If ($? -eq $false) 
{ 
    Write-Host -ForegroundColor Red "Failed to load library '$scriptLibrary'. Error: $($error[0])"; break 
    EXIT 1
};
###############################################################################
#   Loading script install tools C# library
###############################################################################
Write-Verbose "Loading script install tools C# library..."
$assembly = LoadLibrary(CombinePaths($scriptFolder , "Tools", "Script.Install.Tools.Library", "Common.Logging.dll"))
if($assembly -eq $null)
{
    EXIT 1
}
$assembly = LoadLibrary(CombinePaths($scriptFolder , "Tools", "Script.Install.Tools.Library", "Script.Install.Tools.Library.dll"))
if($assembly -eq $null)
{
    EXIT 1
}
$action = GetAction($args)
Write-Verbose "Action=$action"
Write-Host "Executing Install.ps1..."
Write-Host "Executing $action action..."
switch($action)
{
    "Install"
    {
        $exitCode = ExecuteAction([scriptblock]$function:Init)
        if($exitCode -eq 0)
        {
            $exitCode = ExecuteAction([scriptblock]$function:Install)
        }
        else
        {
            Write-Host -ForegroundColor Red "Initialization failed with error code: $exitCode"
        }
    }

    "UnInstall"
    {
        $exitCode = ExecuteAction([scriptblock]$function:Init)
        if($exitCode -eq 0)
        {
            $exitCode = ExecuteAction([scriptblock]$function:UnInstall)
        }
        else
        {
            Write-Host -ForegroundColor Red "Initialization failed with error code: $exitCode"
        }        
    }
}
Write-Host "Finished executing Install.ps1. Exit code: $exitCode"
EXIT $exitCode
###############################################################################
#
#   End: Main Script - Do not change
#
###############################################################################
