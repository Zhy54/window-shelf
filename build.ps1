param([string]$OutputDirectory = (Join-Path $PSScriptRoot 'bin'))
$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { throw '.NET Framework x64 compiler was not found.' }
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$src = Join-Path $PSScriptRoot 'src'
$compilerArgs = @('/nologo','/target:winexe','/platform:x64','/optimize+','/utf8output',
    '/reference:System.dll','/reference:System.Core.dll','/reference:System.Drawing.dll',
    '/reference:System.Windows.Forms.dll','/reference:System.Web.Extensions.dll',
    ('/win32manifest:' + (Join-Path $src 'app.manifest')),
    ('/out:' + (Join-Path $OutputDirectory 'WindowShelf.exe')),
    (Join-Path $src 'WindowShelf.cs'), (Join-Path $src 'AppSettings.cs'))
& $compiler @compilerArgs
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
Write-Output (Join-Path $OutputDirectory 'WindowShelf.exe')
