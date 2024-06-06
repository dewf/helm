copy /y .\cmake-build-debug\*.dll ..\..\..\client\csharp\AppRunner\bin\Debug\net8.0
robocopy cmake-build-debug\plugins ..\..\..\client\csharp\AppRunner\bin\Debug\net8.0\plugins /e

copy /y .\cmake-build-debug\*.dll ..\..\..\client\csharp\SevenGuisFsharp\bin\Debug\net8.0
robocopy cmake-build-debug\plugins ..\..\..\client\csharp\SevenGuisFsharp\bin\Debug\net8.0\plugins /e

pause



	