# VisPr² Runtime
Represents a single machine that executes desktop operations like reading from UI-Elements, writing to UI-Elements, clicking UI-Elements, starting or terminating applications etc. All these operations are wrapped into Web-Services that are beeing called by the server while executing processes. Each request is beeing authorized against the database.
VisPr² is an ASP.Net application, which makes it effectively a server serving desktop operations in the network. However, for brevity it is called runtime since thats the role it plays within VisPr² infrastructure.

# Examples
Of course the runtime can be used without server and client (i.e. for Ui-Testing). If that is your use case have a look at the file 'VisPr--Runtime-soapui-project.xml'. This is a [Soap-UI-Project](https://www.soapui.org/ "Soap-UI") that has examples for all resources the runtime provides. 
