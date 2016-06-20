param(
[string]$connectionUri,
[string]$domainController,	
[string]$identity,
[string]$pin
)

$session = New-PSSession -configurationname Microsoft.Exchange -connectionURI $connectionUri;
Import-PSSession $session -AllowClobber;

Set-UMMailboxPIN -Identity $identity -Pin $pin -PinExpired $false -DomainController $domainController;

Remove-PSSession $session;