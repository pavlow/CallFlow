param(
[string]$Identity,                
[string]$Name,                    
[string]$Description,             
[string]$ParticipationPolicy,  
[string]$AgentAlertTime,        
[string]$RoutingMethod,          
[string]$DistributionGroupAddress,
[string]$OwnerPool,               
[string]$AgentsByUri 

)

Import-Module SkypeforBusiness

$Group = Get-CsRgsAgentGroup | Where {$_.Name -eq $Name}

if ($Group)
{
	#Set

}
else
{
	#Create
}

$RGSGroup.Name = $RGSCreate.GroupName #Set call group name#
$RGSGroup.Description = $RGSCreate.GroupDescription #Set call group description#
$RGSGroup.AgentAlertTime = $RGSCreate.AgentAlertTime #Set agent alert time in seconds#
$RGSGroup.RoutingMethod = $RGSCreate.RoutingMethod #Set routing method - option: longest idle, parallel, round robin, serial, attendant#
$RGSGroup.ParticipationPolicy = $RGSCreate.ParticipationPolicy #Set participation policy - option: formal, informal#
#Set agent group type to use distributions group#
if($RGSCreate.RGSAgentGroupType -eq "DistributionGroup") 
{
    $RGSGroup.DistributionGroupAddress = $RGSCreate.DistributionGroupAddress 
    write-host "NOTE: Ensure distribution group exists "
}
#Set agent group type to use defined agents#
elseif($RGSCreate.RGSAgentGroupType -eq "ListedAgents") 
{ 
    $RGSGroup.AgentsByUri.Clear()
    $RGSGroup.AgentsByUri.Add("$($RGSCreate.AgentURI)")
}
else 
{ 
    write-host "Invalid choice for AgentGroupType. Please select either DistributionGroup or ListedAgents" 
}
#Saves agent group settings and writes message#
Set-CsRgsAgentGroup -Instance $RGSGroup 