param(
[string]$connectionUri,
[string]$domainController,	
[string]$identity,
[string]$mailboxPolicy,
[string]$extensions,
[string]$pin,
[string]$sipAddress
)

$session = New-PSSession -configurationname Microsoft.Exchange -connectionURI $connectionUri;
Import-PSSession $session -AllowClobber;

Enable-UMMailbox -Identity $identity -UMMailboxPolicy $mailboxPolicy -Extensions $extensions -PIN $pin -SIPResourceIdentifier $sipAddress -PINExpired $true -DomainController $domainController;

Remove-PSSession $session;