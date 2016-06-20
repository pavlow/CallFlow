Param(
[object]$WhereFilter = $null
)


if ($WhereFilter)
{
    $PropertyName = $WhereFilter.PropertyName
    $PropertyValue = $WhereFilter.PropertyValue

    Get-CsUnassignedNumber | Where-Object {$_.$PropertyName -eq $PropertyValue}
}
else
{
    Get-CsUnassignedNumber
}