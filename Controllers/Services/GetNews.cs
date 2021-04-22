using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Data;
using workshop.DatabaseModel;
namespace workshop.Controllers.Services
{
    public class GetNews
    {
        DatabaseModel.DatabaseConnect db = new DatabaseConnect ();
        Services.EmailAddress mail = new EmailAddress ();

        public Models.ServerResp DeleteItem(int id){
            try{
            DataTable getdetail = db.Query("select * from News where id='"+id+"'");
            if(getdetail.Rows.Count==0){
                return new Models.ServerResp{
                    Message="ไม่พบข้อมูลที่ต้องการ",
                    statusCode = System.Net.HttpStatusCode.BadRequest,
                    Target = db.configTarget
                };
            }

            if(getdetail.Rows[0]["status"].ToString()!="0"){
                return new Models.ServerResp{
                    statusCode = System.Net.HttpStatusCode.BadRequest,
                    Message="ไม่สามารถลบรายการที่ออกเอกสารแล้วได้",
                    Target = db.configTarget
                };
            }
            db.Execute("delete from News where id='"+id+"'");
         
            return new Models.ServerResp{
                statusCode = System.Net.HttpStatusCode.OK,
                Target = db.configTarget
            };
            }catch(Exception er){
                return new Models.ServerResp{
                   statusCode = System.Net.HttpStatusCode.BadRequest,
                   Message = er.Message.ToString() ,
                   Target = db.configTarget
                };
            }
        }
        public Models.ServerResp GetNewsRead(){
            Models.ServerResp service = new Models.ServerResp ();
            try{
            Models.DataTableString dt = (Models.DataTableString) db.Query("select * from news");    

            service.statusCode = System.Net.HttpStatusCode.OK;
            service.Data = dt.GetStringItem();
            service.Target = db.configTarget;
            return service;
            }catch(Exception er){
                service .Message = er.Message.ToString();
                service.statusCode = System.Net.HttpStatusCode.BadRequest;
                service.Target = db.configTarget;
                return service;
            }

        }

        public dynamic GetNewsRead(string id){
            Models.ServerResp service = new Models.ServerResp ();
            try{
            Models.DataTableString  dt = (Models.DataTableString ) db.Query("select * from news where id='"+id+"'");        
            service.statusCode = System.Net.HttpStatusCode.OK;
            service.Data =dt.GetStringItem();
            service.Target = db.configTarget;
            return service;
            }catch(Exception er){
                service .Message = er.Message.ToString();
                service.statusCode = System.Net.HttpStatusCode.BadRequest;
                service.Target = db.configTarget;
                return service;
            }

        }

        public dynamic AddNews(Models.RegisterNews input){
            Models.ServerResp resp = new Models.ServerResp ();
            try{
                int status = 1;
              var state = mail.SendEmail(new Models.AsEmailInput{
                            Title="คุณมีการมอบหมายเอกสารที่ต้องรับผิดชอบเรื่อง : "+input.Title,
                            Detail = input.Detail,
                            To = input.To
                        });
                        if(state.statusSendder==Models.EmailStatus.TypeStatus.Error){
                            status = 0;
                        }

            db.Execute("insert into News(title,Email,Detail,Tel,status) values('"+input.Title+"','"+input.To+"','"+input.Detail+"','"+status+"')");
            resp.statusCode =System.Net.HttpStatusCode.OK;
            resp.Target  = db.configTarget;
            }catch (Exception ER){
            resp.Message = ER.Message.ToString();
            resp.statusCode = System.Net.HttpStatusCode.BadRequest;    
            resp.Target  = db.configTarget;

            }
            return resp;
        }

        public Models.ServerResp Confirm(Models.RegisterNews input,int cstate,int id){
           try{
            if(cstate==0){//Reject
            DataTable searchProfileItem = db.Query("select * from News where id='"+id+"'");
            if(searchProfileItem.Rows.Count>0){
               var status =  mail.SendEmail(new Models.AsEmailInput{
                To="manosit68@gmail.com",
                Title="ระบบได้ทำการยกเลิกรายการ "+searchProfileItem.Rows[0]["Title"].ToString(),
                Detail="รายการนี้ถูก Reject ออกจากระบบแล้ว",
                
            });
            if(status.statusSendder==Models.EmailStatus.TypeStatus.Complete){
            db.Execute("delete from News where id='"+id+"'");
            }else{
                return new Models.ServerResp{
                    statusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = status.errorMessage.ToString(),
                    Target = db.configTarget
                };
            }

            }else{
                return new Models.ServerResp{
                    statusCode = System.Net.HttpStatusCode.BadRequest,
                    Message ="ไม่พบรายการที่ต้องการ Reject ในขณะนี้",
                    Target = db.configTarget
                };
            }
            }else if(cstate==1 || cstate==2){ //Confirm
               
                DataTable getprofile = db.Query("select * from news where id='"+id+"'");
                if(getprofile.Rows.Count>0){
                    if(cstate==1){//Confirm
                    var statusEmail = mail.SendEmail(new Models.AsEmailInput{
                        Title="มีการตอบคำถามของรายการ "+getprofile.Rows[0]["Title"].ToString(),
                        To  = "manosit68@gmail.com",
                        Detail = "รายการถูกดำเนินการแล้ว"
                    });
                    SMS sms = new SMS ();
                    var smsdetail = sms.SendSMS(input.Detail,input.Tel);
                    if(statusEmail.statusSendder==Models.EmailStatus.TypeStatus.Error){
                        return new Models.ServerResp{
                            statusCode = System.Net.HttpStatusCode.BadRequest,
                            Message = "ไม่สามารถส่งอีเมล์ได้เนื่องจาก : "+statusEmail.errorMessage,
                            Target = db.configTarget
                        };
                    }

                        if(smsdetail.statusSendder==Models.EmailStatus.TypeStatus.Error){
                            return new Models.ServerResp{
                                Message="ไม่สามารถส่ง SMS ได้เนืองจาก : "+smsdetail.errorMessage,
                                statusCode = System.Net.HttpStatusCode.BadRequest,
                                Target = db.configTarget
                            };
                        }

                    db.Execute("update news set status=1,MessageResp='"+input.Detail+"' where id='"+id+"");
                    }else{ //Re ass
                        var state = mail.SendEmail(new Models.AsEmailInput{
                            Title="คุณมีการมอบหมายเอกสารที่ต้องรับผิดชอบเรื่อง : "+input.Title,
                            Detail = input.Detail,
                            To = input.To
                        });
                    if(state.statusSendder==Models.EmailStatus.TypeStatus.Error){
                        return new Models.ServerResp{
                            statusCode = System.Net.HttpStatusCode.OK,
                            Message = state.errorMessage,
                            Target = db.configTarget
                        };
                    }
                        db.Execute("update news set status=2,Detail='"+input.Detail+"',Email='"+input.To+"' where id='"+id+"'");
                    }
                }
            }
            return new Models.ServerResp{
                statusCode = System.Net.HttpStatusCode.OK,
                Target = db.configTarget
            };
           }catch(Exception er){
               return new Models.ServerResp{
                   Message = er.Message.ToString(),
                   statusCode = System.Net.HttpStatusCode.OK,
                   Target = db.configTarget
               };
           }
        }

        
        public Models.ServerResp UpdateNews(string id,Models.RegisterNews input){
        try{
        DataTable getitem = db.Query("select * from News where id='"+id+"'");
        if(getitem.Rows.Count>0){
              var state = mail.SendEmail(new Models.AsEmailInput{
                            Title="เอกสาร : "+input.Title + " มีการแก้ไขใหม่",
                            Detail = input.Detail,
                            To = input.To
                        });
                        if(state.statusSendder==Models.EmailStatus.TypeStatus.Error){
                            return new Models.ServerResp{
                                Message = state.errorMessage,
                                statusCode = System.Net.HttpStatusCode.BadRequest
                            };
                        }
            db.Execute("update News set Title='"+input.Title+"',Detail='"+input.Detail+"',Email='"+input.To+"' where id='"+id+"'");
        }else{
            return new Models.ServerResp{
                Message="ไม่พบรายการที่ต้องการ",
                statusCode = System.Net.HttpStatusCode.BadRequest,
                Target = db.configTarget
            };
        }
        return new Models.ServerResp{
            statusCode = System.Net.HttpStatusCode.OK,
            Target = db.configTarget
        };
        }catch(Exception er ){
            return new Models.ServerResp{
                Message = er.Message.ToString(),
                statusCode = System.Net.HttpStatusCode.OK,
                Target = db.configTarget
            };
        }
        }
    }
}