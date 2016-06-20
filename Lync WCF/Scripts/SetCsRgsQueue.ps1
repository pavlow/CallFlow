param(
$queuesObj
)
$VerbosePreference = "continue"

<#
cls
$agentsObj = @()
$obj1 = New-Object PSObject
$obj1 | Add-Member -MemberType NoteProperty -Name "QueueGroups" -Value "FE01 - Group 1"
$agentsObj += $obj1

$obj1 = New-Object PSObject
$obj1 | Add-Member -MemberType NoteProperty -Name "QueueGroups" -Value "FE01 - Group 2"
$agentsObj += $obj1

$queuesObj = New-Object PSObject
$queuesObj | Add-Member -MemberType NoteProperty -Name "AgentGroupIDList" -Value $AgentsObj
$queuesObj | Add-Member -MemberType NoteProperty -Name "Description" -Value "This is a test description!"
#$queuesObj | Add-Member -MemberType NoteProperty -Name "Identity" -Value "identity"
$queuesObj | Add-Member -MemberType NoteProperty -Name "OwnerPool" -Value "CFM-S4BFE01.callflowmanager.nz"
$queuesObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "New 6"

$queuesObj | Add-Member -MemberType NoteProperty -Name "TimeoutOn" -Value $true
$queuesObj | Add-Member -MemberType NoteProperty -Name "TimeoutThreshold" -Value "20" #named TimeoutThreshold in C#
$queuesObj | Add-Member -MemberType NoteProperty -Name "TimeoutUri" -Value "jim.smith"
$queuesObj | Add-Member -MemberType NoteProperty -Name "TimeoutSipDomain" -Value "ucgeek.nz"
$queuesObj | Add-Member -MemberType NoteProperty -Name "TimeoutQueue" -Value "XXXXX"
$queuesObj | Add-Member -MemberType NoteProperty -Name "TimeoutDestination" -Value "TransferToUri"
#$queuesObj | Add-Member -MemberType NoteProperty -Name "TimeoutAction" -Value "????" #Build this out of above

$queuesObj | Add-Member -MemberType NoteProperty -Name "OverFlowOn" -Value $true
$queuesObj | Add-Member -MemberType NoteProperty -Name "OverflowThreshold" -Value "0" #named Overflow in C#
$queuesObj | Add-Member -MemberType NoteProperty -Name "OverFlowUri" -Value "bob.builder"
$queuesObj | Add-Member -MemberType NoteProperty -Name "OverFlowSipDomain" -Value "ucgeek.nz"
$queuesObj | Add-Member -MemberType NoteProperty -Name "OverflowQueue" -Value "8b72a533-9463-403f-aee1-9abc2fe2cfac"
$queuesObj | Add-Member -MemberType NoteProperty -Name "OverFlowDestination" -Value "TransferToVoicemailUri" 
$queuesObj | Add-Member -MemberType NoteProperty -Name "OverflowCandidate" -Value "Newest" #Need to add this in GUI
#$queuesObj | Add-Member -MemberType NoteProperty -Name "OverflowAction" -Value "????" #Build this out of above
#>

#Start-Transcript -Path "C:\Logs\Log_SetCsRgsQueue.log"

#Build TimeoutUri
$timeoutUri =$null
if($queuesObj.TimeoutUri -ne "" -and $queuesObj.TimeoutSipDomain -ne "")
{            
	if ($queuesObj.TimeoutUri -like "sip:*")
    {
        $timeoutUri = $queuesObj.TimeoutUri + "@" + $queuesObj.TimeoutSipDomain 
    }
    else
    {
        $timeoutUri = "sip:" + $queuesObj.TimeoutUri + "@" + $queuesObj.TimeoutSipDomain
    }
	
	Write-Verbose "Built Timeout Uri - $timeoutUri"
}

#Build OverFlowUri
$overFlowUri =$null
if($queuesObj.OverFlowUri -ne "" -and $queuesObj.OverFlowSipDomain -ne "")
{            
	if ($queuesObj.OverFlowUri -like "sip:*")
    {
        $overFlowUri = $queuesObj.OverFlowUri + "@" + $queuesObj.OverFlowSipDomain 
    }
    else
    {
        $overFlowUri = "sip:" + $queuesObj.OverFlowUri + "@" + $queuesObj.OverFlowSipDomain
    }

	Write-Verbose "Built Overflow Uri - $overFlowUri"
}

#Build Queue full Id
Function QueueId ($OwnerPool, $QueueInstanceId)
{
    if ($OwnerPool -and $QueueInstanceId)
    {
        $QueueId = "service:ApplicationServer:"+ $OwnerPool + "/" + $QueueInstanceId
        return $QueueId
        Write-Verbose "Built QueueId - $QueueId"
    }
    else
    {
        Write-Verbose "QueueId is null"
        return $null
    }
}

#Create call action
Write-Verbose "Creating Timeout destination"
switch ($queuesObj.TimeoutDestination) 
{ 
    "Terminate" {$timeoutCallAction = New-CsRgsCallAction -Action Terminate}
    "TransferToQuestion" {"Not implemented yet"}
    "TransferToVoicemailUri" {$timeoutCallAction = New-CsRgsCallAction -Action TransferToVoicemailUri -Uri $timeoutUri}
    "TransferToUri" {$timeoutCallAction = New-CsRgsCallAction -Action TransferToUri -Uri $timeoutUri}
    "TransferToQueue" {$timeoutCallAction = New-CsRgsCallAction -Action TransferToQueue -QueueID (QueueId $queuesObj.OwnerPool $queuesObj.TimeoutQueue.Id)} #(Get-CsRgsQueue -Name $queuesObj.TimeoutQueue)}
    "TransferToPstn" {$timeoutCallAction = New-CsRgsCallAction -Action TransferToPstn -Uri $timeoutUri} 
    default {$timeoutCallAction = $null}

    #$csRgsCallAction = New-CsRgsCallAction -Prompt "" -Question "" -Action Terminate -QueueID "" -Uri ""
}

Write-Verbose "Creating Overflow destination"
switch ($queuesObj.OverFlowDestination) 
{ 
    "Terminate" {$overflowCallAction = New-CsRgsCallAction -Action Terminate}
    "TransferToQuestion" {"Not implemented yet"}
    "TransferToVoicemailUri" {$overflowCallAction = New-CsRgsCallAction -Action TransferToVoicemailUri -Uri $overFlowUri}
    "TransferToUri" {$overflowCallAction = New-CsRgsCallAction -Action TransferToUri -Uri $overFlowUri}
    "TransferToQueue" {$overflowCallAction = New-CsRgsCallAction -Action TransferToQueue -QueueID (QueueId $queuesObj.OwnerPool $queuesObj.OverflowQueue.Id)}
    "TransferToPstn" {$overflowCallAction = New-CsRgsCallAction -Action TransferToPstn -Uri $overFlowUri} 
    default {$overflowCallAction = $null}

    #$csRgsCallAction = New-CsRgsCallAction -Prompt "" -Question "" -Action Terminate -QueueID "" -Uri ""
}


#Get existing group if it exisits
Write-Verbose "Checking existance of Queue - $Queue"
$Queue = Get-CsRgsQueue | Where {$_.Name -eq $queuesObj.Name}

#If exisits set:
if ($Queue)
{
    Write-Verbose "Queue already exists, editing..."
    #Write-Host "Edit"
    $Queue.Name = $queuesObj.Name #Set queue name#
    $Queue.Description = $queuesObj.Description #Set queue description#  
    
    #Set timeout on
    Write-Verbose "Queue timeout - "
    Write-Verbose "queuesObj_TimeoutOn: $($queuesObj.TimeoutOn)"
    Write-Verbose "timeoutCallAction: $timeoutCallAction"
    Write-Verbose "queuesObj_TimeoutThreshold: $($queuesObj.TimeoutThreshold)"

    if($queuesObj.TimeoutOn -and $timeoutCallAction -ne $null -and $queuesObj.TimeoutThreshold -ne "")
    {
        #$Queue
        Write-Verbose "Setting Queue timeout values - "
        Write-Verbose "queuesObj_TimeoutOn: $($queuesObj.TimeoutOn)"
        Write-Verbose "timeoutCallAction: $timeoutCallAction"
        Write-Verbose "queuesObj_TimeoutThreshold: $($queuesObj.TimeoutThreshold)"
        $Queue.TimeoutThreshold = $queuesObj.TimeoutThreshold
        $Queue.TimeoutAction = $timeoutCallAction #Need to create a action in C#
    }
	elseif (!$queuesObj.TimeoutOn)
	{
		Write-Verbose "Queue timeout disabled"
        $Queue.TimeoutThreshold = $null
	}
    else
    {
        Write-Warning "Queue Timeout not updated" 
    }
    
    #Set overflow on
    Write-Verbose "Queue Overflow values - "
    Write-Verbose "queuesObj_OverFlowOn: $($queuesObj.OverFlowOn)"
    Write-Verbose "overflowCallAction: $overflowCallAction"
    Write-Verbose "queuesObj_OverflowThreshold: $($queuesObj.OverflowThreshold)"
    Write-Verbose "queuesObj_OverflowCandidate: $($queuesObj.OverflowCandidate)"

    #if($queuesObj.OverFlowOn -and $overflowCallAction -ne $null -and $queuesObj.OverflowThreshold -ne "" -and $queuesObj.OverflowCandidate -ne "")
	if($queuesObj.OverFlowOn -and $overflowCallAction -ne $null -and $queuesObj.OverflowThreshold -ge 0 -and $queuesObj.OverflowCandidate)
    { 
        Write-Verbose "Setting Queue Overflow - "
        Write-Verbose "queuesObj_OverFlowOn: $($queuesObj.OverFlowOn)"
        Write-Verbose "overflowCallAction: $overflowCallAction"
        Write-Verbose "queuesObj_OverflowThreshold: $($queuesObj.OverflowThreshold)"
        Write-Verbose "queuesObj_OverflowCandidate: $($queuesObj.OverflowCandidate)"

        $Queue.OverflowThreshold = $queuesObj.OverflowThreshold          
        $Queue.OverflowCandidate = $queuesObj.OverflowCandidate #needs to be added to form
        $Queue.OverflowAction = $overflowCallAction #Need to create a action in C#
    }
	elseif (!$queuesObj.OverFlowOn)
	{
		Write-Verbose "Queue Overflow disabled"
        $Queue.OverflowThreshold = $null
	}
    else
    {
        Write-Warning "Queue Overflow not updated" 
    }

    #Set queue group 
    #Write-Host "Queues"
    Write-Verbose "Preparing to add agents to Group"
    $Queue.AgentGroupIDList.Clear()

    foreach ($group in $queuesObj.AgentGroupIDList.QueueGroups)
    {     
        Write-Verbose "Adding Group $group to Queue"
        $Queue.AgentGroupIDList.Add((Get-CsRgsAgentGroup -Name $group).Identity)
    }

    Write-Verbose "Commiting changes to Queue"
    Set-CsRgsQueue -Instance $Queue -Verbose


}
#If not exists create new
else
{
    #Write-Host "New"
    Write-Verbose "Creating new Queue"
    $Queue = New-CsRgsQueue -Parent $queuesObj.OwnerPool `
    -Name $queuesObj.Name `
    -Description $queuesObj.Description `
    -InMemory -Verbose
    
    #Set timeout on
    Write-Verbose "Queue timeout values - "
    Write-Verbose "queuesObj_TimeoutOn: $($queuesObj.TimeoutOn)"
    Write-Verbose "timeoutCallAction: $timeoutCallAction"
    Write-Verbose "queuesObj_TimeoutThreshold: $($queuesObj.TimeoutThreshold)"

    if($queuesObj.TimeoutOn -and $timeoutCallAction -ne $null -and $queuesObj.TimeoutThreshold -ne "")
    {
        #$Queue
        Write-Verbose "Setting Queue timeout - "
        Write-Verbose "queuesObj_TimeoutOn: $($queuesObj.TimeoutOn)"
        Write-Verbose "timeoutCallAction: $timeoutCallAction"
        Write-Verbose "queuesObj_TimeoutThreshold: $($queuesObj.TimeoutThreshold)"

        $Queue.TimeoutThreshold = $queuesObj.TimeoutThreshold
        $Queue.TimeoutAction = $timeoutCallAction #Need to create a action in C#
    }
	elseif (!$queuesObj.TimeoutOn)
	{
        Write-Verbose "Queue timeout disabled"		
        $Queue.TimeoutThreshold = $null
	}
    else
    {
        Write-Warning "Queue Timeout not updated" 
    }
    
    #Set overflow on
    Write-Verbose "Queue Overflow values - "
    Write-Verbose "queuesObj_OverFlowOn: $($queuesObj.OverFlowOn)"
    Write-Verbose "overflowCallAction: $overflowCallAction"
    Write-Verbose "queuesObj_OverflowThreshold: $($queuesObj.OverflowThreshold)"
    Write-Verbose "queuesObj_OverflowCandidate: $($queuesObj.OverflowCandidate)"

    #if($queuesObj.OverFlowOn -and $overflowCallAction -ne $null -and $queuesObj.OverflowThreshold -ne "" -and $queuesObj.OverflowCandidate -ne "")
	if($queuesObj.OverFlowOn -and $overflowCallAction -ne $null -and $queuesObj.OverflowThreshold -ge 0 -and $queuesObj.OverflowCandidate)
    { 
        Write-Verbose "Setting Queue Overflow - "
        Write-Verbose "queuesObj_OverFlowOn: $($queuesObj.OverFlowOn)"
        Write-Verbose "overflowCallAction: $overflowCallAction"
        Write-Verbose "queuesObj_OverflowThreshold: $($queuesObj.OverflowThreshold)"
        Write-Verbose "queuesObj_OverflowCandidate: $($queuesObj.OverflowCandidate)"

        $Queue.OverflowThreshold = $queuesObj.OverflowThreshold          
        $Queue.OverflowCandidate = $queuesObj.OverflowCandidate #needs to be added to form
        $Queue.OverflowAction = $overflowCallAction #Need to create a action in C#
    }
	elseif (!$queuesObj.OverFlowOn)
	{
		Write-Verbose "Queue Overflow disabled"
        $Queue.OverflowThreshold = $null
	}
    else
    {
        Write-Warning "Queue Overflow not updated" 
    }

    #Set queue group 
    #Write-Host "Queues"
    Write-Verbose "Preparing to add agents to Group"
    $Queue.AgentGroupIDList.Clear()

    foreach ($group in $queuesObj.AgentGroupIDList.QueueGroups)
    {     
        Write-Verbose "Adding Group $group to Queue"
        $Queue.AgentGroupIDList.Add((Get-CsRgsAgentGroup -Name $group).Identity)
    }

    Write-Verbose "Commiting Queue"
    Set-CsRgsQueue -Instance $Queue -Verbose
}

#Get-CsRgsAgentGroup -Name $groupsObj.Name
#Get-CsRgsAgentGroup -Name "New 3"
#Stop-Transcript
#$Queue
#$queuesObj.AgentGroupIDList.QueueGroups
#Get-CsRgsQueue | where {$_.Name -like "New*"}
#Get-CsRgsQueue | where {$_.Name -like "New*"} | Remove-CsRgsQueue


<#           
Name!
Description!
Identity
OwnerPool
OverflowThreshold!!       
OverflowCandidate!
OverflowAction!
            
TimeoutThreshold!
TimeoutAction!

AgentGroupIDList!

#>

 #New-CsRgsCallAction
        # -Action: Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
        # -Prompt(played before transfer,created using New-CsRgsPrompt) - 
            #New-CsRgsPrompt
            # -AudioFilePrompt(audio to play, created using Import-CsRgsAudioFile): Import-CsRgsAudioFile -Identity "service:ApplicationServer:<pool>" -FileName "audio.wav" -Content (Get-Content C:\audio.wav -Encoding byte -ReadCount 0)
            # -TextToSpeechPrompt(text to be read as speech): "bla bla bla"
        # -Question(question to be asked if Action is TransferToQuestion, created using New-CsRgsQuestion):
            #New-CsRgsQuestion
            # -Name
            # -AnswerList(array of valid voice or DTMF inputs, created using New-CsRgsAnswer) -
                #New-CsRgsAnswer
                # -Name
                # -Action(created using New-CsRgsCallAction): Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
                # -DtmfResponse(Dtmf key): 0-9
                # -VoiceResponseList(comma seperated list of valid voice responses): e.g Hardware, Devices             
            # -InvalidAnswerPrompt(message to play when input is invalid - after playing prompt is played again, created using New-CsRgsPrompt): Example above
            # -NoAnswerPrompt(message to play when no input recieved, created using New-CsRgsPrompt): Example above
            # -Prompt(question to be asked, created using New-CsRgsPrompt): Example above
        # -QueueID(Queue ID to transfer call to when Action is TransferToQueue): (Get-CsRgsQueue - Name "bla").Identity
        # -Uri(Uri to transfer to when Action is TransferToUri; TransferToVoiceMailUri; or TransferToPSTN): sip:<user|number>@<sipdomain.com>