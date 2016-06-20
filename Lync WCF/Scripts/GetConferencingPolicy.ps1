param(
)
Get-CsConferencingPolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity