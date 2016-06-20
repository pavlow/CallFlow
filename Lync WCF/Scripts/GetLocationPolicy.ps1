param(
)
Get-CsLocationPolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity