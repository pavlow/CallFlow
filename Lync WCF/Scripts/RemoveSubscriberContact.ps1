param(
[string]$domainController,	
[string]$sipAddress		# eg: sip:ABC_AllStaff_Subscriber_Access@primary.local
)

$contact = Get-CsExUmContact -Identity $sipAddress -ErrorAction SilentlyContinue -DomainController $domainController;

if ($contact -ne $null) {
	Remove-CsExUmContact -Identity $contact.Identity.DistinguishedName -Confirm:$false;
}