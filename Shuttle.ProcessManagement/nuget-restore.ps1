$files = Get-ChildItem -Path . -Recurse -Include packages.config

Write-Host $files

foreach ($file in $files)
{
	Write-Host $file.FullName
	Invoke-Expression "nuget.exe restore $file -SolutionDirectory ."
}