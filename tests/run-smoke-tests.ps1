param(
    [string]$BaseUrl = "http://127.0.0.1:5187",
    [int]$StartupTimeoutSeconds = 60
)

$ErrorActionPreference = "Stop"

dotnet run `
    --project "tests/StudyNotesPlatform.SmokeTests/StudyNotesPlatform.SmokeTests.csproj" `
    -- `
    --base-url $BaseUrl `
    --startup-timeout-seconds $StartupTimeoutSeconds
