param(
    [string]$ExecutablePath = (Join-Path (Split-Path $PSScriptRoot -Parent) 'WindowShelf.exe'),
    [ValidateRange(1,3600)][int]$DelaySeconds = 30,
    [string]$TaskName = 'WindowShelf'
)
$ErrorActionPreference = 'Stop'
$resolved = (Resolve-Path -LiteralPath $ExecutablePath).Path
if ([IO.Path]::GetFileName($resolved) -ne 'WindowShelf.exe') { throw 'Expected WindowShelf.exe.' }
$user = [Security.Principal.WindowsIdentity]::GetCurrent().Name
$existing = Get-ScheduledTask -TaskName $TaskName -TaskPath '\' -ErrorAction SilentlyContinue
if ($existing) {
    $paths = @($existing.Actions | ForEach-Object { $_.Execute })
    if ($paths.Count -ne 1 -or -not [string]::Equals($paths[0],$resolved,[StringComparison]::OrdinalIgnoreCase)) {
        throw 'A different task already uses this name. Choose another TaskName.'
    }
}
$action = New-ScheduledTaskAction -Execute $resolved -WorkingDirectory (Split-Path $resolved -Parent)
$trigger = New-ScheduledTaskTrigger -AtLogOn -User $user
$trigger.Delay = 'PT' + $DelaySeconds + 'S'
$principal = New-ScheduledTaskPrincipal -UserId $user -LogonType Interactive -RunLevel Limited
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -ExecutionTimeLimit ([TimeSpan]::Zero) -MultipleInstances IgnoreNew
Register-ScheduledTask -TaskName $TaskName -TaskPath '\' -Action $action -Trigger $trigger -Principal $principal -Settings $settings -Description 'Window Shelf: start after interactive sign-in; no boot trigger and no Startup-folder entry.' -Force | Out-Null
$task = Get-ScheduledTask -TaskName $TaskName -TaskPath '\'
[pscustomobject]@{ TaskName=$task.TaskName; Delay=$task.Triggers[0].Delay; LogonType=$task.Principal.LogonType; RunLevel=$task.Principal.RunLevel; Execute=$task.Actions[0].Execute } | ConvertTo-Json
