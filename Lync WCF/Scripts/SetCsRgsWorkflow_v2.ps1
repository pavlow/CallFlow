param(
$wfObj
)
$VerbosePreference = "continue"

<#
cls
#Stuff that relates to IVR or Queue
$wfObj = New-Object PSObject
#Welcome Message = DefaultAction prompt
$wfObj | Add-Member -MemberType NoteProperty -Name "WelcomeMessage" -Value "This is a test welcome message from the script"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioWelcome" -Value "TestAudio - Welcome.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioWelcomeFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio - Welcome.wma"

#Queue
$wfQueueObj = New-Object PSObject
$wfQueueObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "Queue 5"
#$wfQueueObj | Add-Member -MemberType NoteProperty -Name "Id" -Value "service:ApplicationServer:CFM-S4BFE01.callflowmanager.nz/d987e859-5e14-4eaa-ad3a-da0ef8d2fdfb"
$wfQueueObj | Add-Member -MemberType NoteProperty -Name "Id" -Value "d987e859-5e14-4eaa-ad3a-da0ef8d2fdfb"
$wfObj | Add-Member -MemberType NoteProperty -Name "InvoiceQueue" -Value $wfQueueObj

#IVR
$wfObj | Add-Member -MemberType NoteProperty -Name "EnableIVRMode" -Value $true

#IVR Sub1 Options
$wfIvrOptionsObjArray = @()
$wfChildIvrNodesObjArray = @()
#1
$wfChildIvrNodesObj = New-Object PSObject
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "Number" -Value 1
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "TextIvrMessage" -Value "Child Voice Response 1"
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "AudioIvrTree" -Value "TestAudio - 1.wma"
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "AudioIvrTreeFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio - 1.wma"
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "ParentHasChild" -Value $false
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "CanAddChildNode" -Value $false
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "InvoiceQueue" -Value $wfQueueObj
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "ChildIvrNodes" -Value $null
$wfChildIvrNodesObjArray += $wfChildIvrNodesObj
#2
$wfChildIvrNodesObj = New-Object PSObject
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "Number" -Value 2
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "TextIvrMessage" -Value "Child Voice Response 2"
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "AudioIvrTree" -Value "TestAudio - 2.wma"
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "AudioIvrTreeFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio - 2.wma"
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "ParentHasChild" -Value $false
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "CanAddChildNode" -Value $false
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "InvoiceQueue" -Value $wfQueueObj
$wfChildIvrNodesObj | Add-Member -MemberType NoteProperty -Name "ChildIvrNodes" -Value $null
$wfChildIvrNodesObjArray += $wfChildIvrNodesObj


#IVR Root Options
#1
$wfIvrOptionsObj = New-Object PSObject
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "Number" -Value 1
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "Root Voice Response 1" #This should be TextIvrMessage like child options, but needs changing in C#
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "AudioIvrTree" -Value "TestAudio - 1.wma"
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "AudioIvrTreeFilePath" -Value "" #"E:\Code\Andrew\RGSFiles\TestAudio - 1.wma"
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "ParentHasChild" -Value $true
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "CanAddChildNode" -Value $true
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "InvoiceQueue" -Value $null
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "ChildIvrNodes" -Value $wfChildIvrNodesObjArray
$wfIvrOptionsObjArray += $wfIvrOptionsObj

#2
$wfIvrOptionsObj = New-Object PSObject
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "Number" -Value 2
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "Root Voice Response 2" #This should be TextIvrMessage like child options, but needs changing in C#
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "AudioIvrTree" -Value "TestAudio - 2.wma"
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "AudioIvrTreeFilePath" -Value "" #"E:\Code\Andrew\RGSFiles\TestAudio - 2.wma"
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "ParentHasChild" -Value $true
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "CanAddChildNode" -Value $true
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "InvoiceQueue" -Value $null
$wfIvrOptionsObj | Add-Member -MemberType NoteProperty -Name "ChildIvrNodes" -Value $wfChildIvrNodesObjArray
$wfIvrOptionsObjArray += $wfIvrOptionsObj

$wfObj | Add-Member -MemberType NoteProperty -Name "IvrOptions" -Value $wfIvrOptionsObjArray

$wfObj | Add-Member -MemberType NoteProperty -Name "IvrMessage" -Value "This is a test IVR message from the script"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioIvr" -Value "TestAudio - Ivr Root.wma" #Dont think this exisits in C#
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioIvrFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio - Ivr Root.wma"

#--------------------------------------------------------------------------------------------------------------------
$num = "1"
#WORKFLOW GENERAL PROPERTIES
$wfObj | Add-Member -MemberType NoteProperty -Name "OwnerPool" -Value "CFM-S4BFE01.callflowmanager.nz"
$wfObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "Script Test $num"
$wfObj | Add-Member -MemberType NoteProperty -Name "Description" -Value "This is a test descript from the script"
#Need to add to C#???
$wfObj | Add-Member -MemberType NoteProperty -Name "Number" -Value "tel:+6497997$num" #Need logic to add tel:
$wfObj | Add-Member -MemberType NoteProperty -Name "DisplayNumber" -Value "+64 9 799 7777"
#$wfObj | Add-Member -MemberType NoteProperty -Name "SipAddress" -Value ""
$wfObj | Add-Member -MemberType NoteProperty -Name "Uri" -Value "scripttest$num" #This is the Left side of the sip address
$wfObj | Add-Member -MemberType NoteProperty -Name "SipDomain" -Value "callflowmanager.nz" #This is the right side of the sip address

#Needs to be added in C#:
$wfObj | Add-Member -MemberType NoteProperty -Name "EnableWorkflow" -Value $true
$wfObj | Add-Member -MemberType NoteProperty -Name "EnableAgentAnonymity" -Value $true
$wfObj | Add-Member -MemberType NoteProperty -Name "EnableForFederation" -Value $true
$wfObj | Add-Member -MemberType NoteProperty -Name "Language" -Value "en-AU"
$wfObj | Add-Member -MemberType NoteProperty -Name "TimeZone" -Value "Samoa Standard Time"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioHoldMusic" -Value "TestAudio - Music.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioHoldMusicFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio - Music.wma"

#AFTER HOURS
#After hour Group
$wfAfterHoursGroupObj = New-Object PSObject
$wfAfterHoursGroupObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "Bus Hours Group 4"
$wfObj | Add-Member -MemberType NoteProperty -Name "BusinessHoursGroup" -Value $wfAfterHoursGroupObj

#After hours destination
#$wfObj | Add-Member -MemberType NoteProperty -Name "BusinessHourDestination" -Value ""
$wfObj | Add-Member -MemberType NoteProperty -Name "AfterHoursDestination" -Value "TransferToUri" #Dont think this exisits in C#
$wfObj | Add-Member -MemberType NoteProperty -Name "AfterHoursMessage" -Value "This is a test afterhours message from the script"
$wfObj | Add-Member -MemberType NoteProperty -Name "AfterHoursUri" -Value "ahuser"
$wfObj | Add-Member -MemberType NoteProperty -Name "AfterHoursSipDomain" -Value "callflowmanager.nz"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioAfterHours" -Value "TestAudio - AfterHours.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioAfterHoursFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio - AfterHours.wma"

#HOLIDAYS
#Hol Group
$wfHolGroupObj = New-Object PSObject
$wfHolGroupObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "Hol Group 4"
$wfObj | Add-Member -MemberType NoteProperty -Name "HolidayGroup" -Value $wfHolGroupObj

#Hol destination
$wfObj | Add-Member -MemberType NoteProperty -Name "HolidayDestination" -Value "TransferToUri"
$wfObj | Add-Member -MemberType NoteProperty -Name "HolidayMessage" -Value "This is a test holiday message from the script"
$wfObj | Add-Member -MemberType NoteProperty -Name "HolidayUri" -Value "holidayuser"
$wfObj | Add-Member -MemberType NoteProperty -Name "HolidaySipDomain" -Value "callflowmanager.nz"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioHolidays" -Value "TestAudio.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "AudioHolidaysFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio - Holidays.wma"
#>
#--------------------------------------------------------------------------------------------------------------------

##LOGIC#####################:::

#Build Workflow SIP Address Uri
$wfSipAddress =$null
if($wfObj.Uri -ne "" -and $wfObj.SipDomain -ne "")
{            
    if ($wfObj.Uri -like "sip:*")
    {
        $wfSipAddress = $wfObj.Uri + "@" + $wfObj.SipDomain 
    }
    else
    {
        $wfSipAddress = "sip:" + $wfObj.Uri + "@" + $wfObj.SipDomain
    }
}

#Build Workflow LineUri (TEL)
$wfLineUri =$null
if($wfObj.Number -ne "")
{            
    if ($wfObj.Number -like "tel:*")
    {
        $wfLineUri = $wfObj.Number
    }
    else
    {
        $wfLineUri = "tel:" + $wfObj.Number
    }
}

#Build After Hours Uri
$afterHoursUri =$null
if($wfObj.afterHoursUri -ne "" -and $wfObj.afterHoursSipDomain -ne "")
{             
    if ($wfObj.afterHoursUri -like "sip:*")
    {
        $afterHoursUri = $wfObj.afterHoursUri + "@" + $wfObj.afterHoursSipDomain 
    }
    else
    {
        $afterHoursUri = "sip:" + $wfObj.afterHoursUri + "@" + $wfObj.afterHoursSipDomain 
    }
}

#Build Holiday Uri
$holidayUri =$null
if($wfObj.holidayUri -ne "" -and $wfObj.holidaySipDomain -ne "")
{                
    if ($wfObj.holidayUri -like "sip:*")
    {
        $holidayUri = $wfObj.holidayUri + "@" + $wfObj.holidaySipDomain 
    }
    else
    {
        $holidayUri = "sip:" + $wfObj.holidayUri + "@" + $wfObj.holidaySipDomain 
    }
}

#Build Queue full Id
Function QueueId ($OwnerPool, $QueueInstanceId)
{
    if ($OwnerPool -and $QueueInstanceId)
    {
        return "service:ApplicationServer:"+ $OwnerPool + "/" + $QueueInstanceId
    }
    else
    {
        return $null
    }
}

Function CsRgsAudioFile ($pool, $audioFilePath)
{
    $fileName = $audioFilePath.Substring($audioFilePath.LastIndexOf("\") + 1)
    ##Write-Host "Audio File - $pool, $audioFilePath, $fileName"
    return Import-CsRgsAudioFile -Identity "service:ApplicationServer:$pool" -FileName $fileName -Content (Get-Content $audioFilePath -Encoding byte -ReadCount 0) -Verbose
}

Function CsRgsPrompt($textToSpeechPrompt, $audioFilePath, $pool)
{
    if ($pool -and $audioFilePath)
    {
        ##Write-Host "csRgsAudioFile - $pool, $audioFilePath"
        $csRgsAudioFile = CsRgsAudioFile $pool $audioFilePath
        ##Write-Host "Test: $csRgsAudioFile"
    }
    
    if ($csRgsAudioFile -and $textToSpeechPrompt)
    {
        ##Write-Host "Audio and Text to Speech Prompt"
        return New-CsRgsPrompt -AudioFilePrompt $csRgsAudioFile -TextToSpeechPrompt $textToSpeechPrompt -Verbose
    }
    elseif ($csRgsAudioFile)
    {
       ##Write-Host "Audio Only Prompt"
       return New-CsRgsPrompt -AudioFilePrompt $csRgsAudioFile -Verbose
    }
    elseif($textToSpeechPrompt)
    {
        ##Write-Host "Text to Speech Only Prompt"
        return New-CsRgsPrompt -TextToSpeechPrompt $textToSpeechPrompt -Verbose
    }
    else
    {
     return $null
    }
}

Function CsRgsDefaultCallAction($action, $ivrOptions, $ivrMessage, $existingIvrPrompt, $AudioIvr, $ivrAudioFilePath, $queueId, $uri, $textToSpeechPrompt, $existingPrompt, $audioFileName, $audioFilePath, $pool)
{
    Write-Verbose "Invoking CsRgsDefaultCallAction"
	
	#Create Welcome Message prompt
	Write-Verbose "Creating Welcome Prompt - "
	Write-Verbose "audioFilePath: $audioFilePath"
	Write-Verbose "audioFileName: $audioFileName"
	Write-Verbose "extistingPrompt: $($existingPrompt)"
	if ($existingPrompt.AudioFilePrompt -and $audioFileName -and ($audioFilePath -eq $null -or $audioFilePath -eq "")) # -and $audioFileName -and !$audioFilePath)
	{
		Write-Verbose "Updating exisiting Welcome Prompt"
		#if there is an exisiting prompt and an audio file name exists but no file path, then we need to update the exisiting prompt				
		$welcomePrompt = $existingPrompt
		$welcomePrompt.TextToSpeechPrompt = $textToSpeechPrompt
	}
    elseif($audioFilePath -and $pool)
    {
        Write-Verbose "Creating new audio Welcome Prompt"
		##Write-Host "Create Prompt - $textToSpeechPrompt, $audioFilePath, $pool"
        $welcomePrompt = CsRgsPrompt $textToSpeechPrompt $audioFilePath $pool   
    }
    elseif($textToSpeechPrompt)
    {
		Write-Verbose "Creating new text to speech Welcome Prompt"
        $welcomePrompt = CsRgsPrompt $textToSpeechPrompt   
    }
	elseif (!$audioFileName)
	{
		Write-Verbose "Removing Welcome Prompt"
		$welcomePrompt = $null
	}


    #If the action is TransferToQueue:
    if($action -eq "TransferToQueue")
    {
        Write-Verbose "DefaultAction is TransferToQueue"

		if ($queueId)
        {
            #$queueId = (Get-CsRgsQueue -Name $queue).Identity
            
            ##Write-Host "Action is TransferToQueue"
            if($welcomePrompt -and $queueId)
            {
                ##Write-Host "$action with prompt"
                New-CsRgsCallAction -Action $action -Prompt $welcomePrompt -QueueID $queueId -Verbose
            }
            elseif($queueId)
            {        
                ##Write-Host "$action without prompt"
                New-CsRgsCallAction -Action $action -QueueID $queueId -Verbose
            }
        }
    }

    #If the action is TransferToQuestion:
    if($action -eq "TransferToQuestion")
    {
		Write-Verbose "DefaultAction is TransferToQuestion"

		#Create Root Ivr Message
		Write-Verbose "Creating Root Ivr Prompt - "
		Write-Verbose "ivrRootPrompt_TextToSpeechPrompt: $($ivrRootPrompt.TextToSpeechPrompt)"
		Write-Verbose "AudioIvr: $AudioIvr"
		Write-Verbose "AudioIvr: $AudioIvr"
		Write-Verbose "existingIvrPrompt: $($existingIvrPrompt)"
		if ($existingIvrPrompt.AudioFilePrompt -and $AudioIvr -and ($ivrAudioFilePath -eq $null -or $ivrAudioFilePath -eq "")) # -and $audioFileName -and !$audioFilePath)
		{
			Write-Verbose "Updating exisiting Root Ivr Prompt"
			#if there is an exisiting prompt and an audio file name exists but no file path, then we need to update the exisiting prompt			
			$ivrRootPrompt = $existingIvrPrompt
			$ivrRootPrompt.TextToSpeechPrompt = $ivrMessage
		}
		elseif($ivrAudioFilePath -and $pool)
		{
			Write-Verbose "Creating new audio Root Ivr Prompt"	
			##Write-Host "Create Prompt - $textToSpeechPrompt, $audioFilePath, $pool"
			$ivrRootPrompt = CsRgsPrompt $ivrMessage $ivrAudioFilePath $pool   
		}
		elseif($ivrMessage)
		{
			Write-Verbose "Creating new text to speech Root Ivr Prompt"
			$ivrRootPrompt = CsRgsPrompt $ivrMessage $null $null  
		}
		else
		{
			Write-Warning "No prompt was specified - IVR prompts are required so that callers know that DTMF or voice commands are required. Please replace this text, or upload an audio file"	
			$ivrRootPrompt = CsRgsPrompt "Required! IVR prompts are required so that callers know that DTMF or voice commands are required. Please replace this text, or upload an audio file" $null $null 
		}
		    
        if ($ivrOptions -ne $null)
        {
            $RootCsRgsAnswerArray = @()
            
            foreach ($ivrOption in $ivrOptions)
            {
                #Write-Host "Ivr Root Option"
                
                #If the Root Ivr has children:
                if ($ivrOption.ChildIvrNodes -ne $null)
                {
                    $ChildCsRgsAnswerArray = @()

                    #Root has sub options
                    foreach ($ChildIvrNode in $ivrOption.ChildIvrNodes)
                    {
                        #Write-Host "Creating Child Ivr Option"
                        #$ChildIvrNode.Number $ChildIvrNode.Name $ChildIvrNode.AudioIvrTreeFilePath #ChildIvrNode.NEEDTESTTOSPEECH $ChildIvrNode.SelectedInvoiceQueue
                                       
                        #ChildIvrNode's can only have an option of TransferToQueue
                        $ChildCsRgsCallAction = New-CsRgsCallAction -Action TransferToQueue -QueueID (QueueId $WfObj.OwnerPool $ChildIvrNode.InvoiceQueue.Id)  -Verbose #(Get-CsRgsQueue -Name $ChildIvrNode.SelectedInvoiceQueue).Identity                                                                                                      

                        #Child AnswerList
                        $ChildCsRgsAnswer = New-CsRgsAnswer -Name "CfmCsRgsAnswer" -DtmfResponse $ChildIvrNode.Number -VoiceResponseList $ChildIvrNode.Name -Action $ChildCsRgsCallAction -Verbose
                        $ChildCsRgsAnswerArray += $ChildCsRgsAnswer

                        #NOT REQUIRE I THINK:
                        #$DefaultActionQuestion = CsRgsDefaultCallAction -action "TransferToQuestion" -ivrOptions $wfObj.IvrOptions -ivrMessage $wfObj.IvrMessage -ivrAudioFilePath $wfObj.AudioIvrFilePath -queue $wfObj.Queue.Name -uri $null -textToSpeechPrompt $wfObj.WelcomeMessage -audioFilePath $wfObj.AudioWelcomeFilePath -pool $wfObj.OwnerPool
                        #$wf.DefaultAction = $DefaultActionQuestion
                    }

                    #Add the child AnswerList to the child Question:
                        #TEMP PROMPT - Need to access the new prompt settings from sub Ivr options
                        #$tempPrompt = New-CsRgsPrompt -TextToSpeechPrompt "Child Ivr Prompt - Hard coded temp, Needs real data"

					#$ivrPromptExists = ($wf | Select -ExpandProperty DefaultAction | Select -ExpandProperty Question | Select -ExpandProperty AnswerList | where {$_.DtmfResponse -eq $ivrOption.Number}).Action.Prompt
                    $ivrPromptExists = ($wf | Select -ExpandProperty DefaultAction | Select -ExpandProperty Question | Select -ExpandProperty AnswerList | where {$_.DtmfResponse -eq $ivrOption.Number}).Action.Question.Prompt

					Write-Verbose "Creating Sub Ivr Question - "					
					Write-Verbose "ivrOption_AudioIvrTree: $($ivrOption.AudioIvrTree)"
					Write-Verbose "ivrOption_AudioIvrTreeFilePath: $($ivrOption.AudioIvrTreeFilePath)"
					Write-Verbose "ivrOption_TextIvrMessage: $($ivrOption.TextIvrMessage)"
					Write-Verbose "ivrPromptExists: $ivrPromptExists"
										
					if ($ivrPromptExists.AudioFilePrompt -and $ivrOption.AudioIvrTree -and ($ivrOption.AudioIvrTreeFilePath -eq $null -or $ivrOption.AudioIvrTreeFilePath -eq ""))
					{
						Write-Verbose "Creating new Sub Ivr Question with updated exisiting Prompt"
						#if there is an exisiting prompt and an audio file name exists but no file path, then we need to update the exisiting prompt			
						$ivrPromptExists.TextToSpeechPrompt = $ivrOption.TextIvrMessage
						$ChildCsRgsQuestion = New-CsRgsQuestion -Name "CfmCsRgsQuestion" -AnswerList $ChildCsRgsAnswerArray -Prompt $ivrPromptExists -Verbose
					}
					elseif($ivrOption.AudioIvrTreeFilePath -and $pool)
					{
						Write-Verbose "Creating new Sub Ivr Question with audio Prompt"
						$ChildCsRgsQuestion = New-CsRgsQuestion -Name "CfmCsRgsQuestion" -AnswerList $ChildCsRgsAnswerArray -Prompt (CsRgsPrompt $ivrOption.TextIvrMessage $ivrOption.AudioIvrTreeFilePath $WfObj.OwnerPool) -Verbose
					}
					elseif($ivrOption.TextIvrMessage)
					{
						Write-Verbose "Creating new Sub Ivr Question with text to speech Prompt"
						$ChildCsRgsQuestion = New-CsRgsQuestion -Name "CfmCsRgsQuestion" -AnswerList $ChildCsRgsAnswerArray -Prompt (CsRgsPrompt $ivrOption.TextIvrMessage) -Verbose
					}
					else
					{
						Write-Verbose "Creating new Sub Ivr Question"
						Write-Warning "No prompt was specified - IVR prompts are required so that callers know that DTMF or voice commands are required. Please replace this text, or upload an audio file"	
						$ChildCsRgsQuestion = New-CsRgsQuestion -Name "CfmCsRgsQuestion" -AnswerList $ChildCsRgsAnswerArray -Prompt (CsRgsPrompt "Required! IVR prompts are required so that callers know that DTMF or voice commands are required. Please replace this text, or upload an audio file") -Verbose
					}
					

                    #-InvalidAnswerPrompt -NoAnswerPrompt
                    
                    #Write-Host "Creating Root Ivr Option that transfers to question..."
                    #$ivrOption.Number $ivrOption.Name $ivrOption.AudioIvrTreeFilePath #ivrOption.NEEDTESTTOSPEECH $ivrOption.SelectedInvoiceQueue
                                       
                    #Root Ivr has children so Action is TransferToQuestion (child)
					Write-Verbose "Creating CallAction TransferToQuestion - "
					Write-Verbose "ChildCsRgsQuestion: $($ChildCsRgsQuestion)"
                    $RootCsRgsCallAction = New-CsRgsCallAction -Action TransferToQuestion -Question $ChildCsRgsQuestion -Verbose

                    #Root AnswerList
					Write-Verbose "Creating CsRgsAnswer - "		
					Write-Verbose "ivrOption_Number: $($ivrOption.Number)"
					Write-Verbose "ivrOption_Name: $($ivrOption.Name)"
					Write-Verbose "RootCsRgsCallAction: $($RootCsRgsCallAction)"			
					$RootCsRgsAnswer = New-CsRgsAnswer -Name "CfmCsRgsAnswer" -DtmfResponse $ivrOption.Number -VoiceResponseList $ivrOption.Name -Action $RootCsRgsCallAction -Verbose
                    $RootCsRgsAnswerArray += $RootCsRgsAnswer

                }#if root ivr has children      
                else
                {
                    #Write-Host "Creating Root Ivr Option that transfers to queue..."
                    
                    #if the root has no children, then Action has to be TransferToQueue
                    $RootCsRgsCallAction = New-CsRgsCallAction -Action TransferToQueue -QueueID (QueueId $WfObj.OwnerPool $ivrOption.InvoiceQueue.Id) -Verbose #(Get-CsRgsQueue -Name $ivrOption.SelectedInvoiceQueue).Identity 

                    #Root AnswerList
                    $RootCsRgsAnswer = New-CsRgsAnswer -Name "CfmCsRgsAnswer" -DtmfResponse $ivrOption.Number -VoiceResponseList $ivrOption.Name -Action $RootCsRgsCallAction -Verbose
                    $RootCsRgsAnswerArray += $RootCsRgsAnswer

                }#root has no children


            }#foreach IvrOption           
                
			Write-Verbose "Creating CsRgsQuestion - "
			Write-Verbose "RootCsRgsAnswerArray $($RootCsRgsAnswerArray)"
			Write-Verbose "ivrRootPrompt $($ivrRootPrompt)"
            #Add the root AnswerList to the root Question:
            #Prompt for Ivr Root
            #$RootCsRgsPrompt = New-CsRgsPrompt -TextToSpeechPrompt $wfObj.IvrMessage -AudioFilePrompt $ivrRootPrompt
            $RootCsRgsQuestion = New-CsRgsQuestion -Name "CfmCsRgsQuestion" -AnswerList $RootCsRgsAnswerArray -Prompt $ivrRootPrompt -Verbose
            #-InvalidAnswerPrompt -NoAnswerPrompt 

            #We should now have the IvrOptions Creatingready for use. Now we create the DefaultAction and add the ivr options
            
            #For the current root option
            #Write-Host "Creating DefaultAction of TransferToQuestion..."
            if($welcomePrompt -and $RootCsRgsAnswerArray)
            {
                ##Write-Host "$action with prompt"
				Write-Verbose "Creating CallAction with Welcome Prompt - "
				Write-Verbose "action: $action"
				Write-Verbose "welcomePrompt: $($welcomePrompt)"
				Write-Verbose "RootCsRgsQuestion: $($RootCsRgsQuestion)"
                New-CsRgsCallAction -Action $action -Prompt $welcomePrompt -Question $RootCsRgsQuestion -Verbose
            }
            elseif($RootCsRgsAnswerArray)
            {        
                ##Write-Host "$action without prompt"
				Write-Verbose "Creating CallAction without Welcome Prompt - "
				Write-Verbose "action: $action"
				Write-Verbose "RootCsRgsQuestion: $($RootCsRgsQuestion)"
                New-CsRgsCallAction -Action $action -Question $RootCsRgsQuestion -Verbose
            }        
            
        }

    }
}


Function CsRgsCallAction($action, $queueId, $uri, $textToSpeechPrompt, $existingPrompt, $audioFileName, $audioFilePath, $pool) #-existingAudioPrompt $wf.HolidayAction.Prompt -audioFileName $wfObj.AudioAfterHours 
{
    #Create prompt
	if ($existingPrompt.AudioFilePrompt -and $audioFileName -and ($audioFilePath -eq $null -or $audioFilePath -eq "")) # -and $audioFileName -and !$audioFilePath)
	{
		#if there is an exisiting prompt and an audio file name exists but no file path, then we need to update the exisiting prompt		
		Write-Verbose "Updating exisiting prompt"
		$prompt = $existingPrompt
		$prompt.TextToSpeechPrompt = $textToSpeechPrompt
	}
    elseif($audioFilePath -and $pool)
    {
        ##Write-Host "Create Prompt - $textToSpeechPrompt, $audioFilePath, $pool"
        $prompt = CsRgsPrompt $textToSpeechPrompt $audioFilePath $pool   
    }
	elseif($textToSpeechPrompt)
	{
		$prompt = CsRgsPrompt $textToSpeechPrompt
	}
	else
	{
		$prompt = $null
	}

    #If the action is Terminate:
    if($action -eq "Terminate")
    {          
        ##Write-Host "Action is Terminate"
        if($prompt)
        {
            ##Write-Host "$action with prompt"
            New-CsRgsCallAction -Action $action -Prompt $prompt -Verbose
        }
        else
        {        
            ##Write-Host "$action without prompt"
            New-CsRgsCallAction -Action $action -Verbose
        }
    }

    #If the action is TransferToVoicemailUri/TransferToUri/TransferToPstn:
    if($action -eq "TransferToVoicemailUri" -or $action -eq "TransferToUri" -or $action -eq "TransferToPstn")
    {
        if ($uri)
        {
            ##Write-Host "Action is TransferToVoicemailUri/TransferToUri/TransferToPstn"
            if($prompt)
            {
                ##Write-Host "$action with prompt"
                New-CsRgsCallAction -Action $action -Prompt $prompt -Uri $uri -Verbose
            }
            else
            {        
                ##Write-Host "$action without prompt"
                New-CsRgsCallAction -Action $action -Uri $uri -Verbose
            }
        }
    }

    #If the action is TransferToQueue:
    if($action -eq "TransferToQueue")
    {
        #Not supported  - Lync/SfB wont support this action
    }

    #If the action is TransferToQuestion:
    if($action -eq "TransferToQuestion")
    {
        #Not supported yet, but likely Lync/SfB wont support this action - to test
    }
}


function WfShared()
{
    #Music on hold
    if ($wfObj.AudioHoldMusicFilePath -and $wfObj.OwnerPool)
    {
		#if file path is specified then update the audio file
		Write-Verbose "Updating Music on Hold Audio"
		$MusicOnHold = CsRgsAudioFile -pool $wfObj.OwnerPool -audioFilePath $wfObj.AudioHoldMusicFilePath

        if ($MusicOnHold)
        {
            $wf.CustomMusicOnHoldFile = $MusicOnHold
        }
    }
	elseif (!$wfObj.AudioHoldMusic)
	{
		Write-Verbose "Removing Music on Hold Audio"
		#if there is no audio file name, then we assume it should be removed
		$wf.CustomMusicOnHoldFile = $null
	}

    #After hours
    #if ($wfObj.BusinessHoursGroup.Name -and $wfObj.EnableBusinessHours)
    if ($wfObj.BusinessHoursGroup.Name)
    {
        #$wf.BusinessHoursID = "service:ApplicationServer:CFM-S4BFE01.callflowmanager.nz/047721bc-40d7-4dac-b562-ddc6948379b1"#$wfObj.BusinessHoursID#This doesnt exisit in C# yet
        $wf.BusinessHoursID = ((Get-CsRgsHoursOfBusiness -Name $wfObj.BusinessHoursGroup.Name).Identity)
    }
    else 
    {
        <#
        #Not required, appears this is what happens by default when there is not BusinessHoursId associated - not actually sure 100% how this works. Appears you cant assigned blank/null value for BusinessHoursID. If you customise the default in Wf, it created and assigned a BusinessHoursID automatically

        $BusHoursGroupExists = Get-CsRgsHoursOfBusiness | Where {$_.OwnerPool -eq $wfObj.OwnerPool -and $_.Name -eq $wfObj.BusinessHoursGroup.Name}

        if (!$BusHoursGroupExists)
        {
            #Create a default group
            $24h = New-CsRgsTimeRange -Name "24/5" -OpenTime "00:00" -CloseTime "23:59"
            $24_5 = New-CsRgsHoursOfBusiness -Parent "service:ApplicationServer:$($wfObj.OwnerPool)" -Name "Default 24/5" -MondayHours1 $24h -TuesdayHours1 $24h -WednesdayHours1 $24h -ThursdayHours1 $24h -FridayHours1 $24h
            $wf.BusinessHoursID = $24_5
        }
        elseif ($BusHoursGroupExists)
        {

        }
        #>
    }

    if ($wfObj.AfterHoursDestination) #-and $afterHoursUri
    {
        Write-Verbose "Updating After Hours Destination"
		$AfterHoursAction = CsRgsCallAction -action $wfObj.AfterHoursDestination -uri $afterHoursUri -textToSpeechPrompt $wfObj.AfterHoursMessage -existingPrompt $wf.NonBusinessHoursAction.Prompt -audioFileName $wfObj.AudioAfterHours -audioFilePath $wfObj.AudioAfterHoursFilePath -queueId $null -pool $wfObj.OwnerPool
        $wf.NonBusinessHoursAction = $AfterHoursAction        
    }
    
    #Holidays
    if ($wfObj.HolidayGroup.Name -and $wfObj.EnableHolidays)
    {
        #$wf.HolidaySetIDList.Add("service:ApplicationServer:CFM-S4BFE01.callflowmanager.nz/c481746d-0da2-45a1-9695-b61a368cc54e") #$wfObj.HolidaySetIDList#This doesnt exisit in C# yet
        $wf.HolidaySetIDList.Clear()
        $wf.HolidaySetIDList.Add((Get-CsRgsHolidaySet -Name $wfObj.HolidayGroup.Name).Identity)
    }
	else
	{
		$wf.HolidaySetIDList.Clear()
	}

    if ($wfObj.HolidayDestination) #-and $holidayUri
    {
		Write-Verbose "Updating Holiday Destination"
        $HolidayAction = CsRgsCallAction -action $wfObj.HolidayDestination -uri $holidayUri -textToSpeechPrompt $wfObj.HolidayMessage -existingPrompt $wf.HolidayAction.Prompt -audioFileName $wfObj.AudioHolidays -audioFilePath $wfObj.AudioHolidaysFilePath -queueId $null -pool $wfObj.OwnerPool
        $wf.HolidayAction = $HolidayAction        
    }    

    #Managers
    #$wf.ManagersByUri = $wfObj.ManagersByUri

    #If Ivr Mode is enabled:
    if($wfObj.EnableIVRMode)
    {
        #If IVR Mode is enabled, default action is transfer to question
        #Write-Host "DefaultAction: TransferToQuestion"
        $DefaultActionQuestion = CsRgsDefaultCallAction -action "TransferToQuestion" -ivrOptions $wfObj.IvrOptions -ivrMessage $wfObj.IvrMessage  -existingIvrPrompt $wf.DefaultAction.Question.Prompt -AudioIvr $wfObj.AudioIvr -ivrAudioFilePath $wfObj.AudioIvrFilePath -queueId $null -uri $null -textToSpeechPrompt $wfObj.WelcomeMessage -existingPrompt $wf.DefaultAction.Prompt -audioFileName $wfObj.AudioWelcome -audioFilePath $wfObj.AudioWelcomeFilePath -pool $wfObj.OwnerPool
        $wf.DefaultAction = $DefaultActionQuestion    
    
    }
    #If Ivr Mode is not enabled:
    else
    {
        #If IVR Mode is disabled, default action is tranfers to queue
        if($wfObj.Queue.Id)
        {
            #Write-Host "DefaultAction: TransferToQueue"
            $DefaultActionQueue = CsRgsDefaultCallAction -action "TransferToQueue" -ivrOptions $null -ivrMessage $null -existingIvrPrompt $null -AudioIvr $null -ivrAudioFilePath $null -queueId (QueueId $WfObj.OwnerPool $wfObj.Queue.Id)  -uri $null -textToSpeechPrompt $wfObj.WelcomeMessage -existingPrompt $wf.DefaultAction.Prompt -audioFileName $wfObj.AudioWelcome -audioFilePath $wfObj.AudioWelcomeFilePath -pool $wfObj.OwnerPool
            $wf.DefaultAction = $DefaultActionQueue        
        } 
    }

}


#Get existing workflow if it exisits
$wf = Get-CsRgsWorkflow | Where {$_.Name -eq $wfObj.Name}

#Edit exisiting Workflow
if($wf)
{    
    ##Write-Host "Edit"
	Write-Verbose "Found existing Workflow, updating..."
    $wf.Name = $wfObj.Name
    $wf.Description = $wfObj.Description
    $wf.PrimaryUri = $wfSipAddress
    $wf.LineUri = $wfLineUri
    $wf.DisplayNumber = $wfObj.DisplayNumber
    $wf.Active = $wfObj.EnableWorkflow
    $wf.Anonymous = $wfObj.EnableAgentAnonymity
    $wf.EnabledForFederation = $wfObj.EnableForFederation
    $wf.Language = $wfObj.Language
    $wf.TimeZone = $wfObj.TimeZone
    
    WfShared    
}
#Create a new Workflow
else
{
    ##Write-Host "New"
	Write-Verbose "Creating new Workflow"
    $wf = New-CsRgsWorkflow -Parent $wfObj.OwnerPool `
    -Name $wfObj.Name `
    -Description $wfObj.Description `
    -PrimaryUri $wfSipAddress `
    -LineUri $wfLineUri `
    -DisplayNumber $wfObj.DisplayNumber `
    -Active $wfObj.EnableWorkflow `
    -Anonymous $wfObj.EnableAgentAnonymity `
    -EnabledForFederation $wfObj.EnableForFederation `
    -Language $wfObj.Language `
    -TimeZone $wfObj.TimeZone `
    -InMemory -Verbose
    
    WfShared
}

#Save Workflow
if ($wf)
{
	Write-Verbose "Commiting Workflow"
    Set-CsRgsWorkflow -Instance $wf -Verbose
    return $wf
}
else 
{
    return $null
}


<#

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



C# Variables:
HolidayGroupViewModel afterHoursGroup
BusinessHourGroupViewModel businessHoursGroup
HolidayGroupViewModel holidayGroup
ObservableCollection<IvrViewModel> ivrOptions
QueueViewModel queue
ObservableCollection<QueueViewModel> queues

string afterHoursDestination
string afterHoursMessage
string afterHoursSipDomain
string afterHoursUri
string audioAfterHours
string audioAfterHoursFilePath
string audioHoldMusic
string audioHoldMusicFilePath
string audioHolidays
string audioHolidaysFilePath
string audioIvr
string audioIvrFilePath
string audioWelcome
string audioWelcomeFilePath
string businessHourDestination, 
string description
string displayNumber
bool enableAgentAnonymity
bool enableForFederation
bool enableIvrMode
string holidayDestination
string holidayMessage,
string holidaySipDomain
string holidayUri
string ivrMessage
string language
string name
string number
string ownerPool
string sipAddress 
string sipDomain
string timeZone
string uri 
string welcomeMessage





Parameters:
Identity
NonBusinessHoursAction
HolidayAction
DefaultAction
CustomMusicOnHoldFile
Name
Description
PrimaryUri
Active
Language
TimeZone
BusinessHoursID
Anonymous
Managed
OwnerPool
DisplayNumber
EnabledForFederation
LineUri
HolidaySetIDList
ManagersByUri

E.G:

Identity               : service:ApplicationServer:CFM-S4BFE01.callflowmanager.nz/e9fe4863-af3d-4c13-83fb-ea689ba72c0a
NonBusinessHoursAction : Prompt=Afterhours text...
                         Action=TransferToUri
                         Uri=sip:test@callflowmanager.nz
HolidayAction          : Prompt=Holidays text...
                         Action=TransferToVoicemailUri
                         Uri=sip:test@callflowmanager.nz
DefaultAction          : Prompt=Welcome text
                         Action=TransferToQueue
                         QueueId=bec87a12-f898-4c4e-a8c2-9aa7540db9d2
CustomMusicOnHoldFile  : Antenatal Afterhours.wav
Name                   : Wf 1
Description            : Description....
PrimaryUri             : sip:wf1@callflowmanager.nz
Active                 : True
Language               : en-US
TimeZone               : New Zealand Standard Time
BusinessHoursID        : Service:1-ApplicationServer-1/618c8fed-9767-4874-a974-35f3d1d9c414
Anonymous              : False
Managed                : False
OwnerPool              : CFM-S4BFE01.callflowmanager.nz
DisplayNumber          : +64 9 500 5001
EnabledForFederation   : True
LineUri                : tel:+6495005001
HolidaySetIDList       : {service:ApplicationServer:CFM-S4BFE01.callflowmanager.nz/0a4417da-3d3a-418d-abf8-37b5b1b4325b}
ManagersByUri          : {}


#Create call action
switch ($wfObj.TimeoutDestination) 
{ 
    "Terminate" {$timeoutCallAction = New-CsRgsCallAction -Action Terminate}
    "TransferToQuestion" {"Not implemented yet"}
    "TransferToVoicemailUri" {$timeoutCallAction = New-CsRgsCallAction -Action TransferToVoicemailUri -Uri $timeoutUri}
    "TransferToUri" {$timeoutCallAction = New-CsRgsCallAction -Action TransferToUri -Uri $timeoutUri}
    "TransferToQueue" {$timeoutCallAction = New-CsRgsCallAction -Action TransferToQueue -QueueID (Get-CsRgsQueue -Name $wfObj.TimeoutQueue)}
    "TransferToPstn" {$timeoutCallAction = New-CsRgsCallAction -Action TransferToPstn -Uri $timeoutUri} 
    default {$timeoutCallAction = $null}

    #$csRgsCallAction = New-CsRgsCallAction -Prompt "" -Question "" -Action Terminate -QueueID "" -Uri ""
}



#>













    <#
    #Music on hold
    if ($wfObj.AudioHoldMusicFilePath -and $wfObj.OwnerPool)
    {
        $MusicOnHold = CsRgsAudioFile -pool $wfObj.OwnerPool -audioFilePath $wfObj.AudioHoldMusicFilePath

        if ($MusicOnHold)
        {
            $wf.CustomMusicOnHoldFile = $MusicOnHold
        }
    }

    #After hours
    #To be done

    #Holidays
    #To be done

    #If Ivr Mode is enabled:
    if($wfObj.EnableIVRMode)
    {
        #If IVR Mode is enabled, default action is transfer to question
        #Write-Host "DefaultAction: TransferToQuestion"
        $DefaultActionQuestion = CsRgsDefaultCallAction -action "TransferToQuestion" -ivrOptions $wfObj.IvrOptions -ivrMessage $wfObj.IvrMessage -ivrAudioFilePath $wfObj.AudioIvrFilePath -queueId $wfObj.InvoiceQueue.Id -uri $null -textToSpeechPrompt $wfObj.WelcomeMessage -audioFilePath $wfObj.AudioWelcomeFilePath -pool $wfObj.OwnerPool
        $wf.DefaultAction = $DefaultActionQuestion    
    
    }
    #If Ivr Mode is not enabled:
    else
    {
        #If IVR Mode is disabled, default action is tranfers to queue
        if($wfObj.InvoiceQueue.Id)
        {
            #Write-Host "DefaultAction: TransferToQueue"
            $DefaultActionQueue = CsRgsDefaultCallAction -action "TransferToQueue" -ivrOptions $null -ivrMessage $null -ivrAudioFilePath $null -queueId $wfObj.InvoiceQueue.Id -uri $null -textToSpeechPrompt $wfObj.WelcomeMessage -audioFilePath $wfObj.AudioWelcomeFilePath -pool $wfObj.OwnerPool
            $wf.DefaultAction = $DefaultActionQueue        
        } 
    }    
    #>