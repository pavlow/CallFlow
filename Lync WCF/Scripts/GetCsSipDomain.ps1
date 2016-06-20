Param(
[object]$WhereFilter = $null
)


if ($WhereFilter)
{
    $PropertyName = $WhereFilter.PropertyName
    $PropertyValue = $WhereFilter.PropertyValue

    Get-CsSipDomain | Where-Object {$_.$PropertyName -eq $PropertyValue} | Select-Object Name
}
else
{
   Get-CsSipDomain | Select-Object Name
}