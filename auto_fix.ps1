$ErrorActionPreference = "Stop"

for ($i = 1; $i -le 10; $i++) {
    Write-Host "Iteration $i..."
    
    # Run build and redirect output
    dotnet build > build_errors.txt
    
    # Run python script
    $output = python fix_mappings_v2.py
    Write-Host $output
    
    if ($output -match "Applied fixes to 0 files.") {
        Write-Host "No more fixes applied. Stopping."
        break
    }
}
Write-Host "Done."
