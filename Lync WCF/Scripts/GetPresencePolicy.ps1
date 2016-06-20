param(
)
Get-CsPresencePolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity