param(
[string]$customerOuPath,             
[string]$username,  		  
[string]$primaryDomain    	  
)

#get the customer/tenant AD Organizational Unit:
$ouObject = Get-AdOrganizationalUnit -identity $customerOuPath -Properties msRTCSIP-GroupingID;
$guid = $ouObject.ObjectGUID;
$ouDN = $ouObject.DistinguishedName;

#find the user:
$userObject = Get-AdUser -LDAPFilter "(&(objectClass=user)(sAMAccountName=$Username))" -SearchBase $ouDN -Properties msRTCSIP-GroupingID -ErrorAction:Stop

#set the grouping ID for the user:
$userObject."msRTCSIP-GroupingId" = $null;
Set-ADUser -Instance $userObject;