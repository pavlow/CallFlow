param(
[string]$identity,
[string]$telephonyOption,
[string]$lineUri,
[string]$lineServerUri 
)

if ( $telephonyOption -eq "PcToPcOnly" )
{
    Set-CsUser -Identity $identity -AudioVideoDisabled $False -RemoteCallControlTelephonyEnabled $False -EnterpriseVoiceEnabled $False -LineURI $lineUri -DomainController $domainController -ErrorAction:Stop;
	return
}

if ( $telephonyOption -eq "RemoteCallControl" )
{
    Set-CsUser -Identity $identity -AudioVideoDisabled $False -RemoteCallControlTelephonyEnabled $True -EnterpriseVoiceEnabled $False -LineURI $lineUri -DomainController $domainController -ErrorAction:Stop;
	return
}

if ( $telephonyOption -eq "RemoteCallControlOnly" )
{
    Set-CsUser -Identity $identity -AudioVideoDisabled $True -RemoteCallControlTelephonyEnabled $True -EnterpriseVoiceEnabled $False -LineURI $lineUri -DomainController $domainController -ErrorAction:Stop;
	return
}

if ( $telephonyOption -eq "EnterpriseVoice" )
{
    Set-CsUser -Identity $identity -AudioVideoDisabled $False -RemoteCallControlTelephonyEnabled $False -EnterpriseVoiceEnabled $True -LineURI $LineURI -DomainController $domainController -ErrorAction:Stop;
	return
}

if ( $telephonyOption -eq "AudioVideoDisabled" )
{
    Set-CsUser -Identity $identity -AudioVideoDisabled $True -RemoteCallControlTelephonyEnabled $False -EnterpriseVoiceEnabled $False -DomainController $domainController -ErrorAction:Stop;
	return
}

# Invalid Option
{
    Throw "Invalid Telephony Option"
}