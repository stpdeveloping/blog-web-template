using Newtonsoft.Json;
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
        private static Int32 liczba_id;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["uzytkownik"] = HttpContext.Current.Session["uzytkownik"];
            Session["uprawnienia"] = HttpContext.Current.Session["uprawnienia"];
            Session["aktywna_strona"] = HttpContext.Current.Session["aktywna_strona"];
        }
        public static bool Crud(string query)
        {
            SqlDataReader odczyt;
      string con = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Seshsadboy\\source\\repos\\Blog\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
      string con2 = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\RapForLife\\Source\\Repos\\webapi\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (query.Contains("SELECT"))
                    {   if(query.Contains("COUNT"))
                        {
                            liczba_id = (Int32) cmd.ExecuteScalar();
                            return true;
                        }
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
            Crud("INSERT INTO artykuly (title, subtitle, tresc) VALUES ('" + Tytul + "', '" + Podtytul + "', '" + Tresc + "')");
            Crud("SELECT COUNT(article_id) FROM artykuly");
            for(int i=0; i<Links_arr.Length; i++)
                Crud("INSERT INTO article_imgs (article_id, img) VALUES (" + liczba_id + ", '" + Links_arr[i] + "')");
        }
        public static List<ArtykulBazowy> LoadARTCLS()
        {
            SqlDataReader odczyt;
            List<ArtykulBazowy> artcllist = new List<ArtykulBazowy>();
            string con = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Seshsadboy\\source\\repos\\Blog\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
            string con2 = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\RapForLife\\Source\\Repos\\webapi\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT article_id, title, subtitle, tresc FROM artykuly", connection))
                {
                    odczyt = cmd.ExecuteReader();
                    while (odczyt.Read())
                    {
                        ArtykulBazowy artcl = new ArtykulBazowy();
                        artcl.artid = odczyt.GetInt32(0);
                        artcl.tit = odczyt.GetString(1);
                        artcl.subtit = odczyt.GetString(2);
                        artcl.cont = odczyt.GetString(3);
                        artcllist.Add(artcl);
                    }
                }
            }
            return artcllist;
            }
        public static List<ArtykulBazowy> ARTCLSwithIMGs(List<ArtykulBazowy> listaARTYKULOW)
        {
            SqlDataReader odczyt;
            string con = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Seshsadboy\\source\\repos\\Blog\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
            string con2 = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\RapForLife\\Source\\Repos\\webapi\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();
                for (int i = 0; i < listaARTYKULOW.Count; i++)
                    using (SqlCommand cmd = new SqlCommand("SELECT img FROM article_imgs WHERE article_id = " + listaARTYKULOW[i].artid, connection))
                    {
                        odczyt = cmd.ExecuteReader();
                        while (odczyt.Read())
                            listaARTYKULOW[i].linksarr.Add(odczyt.GetString(0));
                        odczyt.Close();
                    }
            }
            return listaARTYKULOW;
        }
        public static List<ArtykulBazowy> ARTCLSwithCOMMENTs(List<ArtykulBazowy> listaARTYKULOW)
        {
            SqlDataReader odczyt;
            string con = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Seshsadboy\\source\\repos\\Blog\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
            string con2 = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\RapForLife\\Source\\Repos\\webapi\\Blog\\App_Data\\Database1.mdf;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();
                for (int i = 0; i < listaARTYKULOW.Count; i++)
                    using (SqlCommand cmd = new SqlCommand("SELECT Id_komentarza, content_kom, [data], [user], article_id FROM komentarze WHERE article_id = " + listaARTYKULOW[i].artid, connection))
                    {
                        odczyt = cmd.ExecuteReader();
                        while (odczyt.Read())
                        {
                            Komentarz com = new Komentarz();
                            com.id_komentarza = odczyt.GetInt32(0);
                            com.id_artykulu = odczyt.GetInt32(4);
                            com.kom_tresc = odczyt.GetString(1);
                            com.data_dodania = odczyt.GetString(2);
                            com.user = odczyt.GetString(3);
                            listaARTYKULOW[i].comments.Add(com);
                        }
                        odczyt.Close();
                    }
            }
            return listaARTYKULOW;
        }
        [WebMethod]
        public static string WczytanieArtykulow()
        {
            List<ArtykulBazowy> lista = LoadARTCLS();
            lista = ARTCLSwithIMGs(lista);
            ArtykulBazowy[] artarr = ARTCLSwithCOMMENTs(lista).ToArray();
            string json = JsonConvert.SerializeObject(artarr);
            return json;
        }
        [WebMethod]
        public static void DodanieKomentarza(string content, string username, string article_id)
        {
            DateTime data = DateTime.Now;
            string sformatowanaData = data.ToString("yyyy-MM-dd HH:mm");
            Crud("INSERT INTO komentarze (content_kom, [data], [user], article_id) VALUES ('" + content + "', '" 
                + sformatowanaData + "', '" + username + "', " + article_id + ")");
        }
        [WebMethod]
        public static void UsuwanieKomentarza(string com_id)
        {
            Crud("DELETE FROM komentarze WHERE Id_komentarza = " + com_id);
        }
        [WebMethod]
        public static void UsuwanieArtykulu(string art_id)
        {
            Crud("DELETE FROM artykuly WHERE article_id = " + art_id);
        }
    }
}