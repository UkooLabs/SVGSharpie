param (
    [Parameter(Mandatory=$true)][string]$user_profile,
    [Parameter(Mandatory=$true)][string]$github_access_token,
    [Parameter(Mandatory=$true)][string]$github_email,
    [Parameter(Mandatory=$true)][string]$github_user
)

Write-Host "Processing Documents:"

git config --global credential.helper store
Add-Content "$user_profile\.git-credentials" "https://$($github_access_token):x-oauth-basic@github.com`n"
git config --global user.email $github_email
git config --global user.name $github_user
