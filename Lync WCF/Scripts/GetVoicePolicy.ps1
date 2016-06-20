param(
)
Get-CsVoicePolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity