using System.Data;
using System.Net;
namespace workshop.Models
{
    public class ServerResp
    {
        public string Message{get;set;}
        public HttpStatusCode statusCode{get;set;}
        public string Data{get;set;}
        public string Target{get;set;}

        public string EncodeData(){
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}