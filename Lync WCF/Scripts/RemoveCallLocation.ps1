param (
	[string] $PstnUsageName,			# eg: ABC 649 PSTN Usage
	[string] $VoiceRouteName,			# eg: ABC 649 Route
	[string] $DialPlanIdentity,			# eg: ABC649.virtual-apps.net
	[string] $VoicePolicyName			# eg: ABC 649 Voice Policy
)

Get-CsDialPlan -Identity $DialPlanIdentity -ErrorAction SilentlyContinue | Remove-CsDialPlan -Confirm:$false;

Get-CsVoicePolicy -Identity $VoicePolicyName -ErrorAction SilentlyContinue | Remove-CsVoicePolicy -Confirm:$false;

Get-CsVoiceRoute -Identity $VoiceRouteName -ErrorAction SilentlyContinue | Remove-CsVoiceRoute -Confirm:$false;

$pstnUsage = (Get-CsPstnUsage -Identity "Global").Usage | where { $_ -eq $PstnUsageName };
if ($pstnUsage -ne $null) {
	Set-CsPstnUsage -Identity "Global" -Usage @{Remove=$PstnUsageName};
}