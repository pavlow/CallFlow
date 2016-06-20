param(
[object]$WhereFilter = $null
)

#Object structure to pass from C# where a "where equals" filter is required
#$WhereFilter = New-Object System.Object
#$WhereFilter | Add-Member -type NoteProperty -name "PropertyName" -Value "Name"
#$WhereFilter | Add-Member -type NoteProperty -name "PropertyValue" -Value "Basic Wf 1"			

if ($WhereFilter)
{
    $PropertyName = $WhereFilter.PropertyName
    $PropertyValue = $WhereFilter.PropertyValue

    Get-CsRgsWorkflow | Where-Object {$_.$PropertyName -eq $PropertyValue}
}
else
{
    Get-CsRgsWorkflow
}

