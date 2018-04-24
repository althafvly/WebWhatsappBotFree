using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//esrefyildirici.com
namespace WebWhatsappBotFree
{
    class Mesajlar
    {
        private string mesaj;
        private string id;


        public string Mesaj
        {
            get
            {
                return mesaj;
            }

            set
            {
                mesaj = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
    }
}
