param(
[string]$domainController,	
[string]$dialPlanName,		#eg : ABC_AllStaff
[string]$accessNumber,		#eg : +641234567
[string]$regionCode			#eg : 649
)

$UMServer = Get-UMServer -DomainController $domainController;
if ($UMServer -is [Array]) {
	$UMServer = $UMServer[0];
}

# Create the dial plan
$dialPlan = Get-UMDialPlan $dialPlanName -ErrorAction SilentlyContinue -DomainController $domainController;
if ($dialPlan -eq $null)
{
    $dialPlan = New-UMDialPlan -name $dialPlanName -UriType "SipName" -VoipSecurity Secured -NumberOfDigitsInExtension 8 -CountryOrRegionCode $regionCode -DomainController $domainController;
}
Set-UMDialPlan -Identity $dialPlanName -VoipSecurity Secured -AccessTelephoneNumbers $accessNumber -CountryOrRegionCode $regionCode -ConfiguredInCountryOrRegionGroups "*,*,*" -ConfiguredInternationalGroups "*,*,*" -AllowedInCountryOrRegionGroups "*" -AllowedInternationalGroups "*" -DomainController $domainController;

# Assign the Dial Plan to the UM server (run it twice because the server is not always assigned the first time...)
Set-UMServer -Identity $UMServer.Identity.DistinguishedName -DialPlans @{add=$dialPlanName} -DomainController $domainController;
Set-UMServer -Identity $UMServer.Identity.DistinguishedName -DialPlans @{add=$dialPlanName} -DomainController $domainController;

# Create the hunt group
$huntGroup = Get-UMHuntGroup -UMDialPlan $dialPlanName -DomainController $domainController -ErrorAction SilentlyContinue;
if ($huntGroup -eq $null) {
	New-UMHuntGroup -Name $dialPlanName -PilotIdentifier $dialPlanName -UMDialPlan $dialPlanName -UMIPGateway "1:20" -DomainController $domainController;
}