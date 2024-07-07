using IoTClient.Clients.Modbus;
using IoTClient.Clients.PLC;
using IoTClient.Enums;
using IoTClient.Models;
using IOTforward.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTforward
{
    internal class OmronFinsClientThread
    {
        OmronFinsClient deviceClient;
        public Dictionary<string, DataTypeEnum> inputsDic { get; private set; }
        public string DeviceIsConnectionAddress { get; set; }

        public int FinsDestination { get; set; }
        internal OmronFinsClientThread(string ip,int port, Dictionary<string, DataTypeEnum> inputs, int finsDestination) {            
     
            //1、Instantiate the client-enter the correct IP and port
            deviceClient = new OmronFinsClient(ip, port);
            deviceClient.Open();
            inputsDic = inputs;
            FinsDestination = finsDestination;

        }

        internal void StartTransferThread(Dictionary<string, ModbusTransfer> modbusTransferDic, string deviceIsConnectionAddress) {
            ModbusTcpClient localClient = new ModbusTcpClient("127.0.0.1", 502);
            localClient.Open();
            DeviceIsConnectionAddress = deviceIsConnectionAddress;
            new Thread(() => {

                while (true)
                {
                    //第一次strartAddress執行會是0
                    //是否有連線
                    localClient.Write(DeviceIsConnectionAddress, Convert.ToUInt16(localClient.Connected));

                    var result = deviceClient.BatchRead(inputsDic, FinsDestination);

                    foreach (var item in result.Value)
                    {              
                        modbusTransferDic.TryGetValue(item.Key,out ModbusTransfer modbusTransferAddress);

                        if (modbusTransferAddress.DataType==1) { localClient.Write(modbusTransferAddress.serveraddress, (uint)item.Value);}
                        else if (modbusTransferAddress.DataType == 2) { localClient.Write(modbusTransferAddress.serveraddress, (uint)item.Value);  }
                        else if (modbusTransferAddress.DataType == 3) { localClient.Write(modbusTransferAddress.serveraddress, (uint)item.Value); }
                        else if (modbusTransferAddress.DataType == 4) { localClient.Write(modbusTransferAddress.serveraddress, (uint)item.Value); }
                        else if (modbusTransferAddress.DataType == 5) { localClient.Write(modbusTransferAddress.serveraddress, (uint)item.Value); }
                        else if (modbusTransferAddress.DataType == 6) { localClient.Write(modbusTransferAddress.serveraddress, (uint)item.Value); }
                        else if (modbusTransferAddress.DataType == 7) { localClient.Write(modbusTransferAddress.serveraddress, (uint)item.Value); }
                        else if (modbusTransferAddress.DataType == 8) { localClient.Write(modbusTransferAddress.serveraddress, (uint)item.Value); }



                        //client.Write("0", (short)33, 2, 16);    //Write short type value
                        //client.Write("4", (ushort)61133, 2, 16);   //Write ushort type value
                        //client.Write("8", (int)330000, 2, 16);      //Write int type value
                        //client.Write("12", (uint)33, 2, 16);    //Write uint type value
                        //client.Write("16", (long)330000, 2, 16);    //Write long type value
                        //client.Write("20", (ulong)33, 2, 16);   //Write ulong type value

                    }
                    Thread.Sleep(5000);
                }
            }).Start();
        }

    }
}
