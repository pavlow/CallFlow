param(
[string]$domainController,	
[string]$name,				# eg: ABC_AllStaff UM Subscriber Access"
[string]$sipAddress,		# eg: sip:ABC_AllStaff_Subscriber_Access@primary.local
[string]$registrarPool,		# eg: lync2013-pool001.virtual-apps.net
[string]$tenantOu,			# eg: OU=abc.co.nz,OU=Hosting,DC=virtual-apps,DC=net
[string]$accessNumber,		# eg: +6491234567
[string]$description,		# eg: Exchange UM Subscriber Access
[string]$ipPhone			# eg: sip:ABC_AllStaff_Subscriber_Access@primary.local;opaque=app:exum:ABC_AllStaff.virtual-apps.net
)

$contact = Get-CsExUmContact -Identity $sipAddress -ErrorAction SilentlyContinue -DomainController $domainController;

if ($contact -eq $null) {
	$contact = New-CsExUmContact -SipAddress $sipAddress -RegistrarPool $registrarPool -OU $tenantOu -DisplayNumber $accessNumber -Description $description;
} else {
	$contact | Set-CsExUmContact -DisplayNumber $accessNumber -Description $description -DomainController $domainController;
}

#get the contact from AD, remove the 'otherIpPhone' attribute where the assignment to the correct subscriber dial plan occurs
#and rebuild it with a value that goes to this new dial plan, not the default one
$ldapFilter = "(CN=" + $contact.Name + ")"
$contactAdObject = Get-ADObject -LDAPFilter $ldapFilter -SearchBase $tenantOu

if ($contact.OtherIpPhone -ne $ipPhone) {
	$otherPhoneRemove = $contact.OtherIpPhone;
	Set-ADObject $contactAdObject -Remove @{otherIpPhone="$otherPhoneRemove"};
	Set-ADObject $contactAdObject -Add @{otherIpPhone="$ipPhone"};
}

if ($contact.DisplayName -ne $name) {
	Set-ADObject $contactADobject -DisplayName $name;
	Rename-ADObject $contactADobject -NewName $name;
}