param(
[string]$identity,
[string]$dialplan,
[string]$voice,
[string]$conferencing,
[string]$domainController
)

if ($dialplan -eq "") {
	$dialplan = $null;
}
if ($voice -eq "") {
	$voice = $null;
}
if ($conferencing -eq "") {
	$conferencing = $null;
}

Grant-CsDialPlan -Identity $identity -PolicyName $dialplan -DomainController $domainController -ErrorAction:Stop;
Grant-CsVoicePolicy -Identity $identity -PolicyName $voice -DomainController $domainController -ErrorAction:Stop;
Grant-CsConferencingPolicy -Identity $identity -PolicyName $conferencing -DomainController $domainController -ErrorAction:Stop;