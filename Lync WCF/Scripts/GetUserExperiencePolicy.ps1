param(
)
Get-CsUserExperiencePolicy -ErrorAction:Stop | Where { $_.Identity.StartsWith("Tag:") } | Select Identity