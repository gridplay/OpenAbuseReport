# OpenAbuseReport
 
Place this inside the source code of opensim in the Addons folder then just add to project and compile.

This is for per simulator instance, NOT for Robust.

OpenSim.ini needs to be configured with the following...

```
[Abuse Reports]
    Enabled = true
    DebugEnabled = false
    ReportURI = "https://yoursite.tdl/location/of/abusereport"
```
Can be at the very bottom of the ini file.

Included is the sql and php but i wrote it for Laravel 10 but with some modding im sure your programmer can get it to work with any framework.

The data is sent from the dll as a POST request

In Laravel set Route::post('abusereport', 'YourController@reports'); then just do use Report; in the controller and referrence the static function
If using Laravel 8+ can put the route in your routes/api.php file and change /location/of/ to just /api/

If NOT using Laravel, the data coming in from the DLL is in json format. $d = json_decode(file_get_contents('php://input'), true);

## Some more stuff
Created by Christopher Strachan of GridPlay Productions. Ven Kellie in the Canadian Grid and Aviworlds. Venkellie Resident in SecondLife.
FIRST grid to ever have this module is the CanadianGrid.ca, Second grid is AviWorlds.

MIT license so dont expect support for your modications to the cs file but improvements are welcome.

## KNOWN ISSUE
Screenshots from the reports dont work. If anyone knows how to save the screenshot texture that be great thank you.

I tried loading the included UUID of the picture using robusturl.tdl:8003/assets/imgUUID but came back as invalid.

