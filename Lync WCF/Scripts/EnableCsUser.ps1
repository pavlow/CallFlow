param(
[string]$identity,
[string]$registrarPool,
[string]$sipAddress,
[string]$domainController
)

$user = Get-CsUser -Identity $identity -DomainController $domainController -ErrorAction:SilentlyContinue;

if (!$user) {
	Enable-CsUser -Identity $identity -RegistrarPool $registrarPool -SipAddress $sipAddress -DomainController $domainController -ErrorAction:Stop;
} else {
	if ($user.RegistrarPool.FriendlyName -ne $registrarPool) {
		$user | Move-CSUser -Target $registrarPool -DomainController $domainController -Confirm:$False -ErrorAction:Stop;
	}
	if ($sipAddress -ne $user.SipAddress) {
		$user | Set-CsUser -SipAddress $sipAddress -DomainController $domainController;
	}	
}
