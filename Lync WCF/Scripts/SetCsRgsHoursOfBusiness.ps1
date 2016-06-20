param(
$BusHoursObj
)

<#
cls
$openCloseObj = @()
$obj1 = New-Object PSObject
$obj1 | Add-Member -MemberType NoteProperty -Name "DayOfWeek" -Value "Monday"
$obj1 | Add-Member -MemberType NoteProperty -Name "OpenTime1" -Value "08:21:00"
$obj1 | Add-Member -MemberType NoteProperty -Name "CloseTime1" -Value "11:22:00"
$obj1 | Add-Member -MemberType NoteProperty -Name "OpenTime2" -Value "12:00:00"
$obj1 | Add-Member -MemberType NoteProperty -Name "CloseTime2" -Value "17:00:00"
$obj1 | Add-Member -MemberType NoteProperty -Name "OpenCloseTime1Enabled" -Value $true
$obj1 | Add-Member -MemberType NoteProperty -Name "OpenCloseTime2Enabled" -Value $false
$openCloseObj += $obj1

$obj1 = New-Object PSObject
$obj1 | Add-Member -MemberType NoteProperty -Name "DayOfWeek" -Value "Tuesday"
$obj1 | Add-Member -MemberType NoteProperty -Name "OpenTime1" -Value "08:10:00"
$obj1 | Add-Member -MemberType NoteProperty -Name "CloseTime1" -Value "11:10:00"
$obj1 | Add-Member -MemberType NoteProperty -Name "OpenTime2" -Value "12:00:00"
$obj1 | Add-Member -MemberType NoteProperty -Name "CloseTime2" -Value "17:00:00"
$obj1 | Add-Member -MemberType NoteProperty -Name "OpenCloseTime1Enabled" -Value $true
$obj1 | Add-Member -MemberType NoteProperty -Name "OpenCloseTime2Enabled" -Value $false
$openCloseObj += $obj1


$busHoursObj = New-Object PSObject
$busHoursObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "Test3"
$busHoursObj | Add-Member -MemberType NoteProperty -Name "OwnerPool" -Value "S4BFE01.ucgeek.nz"
$busHoursObj | Add-Member -MemberType NoteProperty -Name "OpenCloseTimes" -Value $openCloseObj
#>
Start-Transcript -Path "C:\Logs\Log_SetCsRgsHoursOfBusiness.log"
#$busHoursObj

#Write-Host "BEFORE-------------------------------------------------------------------------------"
#$HoursOfBusiness

foreach ($OpenCloseTimes in $BusHoursObj.OpenCloseTimes)
{
    #Write-Host "Loop"
    if ($OpenCloseTimes.OpenCloseTime1Enabled)
    {      
        $TimeRange1 = New-CsRgsTimeRange -Name "CallFlowManagerTimeRange" -OpenTime $OpenCloseTimes.OpenTime1.ToString() -CloseTime $OpenCloseTimes.CloseTime1.ToString()
    }
    if ($OpenCloseTimes.OpenCloseTime2Enabled)
    {
        $TimeRange2 = New-CsRgsTimeRange -Name "CallFlowManagerTimeRange" -OpenTime $OpenCloseTimes.OpenTime2.ToString() -CloseTime $OpenCloseTimes.CloseTime2.ToString()
    }
    if (!$($OpenCloseTimes.OpenCloseTime1Enabled))
    {      
        $TimeRange1 = $null
    }
    if (!$($OpenCloseTimes.OpenCloseTime2Enabled))
    {
        $TimeRange2 = $null
    }

    if ($OpenCloseTimes.DayOfWeek -eq "Monday")
    {
        $MondayHours1 = $TimeRange1
        $MondayHours2 = $TimeRange2           
    }

    if ($OpenCloseTimes.DayOfWeek -eq "Tuesday")
    {
        $TuesdayHours1 = $TimeRange1
        $TuesdayHours2 = $TimeRange2            
    } 

    if ($OpenCloseTimes.DayOfWeek -eq "Wednesday")
    {
        $WednesdayHours1 = $TimeRange1
        $WednesdayHours2 = $TimeRange2            
    }      
    
    if ($OpenCloseTimes.DayOfWeek -eq "Thursday")
    {
        $ThursdayHours1 = $TimeRange1
        $ThursdayHours2 = $TimeRange2            
    }     
    
    
    if ($OpenCloseTimes.DayOfWeek -eq "Friday")
    {
        $FridayHours1 = $TimeRange1
        $FridayHours2 = $TimeRange2            
    } 
    
    if ($OpenCloseTimes.DayOfWeek -eq "Saturday")
    {
        $SaturdayHours1 = $TimeRange1
        $SaturdayHours2 = $TimeRange2            
    } 
    
    if ($OpenCloseTimes.DayOfWeek -eq "Sunday")
    {
        $SundayHours1 = $TimeRange1
        $SundayHours2 = $TimeRange2            
    }       
}


$HoursOfBusiness = Get-CsRgsHoursOfBusiness | Where {$_.Name -eq $BusHoursObj.Name}

#If exisits set:
if ($HoursOfBusiness)
{
    $HoursOfBusiness.MondayHours1 = $MondayHours1
    $HoursOfBusiness.MondayHours2 = $MondayHours2
    $HoursOfBusiness.TuesdayHours1 = $TuesdayHours1         
    $HoursOfBusiness.TuesdayHours2 = $TuesdayHours2   
    $HoursOfBusiness.WednesdayHours1 = $WednesdayHours1
    $HoursOfBusiness.WednesdayHours2 = $WednesdayHours2
    $HoursOfBusiness.ThursdayHours1 = $ThursdayHours1
   $HoursOfBusiness.ThursdayHours2 = $ThursdayHours2
    $HoursOfBusiness.FridayHours1 = $FridayHours1
    $HoursOfBusiness.FridayHours2 = $FridayHours2
    $HoursOfBusiness.SaturdayHours1 = $SaturdayHours1
    $HoursOfBusiness.SaturdayHours2 = $SaturdayHours2
    $HoursOfBusiness.SundayHours1 = $SundayHours1
    $HoursOfBusiness.SundayHours2 = $SundayHours2

    Set-CsRgsHoursOfBusiness -Instance $HoursOfBusiness  
    $HoursOfBusiness
}
#If not exists create new
else
{
    New-CsRgsHoursOfBusiness -Name $BusHoursObj.Name -Parent $BusHoursObj.OwnerPool `
    -MondayHours1 $MondayHours1 `
    -MondayHours2 $MondayHours2 `
    -TuesdayHours1 $TuesdayHours1 `
    -TuesdayHours2 $TuesdayHours2 `
    -WednesdayHours1 $WednesdayHours1 `
    -WednesdayHours2 $WednesdayHours2 `
    -ThursdayHours1 $ThursdayHours1 `
    -ThursdayHours2 $ThursdayHours2 `
    -FridayHours1 $FridayHours1 `
    -FridayHours2 $FridayHours2 `
    -SaturdayHours1 $SaturdayHours1 `
    -SaturdayHours2 $SaturdayHours2 `
    -SundayHours1 $SundayHours1 `
    -SundayHours2 $SundayHours2
}

Stop-Transcript
#.\Set-CsRgsAgentGroup.ps1 -Name "Script_Set" -Description "From script" -ParticipationPolicy "Formal" -AgentAlertTime "20" -RoutingMethod "Serial" -AgentsByUri "sip:u1@ucgeek.nz"