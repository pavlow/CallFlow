param(
)
Get-CsArchivingPolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity 