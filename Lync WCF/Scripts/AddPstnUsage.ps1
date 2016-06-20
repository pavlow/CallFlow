param (
	[string] $PstnUsageName			# eg: ABC 649 PSTN Usage
)

#Create the PSTN Usage Policy - eg: ABC 649 PSTN Usage
$pstnUsage = (Get-CsPstnUsage -Identity "Global").Usage | where { $_ -eq $PstnUsageName };
if ($pstnUsage -eq $null) {
	Set-CsPstnUsage -Identity "Global" -Usage @{Add=$PstnUsageName};
}