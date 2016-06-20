param(
)
Get-CsClientPolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity 