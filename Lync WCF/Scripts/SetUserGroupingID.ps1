param(
[string]$customerOUPath,        
[string]$userName,     
[string]$primaryDomain
)

#get the customer/tenant AD Organizational Unit:
$ouObject = Get-AdOrganizationalUnit -identity $customerOUPath -Properties msRTCSIP-GroupingId -ErrorAction:Stop
$guid = $ouObject.ObjectGUID
$ouDN = $ouObject.DistinguishedName

#find the user:
$userObject = Get-AdUser -LDAPFilter "(&(objectClass=user)(sAMAccountName=$Username))" -SearchBase $ouDN -Properties msRTCSIP-GroupingID -ErrorAction:Stop

if($userObject -eq $null) {
	#Find the user again without specifying the base path
	$userObject = Get-AdUser -LDAPFilter "(&(objectClass=user)(sAMAccountName=$Username))" -Properties msRTCSIP-GroupingID -ErrorAction:Stop
}

if($userObject -eq $null)
{
	$exceptionText = "Unable to locate user $Username in customer OU $customerOUPath.  If this user was recently moved, reprovision the user to relocate it the correct customer OU."
	throw $exceptionText
}

#set the grouping ID for the user:
$userObject | Set-ADUser -Replace @{'msRTCSIP-GroupingId'=$guid};