param(
[string]$identity,
[string]$domainController
)
Get-CsUser -Identity $identity -DomainController $domainController -ErrorAction:SilentlyContinue | Disable-CsUser -DomainController $domainController -ErrorAction:Stop