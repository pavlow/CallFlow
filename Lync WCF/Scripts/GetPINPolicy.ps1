param(
)
Get-CsPinPolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity