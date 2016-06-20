param(
)
Get-CsDialPlan -ErrorAction:Stop | Where { $_.Identity.StartsWith("Tag:") }