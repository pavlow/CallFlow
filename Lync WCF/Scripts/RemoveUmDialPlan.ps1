param(
[string]$connectionUri,
[string]$domainController,	
[string]$dialPlanName		#eg : ABC_AllStaff
)

$session = New-PSSession -configurationname Microsoft.Exchange -connectionURI $connectionUri;
Import-PSSession $session -AllowClobber;

$UMServer = (Get-UMServer -DomainController $domainController) | Select -First 1;

# Remove the hunt group
$huntGroup = Get-UMHuntGroup -DomainController $domainController | where { $_.Name -eq $dialPlanName };
if ($huntGroup -ne $null) {
	Remove-UMHuntGroup -Identity $huntGroup.DistinguishedName -Confirm:$false -DomainController $domainController;
}

# Unassign the Dial Plan from the UM server
if (($UMServer.DialPlans | where { $_ -eq $dialPlanName }) -ne $null) {
	Set-UMServer -Identity $UMServer.Identity -DialPlans @{remove=$dialPlanName} -DomainController $domainController;
}

# Remove the mailbox policy
$mailboxPolicyName = $dialPlanName + " Default Policy";
$mailboxPolicy = Get-UMMailboxPolicy $mailboxPolicyName -ErrorAction SilentlyContinue -DomainController $domainController;
if ($mailboxPolicy -ne $null) {
	 $mailboxPolicy | Remove-UMMailboxPolicy -Confirm:$false -DomainController $domainController;
}

# Remove the dial plan
$dialPlan = Get-UMDialPlan $dialPlanName -ErrorAction SilentlyContinue -DomainController $domainController;
if ($dialPlan -ne $null) {	
    $dialPlan | Remove-UMDialPlan -Confirm:$false -DomainController $domainController;
}

Remove-PSSession $session;