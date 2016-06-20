param(
)
Get-CsClientVersionPolicy -ErrorAction:Stop| Where { $_.Identity.StartsWith("Tag:") } | Select Identity