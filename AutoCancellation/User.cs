using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCancellation
{
    class User
    {
        public string _id { get; set; }
        public string _pwd { get; set; }

        public string _comcode { get; set; }
        public string _ipAddress { get; set; }
        public string _qrCode { get; set; }
        public string _userName { get; set; }

        public User(string id,string pwd,string comcode,string qrCode ,string ipaddress)
        {
            _id = id;
            _pwd = pwd;
            _comcode = comcode;
            _ipAddress = ipaddress;
            _qrCode = qrCode;
        } 
    }
}
