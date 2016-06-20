param(
[string]$domainController,	
[string]$identity,
[string]$pin
)

Set-UMMailboxPIN -Identity $identity -Pin $pin -PinExpired $false -DomainController $domainController;