using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTforward.model
{
    public class DeviceinfoModel
    {

        public int Id { get; set; }
        public string name { get; set; }
        public string maker { get; set; }
        public string toolid { get; set; }
        public string dpmid { get; set; }
        public string chamberid { get; set; }
        public string parameterid { get; set; }
        public string devicetype { get; set; }
        public string connectiontype { get; set; }
        public string mapfile { get; set; }

        public string slaveport { get; set; }
        public string baudrate { get; set; }
        public string databits { get; set; }
        public string stop { get; set; }
        public string parity { get; set; }
        public string serverip { get; set; }
        public string serverport { get; set; }
        public string serverstationid { get; set; }
        public string analogcode { get; set; }
        public string readercode { get; set; }
        public string deviceip { get; set; }
        public string deviceport { get; set; }
        public string devicestationid { get; set; }
        public string finsdestination { get; set; }
        public string modbusn { get; set; } 
        
        public string serialporttype { get; set; }
    }
}
