Param(
[string]$sipDomain
)

Remove-CsSipDomain -Identity $SipDomain -ErrorAction:SilentlyContinue;