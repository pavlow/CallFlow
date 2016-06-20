param(
$wfObj
)

#cls
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

#--------------------------------------------------------------------------------------------------------------------

#PROMPT - GLOBAL
#New-CsRgsPrompt - prompt 1
$prompt_1 = New-Object PSObject
$prompt_1 | Add-Member -MemberType NoteProperty -Name "AudioFilePrompt" -Value $null #Import-CsRgsAudioFile object
$prompt_1 | Add-Member -MemberType NoteProperty -Name "TextToSpeechPrompt" -Value "This is prompt 1" #String

#--------------------------------------------------------------------------------------------------------------------

##IVR MODE EXAMPLE##

#CALL ACTIONS - SUB1
#New-CsRgsCallAction - queue 1
$action_queue_1 = New-Object PSObject
$action_queue_1 | Add-Member -MemberType NoteProperty -Name "Action" -Value "TransferToQueue"
$action_queue_1 | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $null
$action_queue_1 | Add-Member -MemberType NoteProperty -Name "Question" -Value $null
$action_queue_1 | Add-Member -MemberType NoteProperty -Name "QueueId" -Value "Queue 1"
$action_queue_1 | Add-Member -MemberType NoteProperty -Name "Uri" -Value $null

#New-CsRgsCallAction - queue 2
$action_queue_2 = New-Object PSObject
$action_queue_2 | Add-Member -MemberType NoteProperty -Name "Action" -Value "TransferToQueue"
$action_queue_2 | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $null
$action_queue_2 | Add-Member -MemberType NoteProperty -Name "Question" -Value $null
$action_queue_2 | Add-Member -MemberType NoteProperty -Name "QueueId" -Value "Queue 1"
$action_queue_2 | Add-Member -MemberType NoteProperty -Name "Uri" -Value $null

#ANSWER LIST - SUB1
$answerList_sub1Options = @() 
#New-CsRgsAnswer - to question, DTMF 1
$answer_actionQueue_Dtmf1 = New-Object PSObject
$answer_actionQueue_Dtmf1 | Add-Member -MemberType NoteProperty -Name "Name" -Value "Answer with transfer to queue and DTMF 1" #String
$answer_actionQueue_Dtmf1 | Add-Member -MemberType NoteProperty -Name "Action" -Value $action_queue_1 #New-CsRgsCallAction object
$answer_actionQueue_Dtmf1 | Add-Member -MemberType NoteProperty -Name "DtmfResponse" -Value 1 #Int 0-9
$answer_actionQueue_Dtmf1 | Add-Member -MemberType NoteProperty -Name "VoiceResponseList" -Value "Voice response 1" #String
$answerList_sub1Options += $answer_actionQueue_Dtmf1
#New-CsRgsAnswer - to question, DTMF 2
$answer_actionQueue_Dtmf2 = New-Object PSObject
$answer_actionQueue_Dtmf2 | Add-Member -MemberType NoteProperty -Name "Name" -Value "Answer with transfer to queue and DTMF 2" #String
$answer_actionQueue_Dtmf2 | Add-Member -MemberType NoteProperty -Name "Action" -Value $action_queue_1 #New-CsRgsCallAction object
$answer_actionQueue_Dtmf2 | Add-Member -MemberType NoteProperty -Name "DtmfResponse" -Value 2 #Int 0-9
$answer_actionQueue_Dtmf2| Add-Member -MemberType NoteProperty -Name "VoiceResponseList" -Value "Voice response 2" #String
$answerList_sub1Options += $answer_actionQueue_Dtmf1

#QUESTION - SUB1
#New-CsRgsQuestion - question 1
$question_1 = New-Object PSObject
$question_1 | Add-Member -MemberType NoteProperty -Name "Name" -Value "question_1" #String
$question_1 | Add-Member -MemberType NoteProperty -Name "AnswerList" -Value $answerList_sub1Options #New-CsRgsAnswer object
$question_1 | Add-Member -MemberType NoteProperty -Name "InvalidAnswerPrompt" -Value $null #New-CsRgsPrompt object
$question_1 | Add-Member -MemberType NoteProperty -Name "NoAnswerPrompt" -Value $null #New-CsRgsPrompt object
$question_1 | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $prompt_1 #New-CsRgsPrompt object

#New-CsRgsQuestion - question 2
$question_2 = New-Object PSObject
$question_2 | Add-Member -MemberType NoteProperty -Name "Name" -Value "question_2" #String
$question_2 | Add-Member -MemberType NoteProperty -Name "AnswerList" -Value $answerList_sub1Options #New-CsRgsAnswer object
$question_2 | Add-Member -MemberType NoteProperty -Name "InvalidAnswerPrompt" -Value $null #New-CsRgsPrompt object
$question_2 | Add-Member -MemberType NoteProperty -Name "NoAnswerPrompt" -Value $null #New-CsRgsPrompt object
$question_2 | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $prompt_1 #New-CsRgsPrompt object

#--------------------------------------------------------------------------------------------------------------------
#CALL ACTIONS - DEFAULT (ROOT)
#New-CsRgsCallAction - question 1
$action_question_1 = New-Object PSObject
$action_question_1 | Add-Member -MemberType NoteProperty -Name "Action" -Value "TransferToQuestion" #Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
$action_question_1 | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $null #New-CsRgsPrompt object
$action_question_1 | Add-Member -MemberType NoteProperty -Name "Question" -Value $question_1 #New-CsRgsQuestion object
$action_question_1 | Add-Member -MemberType NoteProperty -Name "QueueId" -Value $null #String QueueID
$action_question_1 | Add-Member -MemberType NoteProperty -Name "Uri" -Value $null #String Uri

#New-CsRgsCallAction - question 2
$action_question_2 = New-Object PSObject
$action_question_2 | Add-Member -MemberType NoteProperty -Name "Action" -Value "TransferToQuestion" #Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
$action_question_2 | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $null #New-CsRgsPrompt object
$action_question_2 | Add-Member -MemberType NoteProperty -Name "Question" -Value $question_2 #New-CsRgsQuestion object
$action_question_2 | Add-Member -MemberType NoteProperty -Name "QueueId" -Value $null #String QueueID
$action_question_2 | Add-Member -MemberType NoteProperty -Name "Uri" -Value $null #String Uri

#ANSWER LIST - DEFAULT (ROOT)
$answerList_defaultRootOptions = @() 
#New-CsRgsAnswer - to question, DTMF 1
$answer_actionQuestion_Dtmf1 = New-Object PSObject
$answer_actionQuestion_Dtmf1 | Add-Member -MemberType NoteProperty -Name "Name" -Value "Answer with transfer to question and DTMF 1" #String
$answer_actionQuestion_Dtmf1 | Add-Member -MemberType NoteProperty -Name "Action" -Value $action_question_1 #New-CsRgsCallAction object
$answer_actionQuestion_Dtmf1 | Add-Member -MemberType NoteProperty -Name "DtmfResponse" -Value 1 #Int 0-9
$answer_actionQuestion_Dtmf1 | Add-Member -MemberType NoteProperty -Name "VoiceResponseList" -Value "Voice response 1" #String
$answerList_defaultRootOptions += $answer_actionQuestion_Dtmf1
#New-CsRgsAnswer - to question, DTMF 2
$answer_actionQuestion_Dtmf2 = New-Object PSObject
$answer_actionQuestion_Dtmf2 | Add-Member -MemberType NoteProperty -Name "Name" -Value "Answer with transfer to question and DTMF 2" #String
$answer_actionQuestion_Dtmf2 | Add-Member -MemberType NoteProperty -Name "Action" -Value $action_question_1 #New-CsRgsCallAction object
$answer_actionQuestion_Dtmf2 | Add-Member -MemberType NoteProperty -Name "DtmfResponse" -Value 2 #Int 0-9
$answer_actionQuestion_Dtmf2| Add-Member -MemberType NoteProperty -Name "VoiceResponseList" -Value "Voice response 2" #String
$answerList_defaultRootOptions += $answer_actionQuestion_Dtmf2

#QUESTION - DEFAULT
#New-CsRgsQuestion - Question for the default action on the workflow
$question_default = New-Object PSObject
$question_default | Add-Member -MemberType NoteProperty -Name "Name" -Value "question_default" #String
$question_default | Add-Member -MemberType NoteProperty -Name "AnswerList" -Value $answerList_defaultRootOptions #New-CsRgsAnswer object
$question_default | Add-Member -MemberType NoteProperty -Name "InvalidAnswerPrompt" -Value $null #New-CsRgsPrompt object
$question_default | Add-Member -MemberType NoteProperty -Name "NoAnswerPrompt" -Value $null #New-CsRgsPrompt object
$question_default | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $prompt_1 #New-CsRgsPrompt object

#CALL ACTIONS - DEFAULT
#New-CsRgsCallAction - Default action to question - Entry point on call other RGS flow commands
$action_defaultQuestion = New-Object PSObject
$action_defaultQuestion | Add-Member -MemberType NoteProperty -Name "Action" -Value "TransferToQuestion" #Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
$action_defaultQuestion | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $null #New-CsRgsPrompt object (this is equivilent to the Welcome Action)
$action_defaultQuestion | Add-Member -MemberType NoteProperty -Name "Question" -Value $question_default #New-CsRgsQuestion object
$action_defaultQuestion | Add-Member -MemberType NoteProperty -Name "QueueId" -Value $null #String QueueID
$action_defaultQuestion | Add-Member -MemberType NoteProperty -Name "Uri" -Value $null #String Uri

#--------------------------------------------------------------------------------------------------------------------

##BASIC EXAMPLE (NO IVR)##

#CALL ACTIONS - DEFAULT
#New-CsRgsCallAction - Default action to question - Entry point on call other RGS flow commands
$action_defaultQueue = New-Object PSObject
$action_defaultQueue | Add-Member -MemberType NoteProperty -Name "Action" -Value "TransferToQueue" #Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
$action_defaultQueue | Add-Member -MemberType NoteProperty -Name "Prompt" -Value $null #New-CsRgsPrompt object (this is equivilent to the Welcome Action)
$action_defaultQueue | Add-Member -MemberType NoteProperty -Name "Question" -Value $null #New-CsRgsQuestion object
$action_defaultQueue | Add-Member -MemberType NoteProperty -Name "QueueId" -Value (Get-CsQueue -Name "Queue 1").Identity #String QueueID
$action_defaultQueue | Add-Member -MemberType NoteProperty -Name "Uri" -Value $null #String Uri

#--------------------------------------------------------------------------------------------------------------------

#Stuff that relates to IVR or Queue

#Needs to be added in C#:
$wfObj | Add-Member -MemberType NoteProperty -Name "Queue" -Value "Queue 5"

#Welcome Message = DefaultAction prompt
$wfObj | Add-Member -MemberType NoteProperty -Name "welcomeMessage" -Value "This is a test welcome message from the script"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioWelcome" -Value "TestAudio.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioWelcomeFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio.wma"

$wfObj | Add-Member -MemberType NoteProperty -Name "enableIvrMode" -Value $true


#IVR
$wfObj | Add-Member -MemberType NoteProperty -Name "ivrMessage" -Value "This is a test IVR message from the script"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioIvr" -Value "TestAudio.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioIvrFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio.wma"

#--------------------------------------------------------------------------------------------------------------------

#WORKFLOW GENERAL PROPERTIES
$wfObj = New-Object PSObject
$wfObj | Add-Member -MemberType NoteProperty -Name "ownerPool" -Value "CFM-S4BFE01.callflowmanager.nz"
$wfObj | Add-Member -MemberType NoteProperty -Name "name" -Value "Script Test 1"
$wfObj | Add-Member -MemberType NoteProperty -Name "description" -Value "This is a test descript from the script"
#Need to add to C#???
$wfObj | Add-Member -MemberType NoteProperty -Name "number" -Value "tel:+6497997777"
$wfObj | Add-Member -MemberType NoteProperty -Name "displayNumber" -Value "+64 9 799 7777"
#$wfObj | Add-Member -MemberType NoteProperty -Name "sipAddress" -Value ""
$wfObj | Add-Member -MemberType NoteProperty -Name "uri" -Value "scripttest1"
$wfObj | Add-Member -MemberType NoteProperty -Name "sipDomain" -Value "callflowmanager.nz"

#Needs to be added in C#:
$wfObj | Add-Member -MemberType NoteProperty -Name "enableWf" -Value $true
$wfObj | Add-Member -MemberType NoteProperty -Name "enableAgentAnonymity" -Value $true
$wfObj | Add-Member -MemberType NoteProperty -Name "enableForFederation" -Value $true
$wfObj | Add-Member -MemberType NoteProperty -Name "language" -Value "en-AU"
$wfObj | Add-Member -MemberType NoteProperty -Name "timeZone" -Value "Samoa Standard Time"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioHoldMusic" -Value "TestAudio.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioHoldMusicFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio.wma"

#AFTER HOURS
#$wfObj | Add-Member -MemberType NoteProperty -Name "businessHourDestination" -Value ""
$wfObj | Add-Member -MemberType NoteProperty -Name "afterHoursDestination" -Value ""
$wfObj | Add-Member -MemberType NoteProperty -Name "afterHoursMessage" -Value ""
$wfObj | Add-Member -MemberType NoteProperty -Name "afterHoursUri" -Value ""
$wfObj | Add-Member -MemberType NoteProperty -Name "afterHoursSipDomain" -Value ""
$wfObj | Add-Member -MemberType NoteProperty -Name "audioAfterHours" -Value "TestAudio.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioAfterHoursFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio.wma"

#HOLIDAYS
$wfObj | Add-Member -MemberType NoteProperty -Name "holidayDestination" -Value "TransferToUri"
$wfObj | Add-Member -MemberType NoteProperty -Name "holidayMessage" -Value "This is a test holiday message from the script"
$wfObj | Add-Member -MemberType NoteProperty -Name "holidayUri" -Value "holidayuser"
$wfObj | Add-Member -MemberType NoteProperty -Name "holidaySipDomain" -Value "callflowmanager.nz"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioHolidays" -Value "TestAudio.wma"
$wfObj | Add-Member -MemberType NoteProperty -Name "audioHolidaysFilePath" -Value "E:\Code\Andrew\RGSFiles\TestAudio.wma"

#--------------------------------------------------------------------------------------------------------------------

##LOGIC#####################:::

#Build Workflow SIP Address Uri
$wfSipAddress =$null
if($wfObj.uri -ne "" -and $wfObj.sipDomain -ne "")
{            
    $wfSipAddress = "sip:" + $wfObj.uri + "@" + $wfObj.sipDomain 
}

#Build After Hours Uri
$afterHoursUri =$null
if($wfObj.afterHoursUri -ne "" -and $wfObj.afterHoursSipDomain -ne "")
{            
    $afterHoursUri = "sip:" + $wfObj.afterHoursUri + "@" + $wfObj.afterHoursSipDomain 
}

#Build Holiday Uri
$holidayUri =$null
if($wfObj.holidayUri -ne "" -and $wfObj.holidaySipDomain -ne "")
{            
    $holidayUri = "sip:" + $wfObj.holidayUri + "@" + $wfObj.holidaySipDomain 
}

Function CsRgsAudioFile ($pool, $audioFilePath)
{
    $fileName = $audioFilePath.Substring($audioFilePath.LastIndexOf("\") + 1)
    #Write-Host "Audio File - $pool, $audioFilePath, $fileName"
    return Import-CsRgsAudioFile -Identity "service:ApplicationServer:$pool" -FileName $fileName -Content (Get-Content $audioFilePath -Encoding byte -ReadCount 0) -Verbose
}

Function CsRgsPrompt($textToSpeechPrompt, $audioFilePath, $pool)
{
    if ($pool -and $audioFilePath)
    {
        #Write-Host "csRgsAudioFile - $pool, $audioFilePath"
        $csRgsAudioFile = CsRgsAudioFile $pool $audioFilePath
        #Write-Host "Test: $csRgsAudioFile"
    }
    
    if ($csRgsAudioFile -and $textToSpeechPrompt)
    {
        #Write-Host "Audio and Text to Speech Prompt"
        return New-CsRgsPrompt -AudioFilePrompt $csRgsAudioFile -TextToSpeechPrompt $textToSpeechPrompt -Verbose
    }
    elseif ($csRgsAudioFile)
    {
       #Write-Host "Audio Only Prompt"
       return New-CsRgsPrompt -AudioFilePrompt $csRgsAudioFile -Verbose
    }
    elseif($textToSpeechPrompt)
    {
        #Write-Host "Text to Speech Only Prompt"
        return New-CsRgsPrompt -TextToSpeechPrompt $textToSpeechPrompt -Verbose
    }
    else
    {
     return $null
    }
}

Function CsRgsQuestion ($questionObj)
{
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



            #Object Depth

}

Function CsRgsCallAction($action, $questionObj, $queue, $uri, $textToSpeechPrompt, $audioFilePath, $pool)
{
    #Create Prompt
    if($textToSpeechPrompt -or ($audioFilePath -and $pool))
    {
        #Write-Host "Create Prompt - $textToSpeechPrompt, $audioFilePath, $pool"
        $prompt = CsRgsPrompt $textToSpeechPrompt $audioFilePath $pool   
    }

    #Create Question - $question will need to be an object
    if($questionObj)
    {
        #questionObj is an object containing nested question/answer combinations
        #Write-Host "Create Question - $questionObj"
        $question = CsRgsQuestion $questionObj

    }
    #Get Queue
    if ($queue)
    {
        #TEST THIS!!!!!!!!!!!!!!!!!!!
        #Write-Host "Create Queue - $queue"
        $queueId = (Get-CsRgsQueue -Name $queue).Identity
    }    

    ## -Action: Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
    #Write-Host "Action: $action"
    if($action -eq "Terminate")
    {
        if ($prompt)
        {
            #Write-Host "$action with prompt"
            New-CsRgsCallAction -Action $action -Prompt $prompt -Verbose
        }
        else
        {        
            #Write-Host "$action without prompt"
            New-CsRgsCallAction -Action $action -Verbose
        }
    }
    if($action -eq "TransferToQuestion")
    {    
        if($prompt -and $question)
        {
            #Write-Host "$action with prompt"
            New-CsRgsCallAction -Action $action -Prompt $prompt -Question $question -Verbose
        }
        elseif($question)
        {        
            #Write-Host "$action without prompt"
            New-CsRgsCallAction -Action $action -Question $question -Verbose
        }
    }
    if($action -eq "TransferToQueue")
    {
        #Write-Host "Action is TransferToQueue"
        if($prompt -and $queueId)
        {
            #Write-Host "$action with prompt"
            New-CsRgsCallAction -Action $action -Prompt $prompt -QueueID $queueId -Verbose
        }
        elseif($queueId)
        {        
            #Write-Host "$action without prompt"
            New-CsRgsCallAction -Action $action -QueueID $queueId -Verbose
        }
    }
    if($action -eq "TransferToUri" -or $action -eq "TransferToVoiceMailUri" -or $action -eq "TransferToPSTN")
    {
        if($prompt -and $uri)
        {
            #Write-Host "$action with prompt"
            New-CsRgsCallAction -Action $action -Prompt $prompt -Uri $uri -Verbose
        }
        elseif($uri)
        {        
            #Write-Host "$action without prompt"
            New-CsRgsCallAction -Action $action -Uri $uri -Verbose
        }
    }
}

#$x = Get-CsRgsWorkflow -Name "wf 1"
#$x.DefaultAction

#Get existing workflow if it exisits
$wf = Get-CsRgsWorkflow | Where {$_.Name -eq $wfObj.Name}

    #Write-Host "New"
    $wf = New-CsRgsWorkflow -Parent $wfObj.OwnerPool `
    -Name $wfObj.Name `
    -Description $wfObj.Description `
    -PrimaryUri $wfSipAddress `
    -DisplayNumber $wfObj.displayNumber `
    -LineUri $wfObj.number`
    -Active $wfObj.enableWf `
    -EnabledForFederation $wfObj.enableForFederation `
    -Language $wfObj.language `
    -TimeZone $wfObj.timeZone `
    -Anonymous $wfObj.enableAgentAnonymity `
    -InMemory -Verbose
    
    #CsRgsCallAction($action, $questionObj, $queue, $uri, $textToSpeechPrompt, $audioFilePath, $pool)
    if($wfObj.ivrMode)
    {
        #If IVR Mode is enabled, default action is transfer to question
    
		#Get a single IVR
$wf = Get-CsRgsWorkflow -name "IVR 1"
$wf.



    #Inside a Question is an AnswerList which holds all the valid resonses
foreach ($ans in $ivr1.DefaultAction.Question.AnswerList)
{
    $ans 
}

    }
    else
    {
        #If IVR Mode is disabled, default action is tranfers to queue
        #Write-Host "DefaultAction: TransferToQueue"
        #$x = CsRgsCallAction -action "TransferToQueue" -questionObj $null -queue $wfObj.Queue -uri $null -textToSpeechPrompt $wfObj.welcomeMessage -audioFilePath $wfObj.audioWelcomeFilePath -pool $wfObj.ownerPool
        #$wf.DefaultAction = $x
        $wf.DefaultAction = CsRgsCallAction -action "TransferToQueue" -questionObj $null -queue $wfObj.Queue -uri $null -textToSpeechPrompt $wfObj.welcomeMessage -audioFilePath $wfObj.audioWelcomeFilePath -pool $wfObj.ownerPool
        
        
    }    
    
    <#
    -DefaultAction $wfObj. `
    -CustomMusicOnHoldFile $wfObj. `
    -BusinessHoursID $wfObj. `
    -NonBusinessHoursAction $wfObj.`
    -HolidaySetIDList $wfObj. `
    -HolidayAction $wfObj.
    -Managed $wfObj. `
    -ManagersByUri $wfObj. ``
    #>
    #$wf
    Set-CsRgsWorkflow -Instance $wf   
<#
}

#If exisits set:
if ($wf)
{
    #$wf.Identity = 
    #$wf.NonBusinessHoursAction = 
    #$wf.HolidayAction = 
    #$wf.DefaultAction = 
    $wf.CustomMusicOnHoldFile = CsRgsAudioFile $wfObj.ownerPool, $wfObj.audioHoldMusicFilePath
    $wf.Name = $wfObj.Name
    $wf.Description = $wfObj.Description
    $wf.PrimaryUri = $wfObj.PrimaryUri
    $wf.Active = $wfObj.Active
    $wf.Language = $wfObj.Language
    $wf.TimeZone = $wfObj.TimeZone
    $wf.BusinessHoursID = $wfObj.BusinessHoursID
    $wf.Anonymous = $wfObj.Anonymous
    #$wf.Managed = $wfObj.Managed
    $wf.OwnerPool = $wfObj.OwnerPool
    $wf.DisplayNumber = $wfObj.DisplayNumber
    $wf.EnabledForFederation = $wfObj.EnabledForFederation
    $wf.LineUri = $wfObj.LineUri
    $wf.HolidaySetIDList = $wfObj.HolidaySetIDList
    #$wf.ManagersByUri = $wfObj.ManagersByUri

    Set-CsRgsWorkflow -Instance $wf
}
#If not exists create new
else
{
#>






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