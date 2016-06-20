param(
$HolObj
)
$VerbosePreference = "continue"
<#
cls
$holTimesObj = @()
$obj1 = New-Object PSObject
$obj1 | Add-Member -MemberType NoteProperty -Name "Name" -Value "Christmas Day"
$obj1 | Add-Member -MemberType NoteProperty -Name "StartDate" -Value "25/12/2015 12:00:00 a.m."
$obj1 | Add-Member -MemberType NoteProperty -Name "EndDate" -Value "25/12/2015 11:59:59 a.m."
$holTimesObj += $obj1

$obj1 = New-Object PSObject
$obj1 = New-Object PSObject
$obj1 | Add-Member -MemberType NoteProperty -Name "Name" -Value "Boxing Day"
$obj1 | Add-Member -MemberType NoteProperty -Name "StartDate" -Value "26/12/2015 12:00:00 a.m."
$obj1 | Add-Member -MemberType NoteProperty -Name "EndDate" -Value "26/12/2015 11:59:59 a.m."
$holTimesObj += $obj1


$HolObj = New-Object PSObject
$HolObj | Add-Member -MemberType NoteProperty -Name "Name" -Value "Test300"
$HolObj | Add-Member -MemberType NoteProperty -Name "OwnerPool" -Value "CFM-S4BFE02.callflowmanager.nz"
$HolObj | Add-Member -MemberType NoteProperty -Name "HolidayTimes" -Value $holTimesObj
#>
#Start-Transcript -Path "C:\Logs\Log_SetCsRgsHolidaySet.log"

#Create an array of new holiday sets
$HolSetArray = @()
foreach ($HolidayTime in $HolObj.HolidayTimes)
{     
    Write-Verbose "Creating holiday - "
    Write-Verbose "Name: $($HolidayTime.Name)"
    Write-Verbose "StartDate:  $($HolidayTime.StartDate.ToString())"
    Write-Verbose "EndDate: $($HolidayTime.EndDate.ToString())"

    $Holiday = New-CsRgsHoliday -Name $HolidayTime.Name -StartDate $HolidayTime.StartDate.ToString() -EndDate $HolidayTime.EndDate.ToString()
    $HolSetArray += $Holiday
}

#Get existing holiday set if it exisits
Write-Verbose "Checking if holiday group exisits"
$HolidaySet = Get-CsRgsHolidaySet | Where {$_.Name -eq $HolObj.Name}

#If exisits set:
if ($HolidaySet)
{
    if ($HolSetArray.Count -gt 0)
    {
        Write-Verbose "Holiday Group $($HolObj.Name) already exists, updating"      
        $HolidaySet.HolidayList.Clear()  
    
        foreach ($hol in $HolSetArray)
        {
            Write-Verbose "Adding Holiday $($hol.Name) to Holiday Group"
            $HolidaySet.HolidayList.Add($hol)
        }        
    }
    elseif ($HolSetArray.Count -eq 0)
    {
        Write-Verbose "Holiday Group has no holidays, clearing"
        $HolidaySet.HolidayList.Clear()
    }

	Set-CsRgsHolidaySet -Instance $HolidaySet

}
#If not exists create new
else
{
    if ($HolSetArray.Count -gt 0)
    {
        Write-Verbose "Creating Holiday Group $($HolObj.Name)"
        $NewHolidaySet = New-CsRgsHolidaySet -Name $HolObj.Name -Parent $HolObj.OwnerPool -HolidayList $HolSetArray        
    }
}
#>
#Stop-Transcript