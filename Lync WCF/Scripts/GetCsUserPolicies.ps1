#Arrays
$UserPolicies = @()

#VoicePolicy
$VoicePolicy = Get-CsVoicePolicy
#$UserPolicies = @()
if($VoicePolicy -ne $null)
{
    foreach($item in $VoicePolicy)
    {   
        if($item.Identity -like "Tag:*") #-or $item.Identity -eq "Global"
        {
                          
            #if($item.Identity -like "Tag:*")
            #{
                $myObject1 = New-Object System.Object
                $myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "VoicePolicy" 
                $myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]
            #}

            #if($item.Identity -eq "Global")
            #{
            #    $myObject1 | Add-Member -type NoteProperty -name "Name" -Value "Global"
            #}

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#VoiceRoutingPolicy
$VoiceRoutingPolicy = Get-CsVoiceRoutingPolicy
#$UserPolicies = @()
if($VoiceRoutingPolicy -ne $null)
{
    foreach($item in $VoiceRoutingPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
                               
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "VoiceRoutingPolicy" 
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#ConferencingPolicy
$ConferencingPolicy = Get-CsConferencingPolicy
#$UserPolicies = @()
if($ConferencingPolicy -ne $null)
{
    foreach($item in $ConferencingPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
                                
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "ConferencingPolicy"
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#PresencePolicy
$PresencePolicy = Get-CsPresencePolicy
#$UserPolicies = @()
if($PresencePolicy -ne $null)
{
    foreach($item in $PresencePolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
                  
			$myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "PresencePolicy"  
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#DialPlan
$DialPlan = Get-CsDialPlan
#$UserPolicies = @()
if($DialPlan -ne $null)
{
    foreach($item in $DialPlan)
    {   
        if($item.Identity -like "Tag:*")
        {
                                
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "DialPlan"
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#LocationPolicy
$LocationPolicy = Get-CsLocationPolicy
#$UserPolicies = @()
if($LocationPolicy -ne $null)
{
    foreach($item in $LocationPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
                               
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "LocationPolicy" 
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#ClientPolicy
$ClientPolicy = Get-CsClientPolicy
#$UserPolicies = @()
if($ClientPolicy -ne $null)
{
    foreach($item in $ClientPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "ClientPolicy"  
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#ClientVersionPolicy
$ClientVersionPolicy = Get-CsClientVersionPolicy
#$UserPolicies = @()
if($ClientVersionPolicy -ne $null)
{
    foreach($item in $ClientVersionPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "ClientVersionPolicy"
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1] 

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies

#ArchivingPolicy
$ArchivingPolicy = Get-CsArchivingPolicy
#$UserPolicies = @()
if($ArchivingPolicy -ne $null)
{
    foreach($item in $ArchivingPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {            
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "ArchivingPolicy"
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#ExchangeArchivingPolicy
#This is a fixed list


#PinPolicy
$PinPolicy = Get-CsPinPolicy
#$UserPolicies = @()
if($PinPolicy -ne $null)
{
    foreach($item in $PinPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "PinPolicy" 
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#ExternalAccessPolicy
$ExternalAccessPolicy = Get-CsExternalAccessPolicy
#$UserPolicies = @()
if($ExternalAccessPolicy -ne $null)
{
    foreach($item in $ExternalAccessPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "ExternalAccessPolicy" 
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#MobilityPolicy
$MobilityPolicy = Get-CsMobilityPolicy
#$UserPolicies = @()
if($MobilityPolicy -ne $null)
{
    foreach($item in $MobilityPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "MobilityPolicy"  
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#PersistentChatPolicy
$PersistentChatPolicy = Get-CsPersistentChatPolicy
#$UserPolicies = @()
if($PersistentChatPolicy -ne $null)
{
    foreach($item in $PersistentChatPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "PersistentChatPolicy"  
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#UserServicesPolicy
$UserServicesPolicy = Get-CsUserServicesPolicy
#$UserPolicies = @()
if($UserServicesPolicy -ne $null)
{
    foreach($item in $UserServicesPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "UserServicesPolicy"  
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#CallViaWorkPolicy
$CallViaWorkPolicy = Get-CsCallViaWorkPolicy
#$UserPolicies = @()
if($CallViaWorkPolicy -ne $null)
{
    foreach($item in $CallViaWorkPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
			$myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "CallViaWorkPolicy"
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#HostedVoicemailPolicy
$HostedVoicemailPolicy = Get-CsHostedVoicemailPolicy
#$UserPolicies = @()
if($HostedVoicemailPolicy -ne $null)
{
    foreach($item in $HostedVoicemailPolicy)
    {   
        if($item.Identity -like "Tag:*")
        {
            $myObject1 = New-Object System.Object
            $myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "HostedVoicemailPolicy"
			$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity.Split(":")[1]

            $UserPolicies += $myObject1
        }
    }
}
#$UserPolicies


#RegistrarPool
$RegistrarPool = Get-CsPool | Where {$_.Services -like "Registrar:*"}
#$UserPolicies = @()
if($RegistrarPool -ne $null)
{
    foreach($item in $RegistrarPool)
    {   
            $myObject1 = New-Object System.Object
            $myObject1 | Add-Member -type NoteProperty -name "PolicyType" -Value "RegistrarPool"
            $myObject1 | Add-Member -type NoteProperty -name "Name" -Value $item.Identity
            $UserPolicies += $myObject1
    }
}
#$UserPolicies

return $UserPolicies