Param(
[string]$sipDomain
)

$ExistingDomain = Get-CsSipDomain -Identity $sipDomain -ErrorAction:SilentlyContinue;
if (!$ExistingDomain) {
	New-CsSipDomain -Identity $sipDomain;
}