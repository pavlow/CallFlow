param(
)
Get-CsPool | where { $_.Services -match "Registrar:" } | Select Identity