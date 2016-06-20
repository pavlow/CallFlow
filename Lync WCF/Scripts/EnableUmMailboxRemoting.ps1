param(
[string]$domainController,	
[string]$identity,
[string]$mailboxPolicy,
[string]$extensions,
[string]$pin,
[string]$sipAddress
)

$mailbox = Get-Mailbox -Identity $identity -DomainController $domainController;

if (!$mailbox.UMEnabled) {
	Enable-UMMailbox -Identity $identity -UMMailboxPolicy $mailboxPolicy -Extensions $extensions -PIN $pin -SIPResourceIdentifier $sipAddress -PINExpired $true -DomainController $domainController;
} else {
	Set-UMMailbox -Identity $identity -UMMailboxPolicy $mailboxPolicy -DomainController $domainController;
}