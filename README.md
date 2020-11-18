# Azure Custom Commands Sample
A sample that shows how to integrate Custom Commands in a real application, with support for Wake Word.

The repository consists of:
  * [DeviceControl.json](https://github.com/marcominerva/CustomCommands/blob/master/DeviceControl.json), that contains the Custom Commands definition. You can directly import it in the [Microsoft Speech Portal](https://speech.microsoft.com/portal)
  * [CustomCommandsClient](https://github.com/marcominerva/CustomCommands/tree/master/CustomCommandsClient), a .NET Core WPF application to test Custom Commands interations, both via text and speech. To use it you just need to set the required service keys in the [Constants.cs](https://github.com/marcominerva/CustomCommands/blob/master/CustomCommandsClient/Constants.cs) file
  * [DeviceControl](https://github.com/marcominerva/CustomCommands/tree/master/DeviceControl), an ASP.NET Core Web API project that can be deployed on a Raspeberry Pi and provides methods to control a led connected to the board and get the temperature and humidity using a DHT22 Sensor. Take a look respectively to [LedService.cs](https://github.com/marcominerva/CustomCommands/blob/master/DeviceControl/Services/LedService.cs) and [HumitureService.cs](https://github.com/marcominerva/CustomCommands/blob/master/DeviceControl/Services/HumitureService.cs) files to see Pin configurations.
  
**Contribute**

The project is continuously evolving. We welcome contributions. Feel free to file issues and pull requests on the repo and we'll address them as we can.

**Developer Code of Conduct**

The voice and text understanding capabilities of Custom Commands use Microsoft Cognitive Services. Microsoft will receive the audio and other data that you upload (via this app) for service improvement purposes. To report abuse of the Microsoft Cognitive Services to Microsoft, please visit the Microsoft Cognitive Services website at https://www.microsoft.com/cognitive-services, and use the "Report Abuse" link at the bottom of the page to contact Microsoft. For more information about Microsoft privacy policies please see their privacy statement here: https://go.microsoft.com/fwlink/?LinkId=521839.

This project uses the Microsoft Cognitive Services, see https://www.microsoft.com/cognitive-services. Developers using this project are expected to follow the “Developer Code of Conduct for Microsoft Cognitive Services” at http://go.microsoft.com/fwlink/?LinkId=698895. 
