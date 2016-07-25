$projects = "DomainShell", "DomainShell.Extension", "DomainShell.Tests.Domain", "DomainShell.Tests.Web"

$myDirectory = Split-Path $script:myInvocation.MyCommand.path -parent

Remove-Item $myDirectory\$project.v12.suo -Force -ErrorAction Ignore -Recurse

foreach($project in $projects)
{
    Remove-Item $myDirectory\$project\DomainShell.v12.suo -Force -ErrorAction Ignore -Recurse
    Remove-Item $myDirectory\$project\bin -Force -ErrorAction Ignore -Recurse
    Remove-Item $myDirectory\$project\obj -Force -ErrorAction Ignore -Recurse    
}

