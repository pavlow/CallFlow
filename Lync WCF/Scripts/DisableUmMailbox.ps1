param(
[string]$connectionUri,
[string]$domainController,	
[string]$identity
)

$session = New-PSSession -configurationname Microsoft.Exchange -connectionURI $connectionUri;
Import-PSSession $session -AllowClobber;

Get-UMMailbox -Identity $identity -ErrorAction silentlycontinue -DomainController $domainController | Disable-UMMailbox -DomainController $domainController -confirm:$false;

Remove-PSSession $session;