using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using ShareWithUs.Models;

namespace ShareWithUs.Controllers
{
    public class UserController : Controller
    {
        //curl -H "Content-Type: application/json" -X POST http://localhost:8080/user/ -i 
        [System.Web.Mvc.HttpGet]
        public JsonResult Index()
        {
            return Json(getUsers(), JsonRequestBehavior.AllowGet);
        }

        private List<User> getUsers()
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            MySqlConnection conn = new MySqlConnection(connStr);
            List<User> users = new List<Models.User>();
            try
            {
                conn.Open();

                string sql = "SELECT * FROM test.User";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    User u = new User();
                    u.Id = (int)rdr[0];
                    u.Name = (string)rdr[1];
                    users.Add(u);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();

            return users;
        }

        //curl -d '{"Name":"Piet", "Password":"Dit_Is_Een_test"}' -H "Content-Type: application/json" -X POST http://localhost:8080/user/CreateAccount -i      
        [System.Web.Mvc.HttpPost]
        public JsonResult CreateAccount(User user)
        {
            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                string sql = "SELECT * FROM test.User where name like '" + user.Name + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (!rdr.HasRows)
                {
                    conn.Close();
                    conn.Open();
                    sql = "INSERT test.User (name, password) VALUES ('" + user.Name + "', '" + user.Password + "')";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    Response.StatusCode = (int)HttpStatusCode.Accepted;
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Conflict;
                    returnValue = "User name is taken.";
                }
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

        //curl -d '{"Name":"Piet", "Password":"Dit_Is_Een_test"}' -H "Content-Type: application/json" -X POST http://localhost:8080/user/LogIn -i 
        //curl -d '{"Token":"<insert token here>"}' -H "Content-Type: application/json" -X POST http://localhost:8080/user/LogIn -i 
        [System.Web.Mvc.HttpPost]
        public JsonResult LogIn(User user)
        {
            User u = new User();
            u.Id = -1;

            string connStr = "server=localhost;user=root;database=test;" +
                "port=3306;password=Dit_Is_Voor_De_HvA01+-";
            string returnValue = "welcome";

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                if (user.Token.CompareTo("") != 0)
                {
                    string sql = "SELECT idUser FROM test.Token where token like '" + user.Token + "'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            u.Id = (int)rdr[0];
                        }

                        rdr.Close();

                        Response.StatusCode = (int)HttpStatusCode.Accepted;

                        u.Token = "ASFSFASASFSF" + u.Id + "ASFDGQWAFSGEWQAFSDGEW";
                        u.Id = -100;
                    }
                }
                else
                {
                    string sql = "SELECT * FROM test.User where name like '" + user.Name + "' and password like '" + user.Password + "'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            u.Id = (int)rdr[0];
                            u.Name = (string)rdr[1];
                            u.Password = (string)rdr[2];
                        }

                        u.Token = "ASFSFASASFSF" + u.Id + "ASFDGQWAFSGEWQAFSDGEW";

                        conn.Close();
                        conn.Open();
                        sql = "INSERT test.Token (token, idUser, lastLogin) VALUES ('" + u.Token + "', '" + u.Id + "', now())";
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        Response.StatusCode = (int)HttpStatusCode.Accepted;

                        u.Id = -100;
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.Gone;
                        returnValue = "User not known.";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnValue = "Unexcepted error!";
            }
            conn.Close();

            return (u.Id == -100) ? Json(u) : Json(returnValue);
        }
    }
}