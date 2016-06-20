param(
$groupsObj
)

<#
cls
$AgentsObj = @()
$obj1 = New-Object PSObject
$obj1 | Add-Member -MemberType NoteProperty -Name "AgentSipAddress" -Value "sip:u1@ucgeek.nz"
$AgentsObj += $obj1

$obj1 = New-Object PSObject
$obj1 | Add-Member -MemberType NoteProperty -Name "AgentSipAddress" -Value "sip:u2@ucgeek.nz"
$AgentsObj += $obj1

$groupsObj = New-Object PSObject
$groupsObj | Add-Member -MemberType NoteProperty -Name "Agents" -Value $AgentsObj
$groupsObj | Add-Member -MemberType NoteProperty -Name "Description" -Value "This is a test description!"
$groupsObj | Add-Member -MemberType NoteProperty -Name "DistributionGroup" -Value "disty@ucgeek.nz"
#$groupsObj | Add-Member -MemberType NoteProperty -Name "Identity" -Value "identity"
$groupsObj | Add-Member -MemberType NoteProperty -Name "IsGroupAgents" -Value $false
$groupsObj | Add-Member -MemberType NoteProperty -Name "IsDistributionGroup" -Value $true
$groupsObj | Add-Member -MemberType NoteProperty -Name "IsGroupAgentSignIn" -Value $false
$groupsObj | Add-Member -MemberType NoteProperty -Name "OwnerPool" -Value "S4BFE01.ucgeek.nz"
$groupsObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "New 3"
$groupsObj | Add-Member -MemberType NoteProperty -Name "ParticipationPolicy" -Value "Formal"
$groupsObj | Add-Member -MemberType NoteProperty -Name "RoutingMethod" -Value "Serial"
$groupsObj | Add-Member -MemberType NoteProperty -Name "AgentAlertTime" -Value 30
#>
#Start-Transcript -Path "C:\Logs\Log_SetCsRgsAgentGroup.log"

#Get existing group if it exisits
$Group = Get-CsRgsAgentGroup | Where {$_.Name -eq $groupsObj.Name}

#If exisits set:
if ($Group)
{
    $Group.Name = $groupsObj.Name #Set call group name#
    $Group.Description = $groupsObj.Description #Set call group description#
    $Group.AgentAlertTime = $groupsObj.AgentAlertTime #Set agent alert time in seconds#
    $Group.RoutingMethod = $groupsObj.RoutingMethod #Set routing method - option: longest idle, parallel, round robin, serial, attendant#
    
    #Set participation policy - option: formal, informal#
    if($groupsObj.IsGroupAgentSignIn)
    {
        $Group.ParticipationPolicy = "formal"
    }
    else
    {
        $Group.ParticipationPolicy = "informal"
    }
    
    #Set agent group type to use distributions group
    if($groupsObj.IsDistributionGroup) 
    {
        $Group.DistributionGroupAddress = $groupsObj.DistributionGroup 
    }
    
    #Set agent group type to use defined agents
    if($groupsObj.IsGroupAgents)
    { 
        $Group.AgentsByUri.Clear()

        $agentArray = @()
        foreach ($Agent in $groupsObj.Agents.AgentSipAddress)
        {     
            $Group.AgentsByUri.Add($Agent)
        }
    }
    Set-CsRgsAgentGroup -Instance $Group

}
#If not exists create new
else
{
    #Write-Host "New"
    $Group = New-CsRgsAgentGroup -Parent $groupsObj.OwnerPool `
    -Name $groupsObj.Name `
    -Description $groupsObj.Description `
    -AgentAlertTime $groupsObj.AgentAlertTime `
    -RoutingMethod $groupsObj.RoutingMethod `
    -InMemory -Verbose
 
    #Set participation policy - option: formal, informal#
    if($groupsObj.IsGroupAgentSignIn)
    {
        $Group.ParticipationPolicy = "formal"
    }
    else
    {
        $Group.ParticipationPolicy = "informal"
    }
    
    #Set agent group type to use distributions group
    if($groupsObj.IsDistributionGroup) 
    {
        #Write-Host "Disty"
        $Group.DistributionGroupAddress = $groupsObj.DistributionGroup 
    }
    
    #Set agent group type to use defined agents
    if($groupsObj.IsGroupAgents)
    { 
        #Write-Host "Groups"
        $Group.AgentsByUri.Clear()

        $agentArray = @()
        foreach ($Agent in $groupsObj.Agents.AgentSipAddress)
        {     
            $Group.AgentsByUri.Add($Agent)
        }
    }

    Set-CsRgsAgentGroup -Instance $Group -Verbose
}

#Get-CsRgsAgentGroup -Name $groupsObj.Name
#Get-CsRgsAgentGroup -Name "New 3"
#Stop-Transcript
#Get-CsRgsAgentGroup | where {$_.Name -like "New*"} | Remove-CsRgsAgentGroup