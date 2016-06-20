param (
	[string] $PstnUsageName,			# eg: ABC 649 PSTN Usage
	[string] $VoiceRouteName,			# eg: ABC 649 Route
	[string] $DialPlanName,				# eg: ABC649
	[string] $DialPlanIdentity,			# eg: ABC649.virtual-apps.net
	[string] $DialPlanDescription,		# eg: Dial plan for ABC649
	[string] $DialinConferencingRegion,	# eg: New Zealand Dial-in Numbers
	[string] $VoicePolicyName,			# eg: ABC 649 Voice Policy
	[string] $VoicePolicyDescription,	# eg: Voice Policy for ABC 649
	[string] $DialPlanArea,				# eg: 649
	[string] $PstnGateways				# eg: PstnGateway:lyncsip1.kordia.net.nz,PstnGateway:lyncsip2.kordia.net.nz
)

#Create the PSTN Usage Policy - eg: ABC 649 PSTN Usage
$pstnUsage = (Get-CsPstnUsage -Identity "Global").Usage | where { $_ -eq $PstnUsageName };
if ($pstnUsage -eq $null) {
	Set-CsPstnUsage -Identity "Global" -Usage @{Add=$PstnUsageName};
}

#Create the Voice Route - eg: ABC 649 Route
$voiceRoute = Get-CsVoiceRoute -Identity $VoiceRouteName -ErrorAction SilentlyContinue;
if ($voiceRoute -eq $null) {
	$PstnGatewayList = $PstnGateways.Trim().Split(",");
	New-CsVoiceRoute -Identity $VoiceRouteName -PstnGatewayList @{Add=$PstnGatewayList} -PstnUsages @{Add=$PstnUsageName} -NumberPattern .*;
}

#Create the Dial Plan
$dialPlan = Get-CsDialPlan -Identity $DialPlanIdentity -ErrorAction SilentlyContinue;
if ($dialPlan -eq $null) {
	New-CsDialPlan -Identity $DialPlanIdentity -SimpleName $DialPlanIdentity -Description $DialPlanDescription -DialinConferencingRegion $DialinConferencingRegion;
} else {
	$dialPlan | Set-CsDialPlan -Description $DialPlanDescription -DialinConferencingRegion $DialinConferencingRegion;
}

#Create New Voice Policy and add PSTN Usage Policy
$voicePolicy = Get-CsVoicePolicy -Identity $VoicePolicyName -ErrorAction SilentlyContinue;
if ($voicePolicy -eq $null) {
	New-CsVoicePolicy -Identity $VoicePolicyName -Description $VoicePolicyDescription -AllowCallForwarding $true -AllowPSTNReRouting $true -AllowSimulRing $true -EnableCallTransfer $true -EnableDelegation $true -EnableCallPark $false -EnableMaliciousCallTracing $false -EnableBWPolicyOverride $false -EnableTeamCall $true -PstnUsages @{Add=$PstnUsageName};	
}

#Set the Dial Plan and add the appropriate Normaliaztion Rules from the correct Dial Plan Area Template
$normalizationRules = Get-CsVoiceNormalizationRule -Filter _Template_$DialPlanArea/*;
Set-CsDialPlan -Identity $DialPlanIdentity -NormalizationRules @{Add=$normalizationRules} -ErrorAction SilentlyContinue;

#Remove the Keep All Normalization Rule from the Dial Plan
Get-CsVoiceNormalizationRule -Identity "$DialPlanIdentity/Keep All" -ErrorAction SilentlyContinue | Remove-CsVoiceNormalizationRule -Confirm:$false;