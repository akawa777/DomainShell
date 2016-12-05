$myDirectory = Split-Path $script:myInvocation.MyCommand.path -parent

$msBuildDirectory = "C:\Windows\Microsoft.NET\Framework\v4.0.30319"

$projects = "DomainShell", "DomainShell.Tests", "DomainShell.Tests.Commerce", "DomainShell.Tests.Commerce.App", "DomainShell.Tests.Commerce.Domain", "DomainShell.Tests.Commerce.Infrastructure"
    

Remove-Item $myDirectory\DomainShell.v12.suo -Force -ErrorAction Ignore -Recurse
Remove-Item $myDirectory\TestResults -Force -ErrorAction Ignore -Recurse

foreach($project in $projects)
{    
    Remove-Item $myDirectory\$project\bin -Force -ErrorAction Ignore -Recurse
    Remove-Item $myDirectory\$project\obj -Force -ErrorAction Ignore -Recurse    
    Remove-Item $myDirectory\$project\*.user -Force -ErrorAction Ignore -Recurse    
}