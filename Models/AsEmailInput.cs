using System.Net;
using System.Collections.Generic;
namespace workshop.Models
{
    public class AsEmailInput
    {
        public string Title{get;set;}
        public string Detail{get;set;}
        public string To{get;set;}
        public string CC{get;set;}
        public string Display{get;set;}
        public List<string>FileName{get;set;}
    }

    public class EmailStatus{
        public enum TypeStatus{
            Complete = 0,
            Error = 1
        }
        public TypeStatus statusSendder{get;set;}
        public string errorMessage{get;set;}
    }
}