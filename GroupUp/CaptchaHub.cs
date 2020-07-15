using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace GroupUp
{
    public class CaptchaHub : Hub
    {
        public Task HandleResponse(string response)
        {
            var client = new System.Net.WebClient();
            var secret = "6Lev1LEZAAAAAKMA3oBbO8PGKyD-lwj4IsMrWB-s";

            if (string.IsNullOrEmpty(secret))
            {
                Clients.Client(Context.ConnectionId).getCaptchaResult(false);
            }

            var googleReply = client.DownloadString(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var reCaptcha = serializer.Deserialize<ReCaptcha>(googleReply);

            return Clients.Client(Context.ConnectionId).getCaptchaResult(reCaptcha.Success);
        }
        
    }

    public class ReCaptcha
    {
        public bool Success { get; set; }
        public List<string> ErrorCodes { get; set; }

    }


}