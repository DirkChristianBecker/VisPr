# VisPr² Core
Contains data types that are used on server, runtime and client side. This includes all requests and responses, the client, server and runtime exchange. As well as the database layout and all datatypes that are stored in the database. Since both runtime and server communicate with the database server independently this seems appropriate (the client always calls Web-Services on the server to query the database).
# Services
Currently only contains the JWTService that is beeing used one server and client side to authenticate requests.
# Datamodel
- Database: Contains all datatypes that are stored inside the database.
- Requests: Contains all requests that are beeing send back and forth between client, server and runtime. Broken up by function
- Responses: Contains alls responses thate are beeing send back after a request. Broken up by function. 

