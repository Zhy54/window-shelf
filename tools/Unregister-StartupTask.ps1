param([string]$TaskName = 'WindowShelf')
$ErrorActionPreference = 'Stop'
$task = Get-ScheduledTask -TaskName $TaskName -TaskPath '\' -ErrorAction SilentlyContinue
if (-not $task) { Write-Output 'Task not found.'; exit 0 }
if ($task.Description -notlike 'Window Shelf:*') { throw 'The task is not identified as a Window Shelf task.' }
Unregister-ScheduledTask -TaskName $TaskName -TaskPath '\' -Confirm:$false
Write-Output 'Window Shelf scheduled task removed. Running windows and settings were not changed.'
