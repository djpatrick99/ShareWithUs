using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using ShareWithUs.Models;

namespace ShareWithUs.Controllers
{
    public class MessageController : Controller
    {
        public JsonResult Index()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            string returnValue = "use /Message(Server server)";

            return Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":8, "PosterId":0, "Title":"Dit is de title van de body", "Body":"Dit is de body van de message!!"}' -H "Content-Type: application/json" -X POST http://localhost:8080/message/PostMessage -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult PostMessage(Message message)
        {
            message.Id = -100;

            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "Posted";

            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT * FROM test.ServerMember where idUser=" + message.PosterId + " and idServer=" + message.ServerId + " and userRole>0;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Close();

                    conn.Close();
                    conn.Open();

                    sql = "INSERT INTO test.Message (title, body, idPoster, idServer) VALUES ('" + message.Title + "', '" + message.Body + "', " + message.PosterId + ", " + message.ServerId + ");";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    
                    conn.Close();
                    conn.Open();

                    sql = "SELECT id FROM test.Message where title='" + message.Title + "' and body='" + message.Body + "' and idServer=" + message.ServerId + "  and idPoster=" + message.PosterId + ";";
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            message.Id = (int)rdr[0];
                        }

                        Response.StatusCode = (int)HttpStatusCode.Accepted;
                        rdr.Close();
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        returnValue = "Cannot post message.";
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    returnValue = "Cannot post message.";
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }

            conn.Close();

            return (message.Id > -1) ? Json(message) : Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":8, "PosterId":0, "Title":"Dit is de title van de body", "Body":"Dit is de body van de message!!"}' -H "Content-Type: application/json" -X POST http://localhost:8080/message/PostReplyMessage -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult PostReplyMessage(Message message)
        {
            message.Id = -100;

            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "Posted";

            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT * FROM test.ServerMember where idUser=" + message.PosterId + " and idServer=" + message.ServerId + " and userRole>0;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Close();

                    //conn.Close();
                    //conn.Open();

                    sql = "INSERT INTO test.Message (title, body, idPoster, idServer, idParentMessage) VALUES ('" + message.Title + "', '" + message.Body + "', " + message.PosterId + ", " + message.ServerId + ", " + message.ParentMessageId + ");";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    //conn.Close();
                    //conn.Open();

                    sql = "SELECT id FROM test.Message where title='" + message.Title + 
                                                                               "' and body='" + message.Body + 
                                                                               "' and idServer=" + message.ServerId + 
                                                                               "  and idPoster=" + message.PosterId + 
                                                                               "  and idParentMessage=" + message.ParentMessageId + ";";
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            message.Id = (int)rdr[0];
                            break;
                        }

                        Response.StatusCode = (int)HttpStatusCode.Accepted;
                        rdr.Close();
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        returnValue = "Cannot post message.";
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    returnValue = "Cannot post message.";
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }

            conn.Close();

            return (message.Id > -1) ? Json(message) : Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":8}' -H "Content-Type: application/json" -X POST http://localhost:8080/message/GetMessages -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult GetMessages(Message message)
        {
           // message.Id = -100;

            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "Posted";

            HttpCookie userCookie = Request.Cookies["token"];

            List<Message> messages = new List<Message>();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT * FROM test.Message where idServer=" + message.ServerId + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        Message _message = new Message();

                        _message.Id = (int)rdr[0];
                        _message.Body = (String)rdr[1];
                        _message.Title = (String)rdr[2];
                        if(rdr[4].ToString().CompareTo("") != 0)
                        {
                            _message.ParentMessageId = (int)rdr[4];
                        }
                        else
                        {
                            _message.ParentMessageId = -1;
                        }
                        _message.PosterId = (int)rdr[5];
                        _message.ServerId = (int)rdr[6];

                        messages.Add(_message);
                    }

                    Response.StatusCode = (int)HttpStatusCode.Accepted;
                    rdr.Close();
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    returnValue = "Cannot post message.";
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }

            conn.Close();

            return (messages.Count > 0) ? Json(messages) : Json(returnValue);
        }
    }
}
