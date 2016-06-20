using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using CallFlowManager.UI.ViewModels.BusinessHours;
using CallFlowManager.UI.ViewModels.Groups;
using CallFlowManager.UI.ViewModels.Holidays;
using CallFlowManager.UI.ViewModels.Queues;
using CallFlowManager.UI.ViewModels.WorkFlows;

namespace CallFlowManager.UI.Business
{
    internal class WorkflowsSetService
    {
        /// <summary>
        /// Prepares set command by creating a PS object that will be passed to the script file
        /// http://stackoverflow.com/questions/12472718/pass-collection-of-c-sharp-psobjects-to-powershell-pipeline
        /// </summary>
        public PSObject PrepareSetCsRgsWorkflow(
            //WORKFLOW GENERAL PROPERTIES
            string ownerPool,
            string name,
            string description,
            string number,
            string displayNumber,
            string sipAddress,
            string uri,
            string sipDomain,
            bool enableWorkflow, //this needs to be added to form etc
            bool enableAgentAnonymity,
            bool enableForFederation,
            string language,
            string timeZone,
            string audioHoldMusic, //prob dont need this, full file path OK
            string audioHoldMusicFilePath,
            //AFTER HOURS
            BusinessHourGroupViewModel businessHoursGroup, //not sure if this is required
            HolidayGroupViewModel afterHoursGroup, //Name of after hours group
            string businessHourDestination, //not sure about this one
            string afterHoursDestination,
            string afterHoursMessage,
            string afterHoursUri,
            string afterHoursSipDomain,
            string audioAfterHours, //prob dont need this, full file path OK
            string audioAfterHoursFilePath,
            //HOLIDAYS
            HolidayGroupViewModel holidayGroup, //Name of holidays group
            string holidayDestination,
            string holidayMessage,
            string holidayUri,
            string holidaySipDomain,
            string audioHolidays, //prob dont need this, full file path OK
            string audioHolidaysFilePath,

            //ACTIONS - DEFAULT ACTION (Equates to Welcome Message on the UI)
            string audioWelcome, //prob dont need this, full file path OK
            string audioWelcomeFilePath,
            string welcomeMessage,

            //Determine default action type - Queue or Question
            QueueViewModel queue, //If queue selected, TransferToQueue - Not sure which is correct queue or queues
            ObservableCollection<QueueViewModel> queues,

            bool enableIvrMode,
            string audioIvr,
            string audioIvrFilePath,
            string ivrMessage,
            ObservableCollection<IvrViewModel> ivrOptions
            )
        {

//POWERSHELL OBJECT STRUCTURE:::            
            //#New-CsRgsCallAction
            //       # -Action: Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
            //       # -Prompt(played before transfer,created using New-CsRgsPrompt) - 
            //           #New-CsRgsPrompt
            //           # -AudioFilePrompt(audio to play, created using Import-CsRgsAudioFile): Import-CsRgsAudioFile -Identity "service:ApplicationServer:<pool>" -FileName "audio.wav" -Content (Get-Content C:\audio.wav -Encoding byte -ReadCount 0)
            //           # -TextToSpeechPrompt(text to be read as speech): "bla bla bla"
            //       # -Question(question to be asked if Action is TransferToQuestion, created using New-CsRgsQuestion):
            //           #New-CsRgsQuestion
            //           # -Name
            //           # -AnswerList(array of valid voice or DTMF inputs, created using New-CsRgsAnswer) -
            //               #New-CsRgsAnswer
            //               # -Name
            //               # -Action(created using New-CsRgsCallAction): Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn
            //               # -DtmfResponse(Dtmf key): 0-9
            //               # -VoiceResponseList(comma seperated list of valid voice responses): e.g Hardware, Devices             
            //           # -InvalidAnswerPrompt(message to play when input is invalid - after playing prompt is played again, created using New-CsRgsPrompt): Example above
            //           # -NoAnswerPrompt(message to play when no input recieved, created using New-CsRgsPrompt): Example above
            //           # -Prompt(question to be asked, created using New-CsRgsPrompt): Example above
            //       # -QueueID(Queue ID to transfer call to when Action is TransferToQueue): (Get-CsRgsQueue - Name "bla").Identity
            //       # -Uri(Uri to transfer to when Action is TransferToUri; TransferToVoiceMailUri; or TransferToPSTN): sip:<user|number>@<sipdomain.com>

            //C# OBJECT STRUCTURE::  
            // DefaultAction
            PSObject wfDefaultActionObj = new PSObject();

            //IVR Mode OFF
            if (enableIvrMode == false)
            {
                //If IVRMode is false, then DefaultAction is TransferToQueue - could look at adding other options later Terminate, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn 
                //Create DefaultAction
                wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAction", "TransferToQueue"));
                wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQueueId", queue));
                    //QueueId - String QueueID - TransferToQueue
                wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionUri", null));
                    //Uri - String Uri - TransferToVoicemailUri/TransferToUri
                wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQuestion", null));
                    //Question - New-CsRgsQuestion object - TransferToQuestion::

                //Welcome Message
                wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAudioFilePrompt", audioWelcomeFilePath));
                    //Prompt - New-CsRgsPrompt object (this is equivilent to the Welcome Action), imported using Import-CsRgsAudioFile object
                wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionTextToSpeechPrompt", welcomeMessage));
                    //String
            }

            //IVR Mode ON
            if (enableIvrMode == true)
            {
                //Answer
                PSObject wfAnswerObj = new PSObject();
                
                // DefaultAction.Question.AnswerList - list of root ivr objects
                List<PSObject> answerList = new List<PSObject>();

                // DefaultAction.Question
                PSObject wfQuestionObj = new PSObject();

                wfDefaultActionObj.Properties.Add(new PSNoteProperty("Question", wfQuestionObj));

                // include collection of answerlist to DefaultAction.Question
                wfQuestionObj.Properties.Add(new PSNoteProperty("AnswerList", answerList));

                //Foreach IVR root option
                foreach (var ivrOption in ivrOptions)
                {
                    wfAnswerObj.Properties.Add(new PSNoteProperty("VoiceResponseList", ivrOption.Name.Split(',').ToArray()));
                    wfAnswerObj.Properties.Add(new PSNoteProperty("DtmfResponse", ivrOption.Number));

                    // DefaultAction.Question.AnswerList[i].Action.Action
                    PSObject ivrActionObj = new PSObject();
                    wfAnswerObj.Properties.Add(new PSNoteProperty("Action", ivrActionObj));
                    ivrActionObj.Properties.Add(new PSNoteProperty("Action", "TransferToQuestion"));

                    // DefaultAction.Question.AnswerList[i].Action.Question
                    PSObject ivrQuestionObj = new PSObject();
                    ivrActionObj.Properties.Add(new PSNoteProperty("Question", ivrQuestionObj));

                    // DefaultAction.Question.AnswerList[i].Action.Question.AnswerList
                    List<PSObject> ivrAnswerList = new List<PSObject>();
                    ivrQuestionObj.Properties.Add(new PSNoteProperty("AnswerList", ivrAnswerList));

                    foreach (var childIvrNode in ivrOption.ChildIvrNodes)
                    {
                        PSObject ivrAnswerObj = new PSObject();

                        ivrAnswerObj.Properties.Add(new PSNoteProperty("Name", childIvrNode.Name));
                        ivrAnswerObj.Properties.Add(new PSNoteProperty("DtmfResponse", childIvrNode.Number));
                        ivrAnswerList.Add(ivrAnswerObj);
                    }

                    // Write DefaultAction.Question.AnswerList[i].Action.Action = "TransferToQuestion"
                    
                    answerList.Add(wfAnswerObj);

                    ////Answer List
                        //////wfQuestionObj.Properties.Add(new PSNoteProperty("questionAnswerList", wfAnswerObj));

                    //////If the IVR root option has no child nodes, then it must have an action of TransferToQueue
                    ////if (ivrOption.ChildIvrNodes.Count == 0)
                    ////{
                    ////    //Create DefaultAction
                    ////    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAction", "TransferToQueue"));
                    ////    //Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn  
                    ////    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQueueId", ivrOption.InvoiceQueue));
                    ////    //QueueId - String QueueID - TransferToQueue
                    ////    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionUri", null));
                    ////    //Uri - String Uri - TransferToVoicemailUri/TransferToUri
                    ////    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQuestion", null));

                    ////    //Prompt - used to create prompt in PowerShell
                    ////    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAudioFilePrompt",
                    ////        ivrOption.AudioIvrTreeFilePath));
                    ////    //Prompt - New-CsRgsPrompt object (this is equivilent to the Welcome Action), imported using Import-CsRgsAudioFile object
                    ////    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionTextToSpeechPrompt",
                    ////        ivrOption.AudioIvrTree)); //String

                    ////}

                    //If the IVR root option has at least 1 child node, then it must have an action of TransferToQuestion
                    if (ivrOption.ChildIvrNodes.Count > 0)
                    {
                        //Question
                        ////PSObject wfQuestionObj = new PSObject();
                        ////wfDefaultActionObj.Properties.Add(new PSNoteProperty("Question", wfQuestionObj));



                        ////foreach (var childIvr in ivrOption.ChildIvrNodes)
                        ////{
                        ////    wfAnswerObj.Properties.Add(new PSNoteProperty("questionAnswerList", wfAnswerObj)); 
                        ////}
                        ////Answer - New-CsRgsAnswer object
                        //// TODO: There can be multiple answers, need to put this is a loop 
                        //wfAnswerObj.Properties.Add(new PSNoteProperty("answerName", "CfmAnswerName")); //String
                        //wfAnswerObj.Properties.Add(new PSNoteProperty("answerDtmfResponse", ivrOption.Number));
                        ////Int 0-9
                        //wfAnswerObj.Properties.Add(new PSNoteProperty("answerVoiceResponseList", ivrOption.Name));
                        ////String - comma seperated list of voice commands
                        //wfAnswerObj.Properties.Add(new PSNoteProperty("answerAction", "??"));

                        ////New-CsRgsCallAction object::
                        ////Action - New-CsRgsCallAction object

                        ////Question
                        //PSObject wfQuestionObj = new PSObject();
                        ////Question - New-CsRgsQuestion object - TransferToQuestion
                        //wfQuestionObj.Properties.Add(new PSNoteProperty("questionName", "CfmQuestionName")); //String
                        //wfQuestionObj.Properties.Add(new PSNoteProperty("questionInvalidAnswerPrompt", null));
                        ////Prompt - New-CsRgsPrompt object::
                        //wfQuestionObj.Properties.Add(new PSNoteProperty("questionNoAnswerPrompt", null));
                        ////Prompt - New-CsRgsPrompt object::

                        ////Prompt to play before action:
                        //wfQuestionObj.Properties.Add(new PSNoteProperty("questionPromptAudioFile", ""));
                        ////Prompt - New-CsRgsPrompt object::
                        //wfQuestionObj.Properties.Add(new PSNoteProperty("questionPromptTextToSpeech", ""));
                        ////Prompt - New-CsRgsPrompt object::
                        


                        ////Create DefaultAction
                        ////if IVR mode is ON, the action has to be TransferToQuestion
                        ////Create DefaultAction
                        //wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAction", "TransferToQuestion"));
                        ////Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn  
                        //wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQueueId", null));
                        ////QueueId - String QueueID - TransferToQueue
                        //wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionUri", null));
                        ////Uri - String Uri - TransferToVoicemailUri/TransferToUri
                        //wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQuestion", wfQuestionObj));

                        ////Welcome Message
                        //wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAudioFilePrompt",
                        //    audioWelcomeFilePath));
                        ////Prompt - New-CsRgsPrompt object (this is equivilent to the Welcome Action), imported using Import-CsRgsAudioFile object
                        //wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionTextToSpeechPrompt", welcomeMessage));
                        ////String

                    }

                }
            }

            //Root Object - WORKFLOW GENERAL PROPERTIES, AFTER HOURS, HOLIDAYS, DEFAULT ACTION
                //Need to populate all the base object properties here...
                //WORKFLOW GENERAL PROPERTIES
                PSObject wfBaseObj = new PSObject();
                wfBaseObj.Properties.Add(new PSNoteProperty("ownerPool", ownerPool));
                wfBaseObj.Properties.Add(new PSNoteProperty("name", name));
                wfBaseObj.Properties.Add(new PSNoteProperty("description", description));
                wfBaseObj.Properties.Add(new PSNoteProperty("number", number));
                wfBaseObj.Properties.Add(new PSNoteProperty("displayNumber", displayNumber));
                wfBaseObj.Properties.Add(new PSNoteProperty("sipAddress", sipAddress));
                wfBaseObj.Properties.Add(new PSNoteProperty("sipDomain", sipDomain));
                wfBaseObj.Properties.Add(new PSNoteProperty("enableWorkflow", enableWorkflow));
                wfBaseObj.Properties.Add(new PSNoteProperty("enableAgentAnonymity", enableAgentAnonymity));
                wfBaseObj.Properties.Add(new PSNoteProperty("enableForFederation", enableForFederation));
                wfBaseObj.Properties.Add(new PSNoteProperty("language", language));
                wfBaseObj.Properties.Add(new PSNoteProperty("timeZone", timeZone));
                //wfBaseObj.Properties.Add(new PSNoteProperty("audioHoldMusic", audioHoldMusic));
                wfBaseObj.Properties.Add(new PSNoteProperty("audioHoldMusicFilePath", audioHoldMusicFilePath));
                //AFTER HOURS
                wfBaseObj.Properties.Add(new PSNoteProperty("businessHoursGroup", businessHoursGroup));
                wfBaseObj.Properties.Add(new PSNoteProperty("afterHoursGroup", afterHoursGroup));
                wfBaseObj.Properties.Add(new PSNoteProperty("businessHourDestination", businessHourDestination));
                wfBaseObj.Properties.Add(new PSNoteProperty("afterHoursDestination", afterHoursDestination));
                wfBaseObj.Properties.Add(new PSNoteProperty("afterHoursMessage", afterHoursMessage));
                wfBaseObj.Properties.Add(new PSNoteProperty("afterHoursUri", afterHoursUri));
                wfBaseObj.Properties.Add(new PSNoteProperty("afterHoursSipDomain", afterHoursSipDomain));
                wfBaseObj.Properties.Add(new PSNoteProperty("audioAfterHours", audioAfterHours));
                wfBaseObj.Properties.Add(new PSNoteProperty("audioAfterHoursFilePath", audioAfterHoursFilePath));
                //HOLIDAYS
                wfBaseObj.Properties.Add(new PSNoteProperty("holidayGroup", holidayGroup));
                wfBaseObj.Properties.Add(new PSNoteProperty("holidayDestination", holidayDestination));
                wfBaseObj.Properties.Add(new PSNoteProperty("holidayMessage", holidayMessage));
                wfBaseObj.Properties.Add(new PSNoteProperty("holidayUri", holidayUri));
                wfBaseObj.Properties.Add(new PSNoteProperty("holidaySipDomain", holidaySipDomain));
                //wfBaseObj.Properties.Add(new PSNoteProperty("audioHolidays", audioHolidays));
                wfBaseObj.Properties.Add(new PSNoteProperty("audioHolidaysFilePath", audioHolidaysFilePath));
                wfBaseObj.Properties.Add(new PSNoteProperty("audioAfterHours", audioAfterHours));
                wfBaseObj.Properties.Add(new PSNoteProperty("audioAfterHoursFilePath", audioAfterHoursFilePath));
                wfBaseObj.Properties.Add(new PSNoteProperty("defaultAction", wfDefaultActionObj));





                return wfBaseObj;
        }
    }
}

        ////OBJECT STRUCTURE
        //    //Sub1 Object - DEFAULT ACTION
        //    PSObject wfDefaultActionObj = new PSObject();
        //    //Action - New-CsRgsCallAction
        //    ////Need to determine what the action is -
        //    if (enableIvrMode = true)
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAction", "TransferToQuestion")); //Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn   

        //    if (queue != null)
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQueueId", queue)); //QueueId - String QueueID - TransferToQueue

        //    if (uri != null)
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionUri", "")); //Uri - String Uri - TransferToVoicemailUri/TransferToUri
        //    //if ("TransferToQuestion"")
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQuestion", "")); //Question - New-CsRgsQuestion object - TransferToQuestion::
                    
        //        //Question - New-CsRgsQuestion object - TransferToQuestion
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionName", "CfmQuestionName")); //String
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionAnswerList", ""));
                        
        //            //Answer - New-CsRgsAnswer object
        //            wfDefaultActionObj.Properties.Add(new PSNoteProperty("answerName", "CfmAnswerName")); //String
        //            wfDefaultActionObj.Properties.Add(new PSNoteProperty("answerDtmfResponse", "")); //Int 0-9
        //            wfDefaultActionObj.Properties.Add(new PSNoteProperty("answerVoiceResponseList", "")); //String - comma seperated list of voice commands
        //            wfDefaultActionObj.Properties.Add(new PSNoteProperty("answerAction", "")); //New-CsRgsCallAction object::
        //                //Action - New-CsRgsCallAction object
                        
                        
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionInvalidAnswerPrompt", "")); //Prompt - New-CsRgsPrompt object::
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionNoAnswerPrompt", "")); //Prompt - New-CsRgsPrompt object::
        //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionPrompt", "")); //Prompt - New-CsRgsPrompt object::

                    
        //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAudioFilePrompt", "")); //Prompt - New-CsRgsPrompt object (this is equivilent to the Welcome Action), imported using Import-CsRgsAudioFile object
        //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionTextToSpeechPrompt", "")); //String


    //WORK IN PROGRESS - These need to be put in there correct places above to build the object -----------------------------------------------------------------------------------              
            //    //Question
            //    PSObject wfQuestionObj2 = new PSObject();
            //    //Question - New-CsRgsQuestion object - TransferToQuestion
            //    wfQuestionObj2.Properties.Add(new PSNoteProperty("questionName", "CfmQuestionName")); //String

            //    wfQuestionObj2.Properties.Add(new PSNoteProperty("questionInvalidAnswerPrompt", null));
            //    //Prompt - New-CsRgsPrompt object::
            //    wfQuestionObj2.Properties.Add(new PSNoteProperty("questionNoAnswerPrompt", null));
            //    //Prompt - New-CsRgsPrompt object::
            //    //Prompt to play before action:
            //    wfQuestionObj2.Properties.Add(new PSNoteProperty("questionPromptAudioFile", ""));
            //    //Prompt - New-CsRgsPrompt object::
            //    wfQuestionObj2.Properties.Add(new PSNoteProperty("questionPromptTextToSpeech", ""));
            //    //Prompt - New-CsRgsPrompt object::
            //    //Answer List
            //    wfQuestionObj2.Properties.Add(new PSNoteProperty("questionAnswerList", wfAnswerObj));

                
                
            //    //if IVR mode is ON, the action has to be TransferToQuestion
            //    //Create DefaultAction
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAction", "TransferToQuestion"));
            //    //Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn  
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQueueId", null));
            //    //QueueId - String QueueID - TransferToQueue
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionUri", null));
            //    //Uri - String Uri - TransferToVoicemailUri/TransferToUri
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQuestion", wfQuestionObj));

            //    //Welcome Message
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAudioFilePrompt", audioWelcomeFilePath));
            //    //Prompt - New-CsRgsPrompt object (this is equivilent to the Welcome Action), imported using Import-CsRgsAudioFile object
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionTextToSpeechPrompt", welcomeMessage)); //String
                

            //}


            ////Sub1 Object - DEFAULT ACTION
            ////PSObject wfDefaultActionObj = new PSObject();
            ////Action - New-CsRgsCallAction
            //////Need to determine what the action is - if IVR mode is ON, the action has to be TransferToQuestion
            //if (enableIvrMode = true)
            //{
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAction", "TransferToQuestion")); //Terminate, TransferToQuestion, TransferToVoicemailUri, TransferToUri, TransferToQueue, TransferToPstn  
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQueueId", null)); //QueueId - String QueueID - TransferToQueue
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionUri", null)); //Uri - String Uri - TransferToVoicemailUri/TransferToUri
            //}

            //if (enableIvrMode = false)
            //{
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAction", "TransferToQueue"));
                
            //    if (queue != null)
            //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQueueId", queue)); //QueueId - String QueueID - TransferToQueue

            //    if (uri != null)
            //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionUri", "")); //Uri - String Uri - TransferToVoicemailUri/TransferToUri
            //}
            ////if ("TransferToQuestion"")
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionQuestion", "")); //Question - New-CsRgsQuestion object - TransferToQuestion::
                    
            //    //Question - New-CsRgsQuestion object - TransferToQuestion
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionName", "CfmQuestionName")); //String
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionAnswerList", ""));
                        
            //        //Answer - New-CsRgsAnswer object
            //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("answerName", "CfmAnswerName")); //String
            //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("answerDtmfResponse", "")); //Int 0-9
            //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("answerVoiceResponseList", "")); //String - comma seperated list of voice commands
            //        wfDefaultActionObj.Properties.Add(new PSNoteProperty("answerAction", "")); //New-CsRgsCallAction object::
            //            //Action - New-CsRgsCallAction object
                        
                        
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionInvalidAnswerPrompt", "")); //Prompt - New-CsRgsPrompt object::
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionNoAnswerPrompt", "")); //Prompt - New-CsRgsPrompt object::
            //    wfDefaultActionObj.Properties.Add(new PSNoteProperty("questionPrompt", "")); //Prompt - New-CsRgsPrompt object::

                    
            //wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionAudioFilePrompt", "")); //Prompt - New-CsRgsPrompt object (this is equivilent to the Welcome Action), imported using Import-CsRgsAudioFile object
            //wfDefaultActionObj.Properties.Add(new PSNoteProperty("actionTextToSpeechPrompt", "")); //String

//--------------------------------------------------------------------------------------------------------------------------------------------------------------