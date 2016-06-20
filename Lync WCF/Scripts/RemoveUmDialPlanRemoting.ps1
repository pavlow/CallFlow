param(
[string]$domainController,	
[string]$dialPlanName		#eg : ABC_AllStaff
)

$UMServer = Get-UMServer -DomainController $domainController;
if ($UMServer -is [Array]) {
	$UMServer = $UMServer[0];
}

# Remove the hunt group
$huntGroup = Get-UMHuntGroup -UMDialPlan $dialPlanName -DomainController $domainController -ErrorAction SilentlyContinue;
if ($huntGroup -ne $null) {
	Remove-UMHuntGroup -Identity $huntGroup.DistinguishedName -Confirm:$false -DomainController $domainController;
}

# Unassign the Dial Plan from the UM server
Set-UMServer -Identity $UMServer.Identity -DialPlans @{remove=$dialPlanName} -DomainController $domainController -ErrorAction SilentlyContinue;

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