using System;
using System.Collections.Generic;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.Assistant.v2;
using IBM.Watson.Assistant.v2.Model;
using Newtonsoft.Json;
using WatsonAssistant.Models;

namespace WatsonAssistant.Services
{
    public class IBMWatsonAssistant
    {
        string sessionId;
        public void CreateSession()
        {
            IamAuthenticator authenticator = new IamAuthenticator(apikey: Keys.apikey);

            AssistantService service = new AssistantService(Keys.versionDate, authenticator);
            service.SetServiceUrl(Keys.url);

            var result = service.CreateSession(assistantId: Keys.assistantId);
            sessionId = result.Result.SessionId;
        }

        public void DeleteSession()
        {
            IamAuthenticator authenticator = new IamAuthenticator(apikey: Keys.apikey);

            AssistantService service = new AssistantService(Keys.versionDate, authenticator);
            service.SetServiceUrl(Keys.url);

            var result = service.DeleteSession(
                assistantId: Keys.assistantId,
                sessionId: sessionId
                );

            Console.WriteLine("Intencion 3: " + result.Response);
        }

        public WatsonMessage Message(string input)
        {
            IamAuthenticator authenticator = new IamAuthenticator(
                apikey: Keys.apikey);

            AssistantService service = new AssistantService(Keys.versionDate, authenticator);
            service.SetServiceUrl(Keys.url);

            var result = service.Message(
                assistantId: Keys.assistantId,
                sessionId: sessionId,
                input: new MessageInput()
                {
                    Text = input
                }
                );

            Console.WriteLine("Intencion 1: " + result.Response);
            return JsonConvert.DeserializeObject<WatsonMessage>(result.Response);

        }

        public void MessageWithContext()
        {
            IamAuthenticator authenticator = new IamAuthenticator(
                apikey: Keys.apikey);

            AssistantService service = new AssistantService(Keys.versionDate, authenticator);
            service.SetServiceUrl(Keys.url);

            MessageContextSkills skills = new MessageContextSkills();
            MessageContextSkill skill = new MessageContextSkill();
            Dictionary<string, object> userDefinedDictionary = new Dictionary<string, object>();

            userDefinedDictionary.Add("account_number", "123456");
            skill.UserDefined = userDefinedDictionary;
            skills.Add("main skill", skill);

            var result = service.Message(
                assistantId: Keys.assistantId,
                sessionId: sessionId,
                input: new MessageInput()
                {
                    Text = "Hello"
                },
                context: new MessageContext()
                {
                    Global = new MessageContextGlobal()
                    {
                        System = new MessageContextGlobalSystem()
                        {
                            UserId = "my_user_id"
                        }
                    },
                    Skills = skills
                }
                );

            Console.WriteLine("Intencion 2: " + result.Response);
        }

        public void MessageStateless()
        {
            IamAuthenticator authenticator = new IamAuthenticator(apikey: Keys.apikey);

            AssistantService service = new AssistantService(Keys.versionDate, authenticator);
            service.SetServiceUrl(Keys.url);

            var result = service.MessageStateless(
                assistantId: Keys.assistantId,
                input: new MessageInputStateless()
                {
                    Text = "Hola"
                }
                );

            Console.WriteLine(result.Response);
        }

    }
}
