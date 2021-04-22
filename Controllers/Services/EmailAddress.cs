using System.Xml;
using System.Text;
using System;
using System.Net;
using System.Net.Mail;

namespace workshop.Controllers.Services
{
    public class EmailAddress
    {
        public Models.EmailStatus SendEmail(Models.AsEmailInput input){
    try{
    var fromAddress = new MailAddress("manosit68@gmail.com", "Admin");
    var toAddress = new MailAddress(input.To,input.Display);
    const string fromPassword = "028682226";


    var smtp = new SmtpClient
    {
        Host = "smtp.gmail.com",
        Port = 587,
        EnableSsl = true,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
    };
    using (var message = new MailMessage(fromAddress, toAddress)
    {
        Subject = input.Title,
        Body = input.Detail    

    })
    {
    
    if(input.FileName!=null){
        foreach(var file in input.FileName){
    Attachment fileInput = new Attachment(file);
    message.Attachments.Add(fileInput);
    System.IO.File.Delete(file);
        }
    }
        smtp.Send(message);
    }


                return new Models.EmailStatus{
                    statusSendder = Models.EmailStatus.TypeStatus.Complete
                };

        }catch(Exception er){
            return  new Models.EmailStatus{
                statusSendder = Models.EmailStatus.TypeStatus.Error,
                errorMessage = er.Message.ToString()
            };
        }
            
        }


        }


        public class SMS{
            public Models.EmailStatus SendSMS(string message,string tel){
                using(System.Net.WebClient web = new WebClient ()){
                    System.Collections.Specialized.NameValueCollection val = new System.Collections.Specialized.NameValueCollection ();
                    val.Add("",message);
                    val.Add("",tel);
                    byte[] resp = web.UploadValues("",val);
                    Encoding encode = Encoding.UTF8;
                    string messageStringData = encode.GetString(resp);
                    if(messageStringData.IndexOf("QUEUE")>-1){
                    return new Models.EmailStatus{
                        statusSendder = Models.EmailStatus.TypeStatus.Complete
                    };
                    }else{
                        XmlDocument xml = new XmlDocument ();
                        xml.LoadXml(messageStringData);
                        XmlNode root = xml.FirstChild;
                         XmlNode attr = xml.CreateNode(XmlNodeType.Attribute, "genre", root.GetNamespaceOfPrefix("Detail"));
                         Models.EmailStatus state = new Models.EmailStatus ();
                         state.statusSendder = Models.EmailStatus.TypeStatus.Error;
                         state.errorMessage = attr.Value.ToString();
                         return state; 
                    }
                }
            }
        }
}