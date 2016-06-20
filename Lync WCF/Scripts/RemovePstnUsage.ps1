param (
	[string] $PstnUsageName			# eg: ABC 649 PSTN Usage
)

$pstnUsage = (Get-CsPstnUsage -Identity "Global").Usage | where { $_ -eq $PstnUsageName };
if ($pstnUsage -ne $null) {
	Set-CsPstnUsage -Identity "Global" -Usage @{Remove=$PstnUsageName};
}