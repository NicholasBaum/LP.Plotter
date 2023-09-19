param (
    [Parameter(Mandatory=$true)]
    [string]$FolderPath
)

# Get the full path of the folder
$FolderPath = Convert-Path -Path $FolderPath

# Create full path of output file
$OutputFilePath = Join-Path -Path $FolderPath -ChildPath "files_index.txt"

# Get all files within the folder and its subfolders and filter the outputfile itself if it already exists
$Files = Get-ChildItem -Path $FolderPath -File -Recurse | Where-Object { $_.FullName -ne $OutputFilePath }

# Create an array to store the relative paths
$RelativePaths = @()

# Loop through each file and get the relative path
foreach ($File in $Files) {
    $RelativePath = $File.FullName.Replace($FolderPath, '')
    $RelativePaths += $RelativePath
}

# Write the relative paths to a file
$RelativePaths | Out-File -FilePath $OutputFilePath

# Output the file path where the relative paths are stored
Write-Host "Relative paths are saved to: $OutputFilePath"
