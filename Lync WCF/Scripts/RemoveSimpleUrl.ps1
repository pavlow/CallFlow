param (
	[string] $Component,
	[string] $Domain
)

$simpleUrl = (Get-CsSimpleUrlConfiguration).SimpleUrl | where { $_.Component -ieq $Component -and $_.Domain -ieq $Domain };

if ($simpleUrl) {
	Set-CsSimpleUrlConfiguration -Identity "Global" -SimpleUrl @{Remove = $simpleUrl};
}