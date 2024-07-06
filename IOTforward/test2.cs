using IoTClient.Clients.Modbus;
using IoTClient.Clients;
using IoTClient;
using System;
using System.Collections.Generic;
using System.Net;
using IoTClient.Enums;
using IoTClient.Models;
using IOTforward.model;
using System.ComponentModel.DataAnnotations;
namespace IOTforward
{
    internal class test3
    {        

        public static void test4()
        {
         
            var modbusTcpServer = new ModbusTcpServer(502,"127.0.0.1");

            modbusTcpServer.Start();
            // 1、Instantiate the client - enter the correct IP and port
            ModbusTcpClient client = new ModbusTcpClient("127.0.0.1", 502);
            client.Open();
            //2、Write operation-parameters are: address, value, station number, function code
            client.Write("4", (short)33, 1, 16);
            byte stationNumber = 1;
            //2.1、[Note] When writing data, you need to clarify the data type
            client.Write("0", (short)33, 1, 16);    //Write short type value
            client.Write("4", (ushort)61133, 1, 16);   //Write ushort type value
            client.Write("8", (int)330000, 1, 16);      //Write int type value
            client.Write("12", (uint)33, 1, 16);    //Write uint type value
            client.Write("16", (long)330000, 1, 16);    //Write long type value
            client.Write("20", (ulong)33, 1, 16);   //Write ulong type value
            client.Write("24", (float)33, 1, 16);   //Write float type value
            client.Write("28", (double)33, 1, 16);  //Write double type value
            client.Write("32", true, 2, 5);         //Write Coil type value
            client.Write("100", "orderCode", stationNumber);  //Write string


        }
    }
}