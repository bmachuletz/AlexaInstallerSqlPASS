using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using SqlPassInstallerShared;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SqlPassInstaller
{
    public class Function
    {

        /// <summary>
        /// Lambda-Funktion stellt den eigentlich Skill dar.
        /// Diese Funktion muss als Lambda-Funktion nach AWS hochgeladen werden.
        /// Damit der Skill weiß, wo die Requests hingesendet werden sollen, muss die Variable HTTPS_ENDPOINT
        /// mit einem gültigen Wert konfiguriert werden. In meinem Fall ist das ein durch NGROK-Endpunkt (https://ngrok.com/).
        /// </summary>


        private const string INVOCATION_NAME = "pass system installer";
        private const string HTTPS_ENDPOINT = "http://xxxxx.ngrok.io";
        private const string INSTALL_SQLSERVER_INTENT = "InstallSqlServer";
        private const string STOP_INTENT = "AMAZON.StopIntent";
        private const string CANCEL_INTENT = "AMAZON.CancelIntent";
        private const string HELP_INTENT = "AMAZON.HelpIntent";

        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var requestType = input.GetRequestType();

            if (requestType == typeof(LaunchRequest))
            {
                return HelpMessage();
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = input.Request as IntentRequest;
                switch (intentRequest.Intent.Name)
                {
                    case INSTALL_SQLSERVER_INTENT:
                        context.Logger.LogLine(intentRequest.Intent.Slots["SQLServerName"].Value);
                        SqlPassInstallerShared.AlexaCommand alexaCommand = new AlexaCommand
                        {   
                            Type = AlexaCommandType.INSTALL_SQLSERVER, 
                            Payload = new Payload { 
                                SqlServerName = intentRequest.Intent.Slots["SQLServerName"].Value, 
                                IPAddress = intentRequest.Intent.Slots["IPAddress"].Value.ToString().Replace(",", "."),
                                InstanceName = intentRequest.Intent.Slots["InstanceName"].Value.ToString().Replace(",", ".")
                            } };
                        using (var httpClient = new HttpClient())
                        {

                            string retval = string.Empty;
                            try
                            {
                                HttpContent stringContent = new StringContent(JsonConvert.SerializeObject(alexaCommand));
                                stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                
                                HttpResponseMessage response = await httpClient.PostAsync(HTTPS_ENDPOINT + "/Init", stringContent);
                            }
                            catch (Exception ex)
                            {
                                context.Logger.LogLine(string.Format("Exception: {0}", ex.ToString()));
                            }

                            return MakeSkillResponse($"Ich soll also einen SQL-Server installieren? Dann mache ich das jetzt. Der Server, " + intentRequest.Intent.Slots["SQLServerName"].Value + ", wird jetzt installiert.", true);
                        }
                        
                    case CANCEL_INTENT:
                    case STOP_INTENT:
                        return StopMessage();
                    case HELP_INTENT:
                        return HelpMessage();
                    default:
                        return HelpMessage();
                }
            }
            else
            {
                return MakeSkillResponse($"Da ist was schiefgelaufen.", true);
            }


        }

        private SkillResponse MakeSkillResponse(string outputSpeech,bool shouldEndSession, string repromtText = "")
        {
            var response = new ResponseBody
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = new PlainTextOutputSpeech
                {
                    Text = outputSpeech
                }
            };

            if (repromtText != null)
            {
                response.Reprompt = new Reprompt
                {
                    OutputSpeech = new PlainTextOutputSpeech
                    {
                        Text = repromtText
                    }
                };
            }

            var skillResponse = new SkillResponse
            {
                Response = response,
                Version = "1.0"
            };

            return skillResponse;
        }

        private SkillResponse HelpMessage()
        {
            return MakeSkillResponse($"Hier ist der {INVOCATION_NAME}. " +
              "Es gibt folgende Befehle... " +
              "1. Installiere einen SQL Server... ",
              false);
        }

        private SkillResponse StopMessage()
        {
            return MakeSkillResponse($"Vielen Dank für die Verwendung von " +
              $"{INVOCATION_NAME}.", true);
        }
    }
}
