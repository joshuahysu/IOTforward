using IoTClient.Enums;
using IoTClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTforward.model
{
    public class ModbusTransfer
    {
        public string serveraddress;
        public string serverfunctioncode;

        public float scalemultiple;
        public float scaleoffset;

        public int DataType { get; internal set; }
    }
}
