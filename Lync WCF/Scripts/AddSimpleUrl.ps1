param (
	[string] $Component,
	[string] $Domain,
	[string] $Url
)

$existingSimpleUrl = (Get-CsSimpleUrlConfiguration).SimpleUrl | where { $_.Component -ieq $Component -and $_.Domain -ieq $Domain };
$isUrlUpdate = $existingSimpleUrl -ne $null -and $existingSimpleUrl.ActiveUrl -ine $Url;

if (-not $existingSimpleUrl -or $isUrlUpdate) {
	$urlEntry = New-CsSimpleUrlEntry -Url $Url;
	$newSimpleUrl = New-CsSimpleUrl -Component $Component -SimpleUrl $urlEntry -ActiveUrl $Url -Domain $Domain;

    $config = @{Add = $newSimpleUrl};
    if ($isUrlUpdate) {
        $config["Remove"] = $existingSimpleUrl;
    }

	Set-CsSimpleUrlConfiguration -Identity "Global" -SimpleUrl $config;
}