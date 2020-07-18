using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace GroupUp
{
    // This Class is a SignalR Hub to communicate ReCaptcha keys with the client-side.
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

            // Send encoded response to google to figure out whether or not it was successful.
            var googleReply = client.DownloadString(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var reCaptcha = serializer.Deserialize<ReCaptcha>(googleReply);

            // return ReCaptcha response to client-side.
            return Clients.Client(Context.ConnectionId).getCaptchaResult(reCaptcha.Success);
        }
        
    }

    // This class is for holding the necessary values for ReCaptcha reading.
    public class ReCaptcha
    {
        public bool Success { get; set; }
        public List<string> ErrorCodes { get; set; }

    }


}