### Simple SOCKET-based HTTP server to serve static files form the local folder.
Used for SPA development purposes.

Installation:

Either run:

`dotnet run -- {options?}`

Or install globally:

`dotnet tool install -g spa-server --add-source serve/nupkg`

and then simply:

`spa-server {options?}`

Options:

-h hostname
Examples: netflix.com, google.com
Defaults to localhost.

-p port
Examples: 80, 8080
Defaults to 8080 for localhost and 80 for masks.

--path path
Path to the folder you want to serve.
Defaults to current execution folder

Masksing mode is implemented by altering the /etc/hosts file.
It might need additional settings in Chrome (they do ignore hosts in latest versions with Secure DNS switched on.)
This is still an experiment.
