del "%~dp0*.cs"
del "%~dp0Kernels\*.cs"
copy "%~dp0..\DomainShell\*.cs" "%~dp0"
copy "%~dp0..\DomainShell\Kernels\*.cs" "%~dp0Kernels"