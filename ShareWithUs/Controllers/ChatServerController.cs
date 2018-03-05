using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using ShareWithUs.Models;

namespace ShareWithUs.Controllers
{
    public class ChatServerController : Controller
    {
        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/ -i 
        public JsonResult Index()
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";
            List<Server> servers = new List<Models.Server>();

            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT s.* FROM test.Token as t, test.User as u, ServerMember as sa, test.Server as s where t.token like '" + userCookie.Value + "' and t.idUser = u.id and sa.idUser = u.id and sa.idServer = s.id;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Server s = new Server();
                    s.Id = (int)rdr[0];
                    s.Name = (string)rdr[1];
                    servers.Add(s);
                }

                rdr.Close();

                if (servers.Count > 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.Accepted;
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    returnValue = "Unknown error!";
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return (servers.Count() > 0) ? Json(servers) : Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -d '{"Name":"HvA"}' -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/CreateServer -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult CreateServer(Server server)
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";
           
            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT s.* FROM test.Server as s where s.name like '" + server.Name + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (!rdr.HasRows)
                {
                    rdr.Close();

                    sql = "SELECT u.* FROM test.Token as t, test.User as u where t.token like '" + userCookie.Value + "' and t.idUser = u.id;";
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        User u = new User();
                        while (rdr.Read())
                        {
                            u.Id = (int)rdr[0];
                        }

                        rdr.Close();
                        conn.Close();

                        conn.Open();
                        sql = "INSERT test.Server (name) VALUES ('" + server.Name + "')";
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        conn.Open();
                        sql = "SELECT s.* FROM test.Server as s where s.name like '" + server.Name + "';";
                        cmd = new MySqlCommand(sql, conn);
                        rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            server.Id = (int)rdr[0];
                        }

                        conn.Close();
                        conn.Open();
                        sql = "INSERT test.ServerMember (idServer, idUser, userRole) VALUES (" + server.Id + ", " + u.Id + ", 1)";
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        Response.StatusCode = (int)HttpStatusCode.Accepted;
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    returnValue = "Unknown error!";
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return (server.Id > -1) ? Json(server) : Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":2}' -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/JoinServer -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult JoinServer(JoinRequest joinRequest)
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";
          
            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT s.* FROM test.Token as t, test.User as u, ServerMember as sa, test.Server as s where t.token like '" + userCookie.Value + "' and t.idUser = u.id and sa.idUser = u.id and sa.idServer = s.id and s.id=" + joinRequest.ServerId + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if(!rdr.HasRows)
                {
                    rdr.Close();

                    sql = "SELECT u.* FROM test.Token as t, test.User as u where t.token like '" + userCookie.Value + "' and t.idUser = u.id;";
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        User u = new User();
                        while(rdr.Read())
                        {
                            u.Id = (int)rdr[0];
                        }

                        rdr.Close();

                        conn.Close();
                        conn.Open();
                        sql = "INSERT test.ServerMember (idServer, idUser) VALUES (" + joinRequest.ServerId + ", " + u.Id + ")";
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        Response.StatusCode = (int)HttpStatusCode.Accepted;
                        returnValue = "Welcome to the server";
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    returnValue = "Unknown error!";
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":2}' -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/LeaveServer -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult LeaveServer(JoinRequest joinRequest)
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";
           
            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT s.* FROM test.Token as t, test.User as u, ServerMember as sa, test.Server as s where t.token like '" + userCookie.Value + "' and t.idUser = u.id and sa.idUser = u.id and sa.idServer = s.id and s.id=" + joinRequest.ServerId + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Close();

                    sql = "SELECT u.* FROM test.Token as t, test.User as u where t.token like '" + userCookie.Value + "' and t.idUser = u.id;";
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        User u = new User();
                        while (rdr.Read())
                        {
                            u.Id = (int)rdr[0];
                        }

                        rdr.Close();

                        conn.Close();
                        conn.Open();
                        sql = "DELETE FROM test.ServerMember WHERE idServer=" + joinRequest.ServerId + " and idUser=" + u.Id;
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        Response.StatusCode = (int)HttpStatusCode.Accepted;
                        returnValue = "Bye bye from server";
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    returnValue = "Unknown error!";
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF1ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":2, "UserId":0}' -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/BanUserFromServer -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult BanUserFromServer(JoinRequest userBan)
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";

            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT s.* FROM test.Token as t, test.User as u, ServerMember as sa, test.Server as s where t.token like '" + userCookie.Value + "' and t.idUser = u.id and sa.idUser = u.id and sa.idServer = s.id and s.id=" + userBan.ServerId + " and sa.userRole=1;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Close();


                    conn.Close();
                    conn.Open();

                    sql = "SELECT * FROM ServerMember where idUser=" + userBan.UserId + " and idServer=" + userBan.ServerId;
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        rdr.Close();

                        conn.Close();
                        conn.Open();
                        sql = "UPDATE test.ServerMember SET userRole=-1 WHERE idServer=" + userBan.ServerId + " and idUser=" + userBan.UserId;
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        Response.StatusCode = (int)HttpStatusCode.Accepted;
                        returnValue = "Banhammer done it's work!";
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    returnValue = "Unknown error!";
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF1ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":2, "UserId":0}' -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/PromoteUserToUser -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult PromoteUserToUser(JoinRequest userBan)
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";

            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT s.* FROM test.Token as t, test.User as u, ServerMember as sa, test.Server as s where t.token like '" + userCookie.Value + "' and t.idUser = u.id and sa.idUser = u.id and sa.idServer = s.id and s.id=" + userBan.ServerId + " and sa.userRole=1;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Close();


                    conn.Close();
                    conn.Open();

                    sql = "SELECT * FROM ServerMember where idUser=" + userBan.UserId + " and idServer=" + userBan.ServerId;
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        rdr.Close();

                        conn.Close();
                        conn.Open();
                        sql = "UPDATE test.ServerMember SET userRole=0 WHERE idServer=" + userBan.ServerId + " and idUser=" + userBan.UserId;
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        Response.StatusCode = (int)HttpStatusCode.Accepted;
                        returnValue = "User promoted to user!";
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    returnValue = "Unknown error!";
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF1ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":2, "UserId":0}' -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/PromoteUserToAdmin -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult PromoteUserToAdmin(JoinRequest userBan)
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";

            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT s.* FROM test.Token as t, test.User as u, ServerMember as sa, test.Server as s where t.token like '" + userCookie.Value + "' and t.idUser = u.id and sa.idUser = u.id and sa.idServer = s.id and s.id=" + userBan.ServerId + " and sa.userRole=1;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Close();


                    conn.Close();
                    conn.Open();

                    sql = "SELECT * FROM ServerMember where idUser=" + userBan.UserId + " and idServer=" + userBan.ServerId;
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        rdr.Close();

                        conn.Close();
                        conn.Open();
                        sql = "UPDATE test.ServerMember SET userRole=1 WHERE idServer=" + userBan.ServerId + " and idUser=" + userBan.UserId;
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        Response.StatusCode = (int)HttpStatusCode.Accepted;
                        returnValue = "User promoted to admin!";
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    returnValue = "Unknown error!";
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -d '{"ServerId":1}' -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/RemoveServer -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult RemoveServer(JoinRequest joinRequest)
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "Remove";
        
            HttpCookie userCookie = Request.Cookies["token"];

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sql = "SELECT s.* FROM test.Token as t, test.User as u, ServerMember as sa, test.Server as s where t.token like '" + userCookie.Value + "' and t.idUser = u.id and sa.idUser = u.id and sa.idServer = s.id and sa.userRole=1 and s.id=" + joinRequest.ServerId + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    Server s = new Server();

                    while(rdr.Read())
                    {
                        s.Id = (int)rdr[0];
                    }

                    rdr.Close();

                    conn.Close();
                    conn.Open();
                    sql = "DELETE FROM test.ServerMember WHERE idServer=" + joinRequest.ServerId;
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    conn.Open();
                    sql = "DELETE FROM test.Server WHERE id=" + joinRequest.ServerId;
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    returnValue = "Server left the building!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    returnValue = "Unknown error!";
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return Json(returnValue);
        }

        //curl --cookie "token=ASFSFASASFSF0ASFDGQWAFSGEWQAFSDGEW" -d '{"Id":1,"Name":"Piet"}' -H "Content-Type: application/json" -X POST http://localhost:8080/chatserver/PostMessage -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult PostMessage(User user)
        {
            /*
            Console.WriteLine("AllKeys " + Response.Cookies["WHAT"].Value);

            HttpCookie aCookie = Request.Cookies["WHAT"];

            User u = new User();
            u.Id = 1;

            if (string.Compare(aCookie.Value, "dat", StringComparison.CurrentCulture) == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else
            {
                u.Name = "AAAAAA";//aCookie.Value;
            }

            Console.WriteLine("Name " + user.Id + " " + user.Name);
*/
            User u = new User();
            u.Name = "JAJAJAJA";
            return Json(u);
        }
    }
}
