param(
[object]$WhereFilter = $null
)


if ($WhereFilter)
{
    $PropertyName = $WhereFilter.PropertyName
    $PropertyValue = $WhereFilter.PropertyValue

    Get-CsRgsQueue | Where-Object {$_.$PropertyName -eq $PropertyValue}
}
else
{
    Get-CsRgsQueue
}