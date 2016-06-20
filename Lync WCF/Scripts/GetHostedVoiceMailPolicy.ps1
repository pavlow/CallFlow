param(
)
Get-CsHostedVoiceMailPolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity