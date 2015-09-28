$msBuildDirectory = "C:\Windows\Microsoft.NET\Framework\v4.0.30319"

$projects ="DomainShell","DomainShell.Tests"

$netVersions = "v3.5", "v4.0", "v4.5"

$myDirectory = Split-Path $script:myInvocation.MyCommand.path -parent

$releaseDirectory = "ReleaseDlls"

Remove-Item $myDirectory\$releaseDirectory -Force -ErrorAction Ignore -Recurse

foreach($project in $projects)
{    
    $directory = $myDirectory + "\" + $project
    $projectFile = $project + ".csproj";

    if ($project.IndexOf(".Tests") -eq -1)
    {
        foreach($version in $netVersions)
        {
            $versionName = $version.Replace(".", "")

            $text = Get-Content $directory\$projectFile -Raw

            $verText = $text.Replace("<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>", "<TargetFrameworkVersion>" + $version + "</TargetFrameworkVersion>")
            $verText = $verText.Replace("<DebugType>pdbonly</DebugType>", "")    
            $verText = $verText.Replace("<OutputPath>bin\Release\</OutputPath>", "<OutputPath>..\$releaseDirectory\" + $versionName + "\$project</OutputPath>")

            $verProject = $project + "." + $versionName + ".csproj";

            $verText| Out-File $directory\$verProject -Encoding UTF8

            cmd /c $msBuildDirectory\msbuild.exe /p:Configuration=Release $directory\$verProject    

            Remove-Item $directory\$verProject -Force -ErrorAction Ignore        
            Remove-Item $releaseDirectory\$versionName\$project\*.xml -Force -ErrorAction Ignore
            Remove-Item $releaseDirectory\$versionName\$project\*.pdb -Force -ErrorAction Ignore
        }
    }

    Remove-Item $directory\bin -Force -ErrorAction Ignore -Recurse
    Remove-Item $directory\obj -Force -ErrorAction Ignore -Recurse
}

Remove-Item $myDirectory\*.suo -Force -ErrorAction Ignore
Remove-Item $myDirectory\TestResults -Force -ErrorAction Ignore -Recurse

