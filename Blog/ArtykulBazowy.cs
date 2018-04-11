using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog
{
    public class ArtykulBazowy
    {
        public Int32 artid;
        public string tit;
        public string subtit;
        public string cont;
        public List<string> linksarr = new List<string>();
        public List<Komentarz> comments = new List<Komentarz>();
    }
}