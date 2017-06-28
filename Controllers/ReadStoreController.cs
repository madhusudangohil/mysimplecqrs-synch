using CQRS.Common;
using CQRS.ES;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SyncApi.Controllers
{
    public class ReadStoreController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public async Task<HttpResponseMessage> Post()
        {
            string result = await Request.Content.ReadAsStringAsync();
            dynamic message = JObject.Parse(result);
            JObject snsmessage = ((Newtonsoft.Json.Linq.JObject)message);

            var messageType = Request.Headers.Where(x => x.Key == "x-amz-sns-message-type").FirstOrDefault();
            if(messageType.Value.ToArray<String>()[0]  == "SubscriptionConfirmation")
            {
                string url = snsmessage.SelectToken("SubscribeURL").ToString();
                WebRequest request = WebRequest.Create(url);
                try
                {
                    HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
                    if (response.StatusCode == HttpStatusCode.OK)
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                }
                catch
                {
                    return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                }
            }
            else
            {
                var evt = JsonConvert.DeserializeObject<Event>(snsmessage.SelectToken("Message").ToString(), new EventConverter());

                List<Action<Message>> handlers;

                if (!ServiceLocator.Bus._routes.TryGetValue(evt.GetType(), out handlers))
                    return new HttpResponseMessage(HttpStatusCode.BadRequest); 

                foreach (var handler in handlers)
                {
                    handler(evt);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
