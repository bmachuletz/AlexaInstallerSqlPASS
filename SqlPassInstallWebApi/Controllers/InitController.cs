using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlPassInstallerShared;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.IO;

namespace SqlPassInstallWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InitController : ControllerBase
    {
        private const string MyScriptFolder = @"c:\temp\AlexaInstallASqlServer";

        private readonly ILogger<InitController> _logger;

        public InitController(ILogger<InitController> logger)
        {
            _logger = logger;
        }

        // Executing Powershell from C# with the help of: https://blogs.msdn.microsoft.com/kebab/2014/04/28/executing-powershell-scripts-from-c/
        public AlexaCommand Post([FromBody]AlexaCommand input)
        {
            switch(input.Type)
            {
                case AlexaCommandType.INSTALL_SQLSERVER:
                    using (PowerShell PowerShellInstance = PowerShell.Create())
                    {
                        string functions = System.IO.File.ReadAllText(Path.Combine(MyScriptFolder, "Functions.ps1"));
                        string script = System.IO.File.ReadAllText(Path.Combine(MyScriptFolder, "InstallSqlServer.ps1"));

                        PowerShellInstance.AddScript(functions);
                        PowerShellInstance.AddScript(script);

                        PowerShellInstance.AddParameter("Servername",input.Payload.SqlServerName);
                        PowerShellInstance.AddParameter("IPAddress", input.Payload.IPAddress);
                        PowerShellInstance.AddParameter("InstanceName", input.Payload.InstanceName);

                        Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                    }

                    break;
            }
            return new AlexaCommand { Type = AlexaCommandType.RESULT, Payload = new Payload { ResultText = "Success" } };
        }  
    }
}
