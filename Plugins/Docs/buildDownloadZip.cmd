
if exist %1-Binary.zip del %1-Binary.zip
if exist %1-Project.zip del %1-Project.zip

rar.exe a -ep -r -x*.pdb %1-Binary.zip ..\%1\bin\Release -x..\%1\bin\Release\ARMPluginInterfaces.dll README-Binary.txt

rar.exe a -ep1 -r -x*.suo -x..\%1\bin -x..\%1\obj -x*\.svn -x*\.svn\* %1-Project.zip ..\%1\*.* README-Project.txt

