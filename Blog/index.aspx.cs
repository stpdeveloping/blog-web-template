using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blog
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["uzytkownik"] = HttpContext.Current.Session["uzytkownik"];
            Session["uprawnienia"] = HttpContext.Current.Session["uprawnienia"];
            Session["aktywna_strona"] = HttpContext.Current.Session["aktywna_strona"];
        }
        public static bool Crud(string query)
        {
      string con = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Seshsadboy\\source\\repos\\Blog\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
      string con2 = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\RapForLife\\Source\\Repos\\webapi\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(con2))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    SqlDataReader odczyt;
                    if (query.Contains("SELECT"))
                    {
                        odczyt = cmd.ExecuteReader();
                        if (odczyt.HasRows)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                /*cmd.Connection = connection;
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                return true;*/
            }
        }
        [WebMethod]
        public static String RejestracjaUzytkownika (string nick, string password, string city, string email)
        {
            if (Crud("SELECT nazwa, email FROM uzytkownicy WHERE nazwa = '" + nick + "'"))
                return "zly-nick";
            if (Crud("SELECT nazwa, email FROM uzytkownicy WHERE email = '" + email + "'"))
                return "zly-email";
            Crud("INSERT INTO uzytkownicy (nazwa, haslo, miasto, email, uprawnienia) VALUES ('" + nick + "', '" + password + "', '" + city + "', '" + email + "', 'false')");
                return "prawidlowe-dane";
        }
        [WebMethod]
        public static bool LogowanieUzytkownika(string nick, string password)
        {
            
            if (Crud("SELECT nazwa, haslo FROM uzytkownicy WHERE nazwa = '" + nick + "' AND haslo = '" + password + "'"))
            {
                HttpContext.Current.Session["uzytkownik"] = nick;
                if (Crud("SELECT nazwa, uprawnienia FROM uzytkownicy WHERE nazwa = '" + nick + "' AND uprawnienia = 'true'"))
                    HttpContext.Current.Session["uprawnienia"] = "admin";
                else
                    HttpContext.Current.Session["uprawnienia"] = "user";
                return true;
            }
            return false;
        }
        [WebMethod]
        public static string[] SprawdzenieSesji()
        {
            string[] arr = new string[3];
            arr[0] = (string) HttpContext.Current.Session["uzytkownik"];
            arr[1] = (string) HttpContext.Current.Session["uprawnienia"];
            arr[2] = (string) HttpContext.Current.Session["aktywna_strona"];
            return arr;
        }
        [WebMethod]
        public static void UsuwanieSesji()
        {
            HttpContext.Current.Session["uzytkownik"] = null;
            HttpContext.Current.Session["uprawnienia"] = null;
        }
        [WebMethod]
        public static void SprawdzenieAktywnejStrony(int numer_strony)
        {
            switch (numer_strony)
            {
                case 1:
                    HttpContext.Current.Session["aktywna_strona"] = "o-mnie";
                    break;
                case 2:
                    HttpContext.Current.Session["aktywna_strona"] = "newsy";
                    break;
                case 3:
                    HttpContext.Current.Session["aktywna_strona"] = "cpu-rank";
                    break;
                case 4:
                    HttpContext.Current.Session["aktywna_strona"] = "gpu-rank";
                    break;
                case 5:
                    HttpContext.Current.Session["aktywna_strona"] = "reg";
                    break;
            }
        }
        [WebMethod]
        public static void artykul(string Tytul, string Podtytul, string[] Links_arr, string Tresc)
        {
            
        }
    }
}