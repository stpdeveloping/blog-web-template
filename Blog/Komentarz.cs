using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog
{
    public class Komentarz
    {
        public Int32 id_komentarza;
        public string kom_tresc;
        public string data_dodania;
        public string user;
        public Int32 id_artykulu;
    }
}