param(
$Action, #!! Terminate, TransferToPstn, TransferToQuestion, TransferToQueue, TransferToUri, TransferToVoicemailUri
$Prompt, #Prompt to be played before the call action takes place
$Question, #Question to be asked if the Action has been set to TransferToQuestion. !!required if the Action has been set to TransferToQuestion
$QueueID, #Identity of the Response Group queue the call should be transferred to. !!required if the Action is set to TransferToQueue.
$Uri #SIP address, voice mail URI, or PSTN telephone number. !!required if the Action has been set to TransferToUri; TransferToVoiceMailUri; or TransferToPSTN
)

#New-CsRgsCallAction
#The Response Group application uses call actions to determine what the system does when a call is received. For example, 
#a call action might specify that a call be transferred to another queue; that a specific Response Group question be asked; or that the call be ended.
$Action = "TransferToUri"
#$Prompt
#$Question
#$QueueID
$Uri = "sip:u1@ucgeek.nz"

if ($Action -eq "TransferToQuestion")
{
    if ($Prompt)
    {
        New-CsRgsCallAction -Prompt $Prompt -Question $Question -Action TransferToQuestion
    }
    else
    {
        New-CsRgsCallAction -Question $Question -Action TransferToQuestion
    }
}


if ($Action -eq "TransferToQueue")
{
    if ($Prompt)
    {
        New-CsRgsCallAction -Prompt $Prompt -Action TransferToQueue -QueueID $QueueID
    }
    else
    {
        New-CsRgsCallAction -Action TransferToQueue -QueueID $QueueID
    }
}

if ($Action -eq "TransferToUri" -or $Action -eq "TransferToVoiceMailUri" -or $Action -eq "TransferToPSTN")
{
    if ($Prompt)
    {
        New-CsRgsCallAction -Prompt $Prompt -Action $Action -Uri $Uri
    }
    else
    {
        New-CsRgsCallAction -Action $Action -Uri $Uri
    }
}
