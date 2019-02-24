# executeMXS
tiny util that helps to execute maxscript function by MXSEditor command shortcuts

<br/>
<h3>#usage</h3>

In order to use it we first need to add new command to <b>MXS_EditorUser.properties</b><br/>
for this example we will add Ctrl+8 combination what will execute <b>myStruct.MyFunction()</b><br/><br/>
  command.quiet.8.\*.ms=1<br/>
  command.subsystem.8.\*.ms=2<br/>
  command.name.8.\*= My command name<br/>
  command.8.\*="C:\yourpath\executeMxs.exe" myStruct.MyFunction $(FilePath)<br/>

<br/><br/>
  Now in MXSEditor Tools menu you should be able to see 'My command name' command binded to Ctrl+8 combination
  
  
