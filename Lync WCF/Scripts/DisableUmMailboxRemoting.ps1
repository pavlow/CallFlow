param(
[string]$domainController,	
[string]$identity
)

Get-UMMailbox -Identity $identity -ErrorAction silentlycontinue -DomainController $domainController | Disable-UMMailbox -DomainController $domainController -confirm:$false;