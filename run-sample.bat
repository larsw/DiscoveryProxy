rem ping commands to emulate sleep 500 (ms).
@echo Running sample
@start DiscoveryProxy.Console\bin\Debug\DiscoveryProxy.Console.exe
@ping 123.45.67.89 -n 1 -w 500 > nul
@start Samples\DiscoverableService\bin\Debug\DiscoverableService.exe
@ping 123.45.67.89 -n 1 -w 500 > nul
@start Samples\ClientDiscoveringServiceViaProxy\bin\Debug\ClientDiscoveringServiceViaProxy.exe
