param(
[object]$WhereFilter = $null
)


if ($WhereFilter)
{
    $PropertyName = $WhereFilter.PropertyName
    $PropertyValue = $WhereFilter.PropertyValue

    Get-CsUser | Where-Object {$_.$PropertyName -eq $PropertyValue}
}
else
{
    Get-CsUser
}