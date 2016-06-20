$Regex1 = '^(?:tel:)?(?:\+)?(\d+)(?:;ext=(\d+))?(?:;([\w-]+))?$'

#Arrays
$AssignedNumbers = @()
$UnassignedNumbers = @()
$CsUnassignedNumbersArray = @()
$NotAssignedUnassignedNumbersArray = @()
$NumberInventory = @()

#Get Users with LineURI
$UsersLineURI = Get-CsUser -Filter {LineURI -ne $Null}
if($UsersLineURI -ne $null)
{
    foreach($item in $UsersLineURI)
    {                  
        $Matches = @()
        $Item.LineURI -match $Regex1 | out-null
            
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineURI" -Value $Item.LineURI
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Matches[1]
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $Matches[2]
        #$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $Item.Name
        #$myObject1 | Add-Member -type NoteProperty -name "FirstName" -Value $Item.FirstName
        #$myObject1 | Add-Member -type NoteProperty -name "LastName" -Value $Item.LastName
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value $Item.DisplayName #($Item.FirstName + " " + $Item.LastName)
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "User"
        $AssignedNumbers += $myObject1          
    }
}

#Get Users with Private Line
$UsersPrivateLine = Get-CsUser -Filter {PrivateLine -ne $Null} 
if($UsersPrivateLine -ne $null)
{
    foreach($item in $UsersPrivateLine)
    {                   
        $Matches = @()
        $Item.PrivateLine -match $Regex1 | out-null
            
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineURI" -Value $Item.PrivateLine
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Matches[1]
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $Matches[2]
        #$myObject1 | Add-Member -type NoteProperty -name "Name" -Value $Item.Name
        #$myObject1 | Add-Member -type NoteProperty -name "FirstName" -Value $Item.FirstName
        #$myObject1 | Add-Member -type NoteProperty -name "LastName" -Value $Item.LastName
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value $Item.DisplayName #($Item.FirstName + " " + $Item.LastName)
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "User Private Line"
        $AssignedNumbers += $myObject1          
    }
}

#Get analouge lines
$AnalougeLineURI = Get-CsAnalogDevice -Filter {LineURI -ne $Null}  
if($AnalougeLineURI -ne $null)
{
    foreach($item in $AnalougeLineURI)
    {                  
        $Matches = @()
        $Item.LineURI -match $Regex1 | out-null
            
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineURI" -Value $Item.LineURI
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Matches[1]
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $Matches[2]
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value $Item.Name
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "Analogue Line"
        $AssignedNumbers += $myObject1          
    }
}

#Get common area phones
$CommonAreaLineURI = Get-CsCommonAreaPhone -Filter {LineURI -ne $Null} 
if($CommonAreaLineURI -ne $null)
{
    foreach($item in $CommonAreaLineURI)
    {                    
        $Matches = @()
        $Item.LineURI -match $Regex1 | out-null
            
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineURI" -Value $Item.LineURI
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Matches[1]
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $Matches[2]
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value $Item.Name
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "Common Area"
        $AssignedNumbers += $myObject1          
    }
}

#Get RGS workflows
$WorkflowLineURI = Get-CsRgsWorkflow | where {$_.LineURI -ne ""} 
if($WorkflowLineURI -ne $null)
{
    foreach($item in $WorkflowLineURI)
    {                 
        $Matches = @()
        $Item.LineURI -match $Regex1 | out-null
            
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineURI" -Value $Item.LineURI
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Matches[1]
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $Matches[2]
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value $Item.Name
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "Workflow"
        $AssignedNumbers += $myObject1          
    }
}

#Get Exchange UM Contacts
$ExUmContactLineURI = Get-CsExUmContact -Filter {LineURI -ne $Null}
if($ExUmContactLineURI -ne $null)
{
    foreach($item in $ExUmContactLineURI)
    {                   
        $Matches = @()
        $Item.LineURI -match $Regex1 | out-null
            
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineURI" -Value $Item.LineURI
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Matches[1]
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $Matches[2]
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value $Item.Name
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "Ex UM Contact"
        $AssignedNumbers += $myObject1          
    }
}

#Get trusted applications
$TrustedApplicationLineURI = Get-CsTrustedApplicationEndpoint -Filter {LineURI -ne $Null}
if($TrustedApplicationLineURI -ne $null)
{
    foreach($item in $TrustedApplicationLineURI)
    {                   
        $Matches = @()
        $Item.LineURI -match $Regex1 | out-null
            
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineURI" -Value $Item.LineURI
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Matches[1]
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $Matches[2]
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value $Item.DisplayName
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "Trusted Endpoint"
        $AssignedNumbers += $myObject1          
    }
}

#Get conferencing numbers
$DialInConfLineURI = Get-CsDialInConferencingAccessNumber -Filter {LineURI -ne $Null}
if($DialInConfLineURI -ne $null)
{
    foreach($Item in $DialInConfLineURI)
    {                 
        $Matches = @()
        $Item.LineURI -match $Regex1 | out-null
            
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineURI" -Value $Item.LineURI
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Matches[1]
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $Matches[2]
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value $Item.DisplayName
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "Dial-in Conf"
        $AssignedNumbers += $myObject1          
    }
}

#Unassigned Numbers Not Assigned

#Get Unassigned Number Ranges
$CsUnassignedNumbers = Get-CsUnassignedNumber

foreach ($CsUnassignedNumber in $CsUnassignedNumbers)
{
    #Build unassigned number list
    foreach ($Number in $CsUnassignedNumber)
    {   
        #Start Range   
        $Matches = @()
        $Number.NumberRangeStart -match $Regex1 | Out-Null
        [int64]$start = $Matches[1] #$Number.NumberRangeStart.Split("+")[1]
        
        #End Range
        $Matches = @()
        $Number.NumberRangeEnd -match $Regex1 | Out-Null
        [int64]$end = $Matches[1] #$Number.NumberRangeEnd.Split("+")[1]

        While ($start -le $end)
        {             
           $UnassignedNumbers += $start        
           $start++
        }
    }
}

foreach ($Number in $UnassignedNumbers)
{   
    
    if ($AssignedNumbers.DDI -notcontains $Number)
    {   
        $myObject1 = New-Object System.Object
        #$myObject1 | Add-Member -type NoteProperty -name "LineUri" -Value $Number
        $myObject1 | Add-Member -type NoteProperty -name "DDI" -Value $Number
        $myObject1 | Add-Member -type NoteProperty -name "Ext" -Value $null
        #$myObject1 | Add-Member -type NoteProperty -name "Name" -Value ""
        $myObject1 | Add-Member -type NoteProperty -name "AssignedTo" -Value "Unassigned"
        #$myObject1 | Add-Member -type NoteProperty -name "LastName" -Value ""
        $myObject1 | Add-Member -type NoteProperty -name "Type" -Value "Unassigned Number"
        $NotAssignedUnassignedNumbersArray += $myObject1
    }

}

$NumberInventory = $AssignedNumbers + $NotAssignedUnassignedNumbersArray
return $NumberInventory