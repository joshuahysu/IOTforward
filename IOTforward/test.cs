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
    internal class test
    {        

        private static void test2(string[] args)
        {
            Dictionary<String, ModbusTcpClientThread> modbusTcpClientThreadDic = new Dictionary<String, ModbusTcpClientThread>();
            Dictionary<String, OmronFinsClientThread> finsClientThreadDic = new Dictionary<String, OmronFinsClientThread>();


            var deviceinfoTable = SQLiteDb.deviceinfoModel();
            var modbusInputList = new List<ModbusInput>();
            var modbusTransferList = new List<ModbusTransfer>();
            var modbusTransferDic=new Dictionary<string, ModbusTransfer>();     
              
            foreach (var item in deviceinfoTable)
            {
                if (item.connectiontype == "Modbus TCP")
                {                  
                    var svidinfoTable = SQLiteDb.selectSvidinfoModbusN(item.modbusn,item.mapfile);
                    foreach (var sivdItem in svidinfoTable)
                    {
                        static DataTypeEnum GetDataType(int sivdItem)
                        {
                            // 假设根据 sivdItem 的某个属性来决定 DataType
                            if (sivdItem == 1)
                            {
                                return DataTypeEnum.Int16;
                            }
                            else if (sivdItem == 2)
                            {
                                return DataTypeEnum.UInt32; // 其他数据类型
                            }
                            else if (sivdItem == 3)
                            {
                                return DataTypeEnum.Int32; // 其他数据类型
                            }
                            else if (sivdItem == 4)
                            {
                                return DataTypeEnum.Float; // 其他数据类型
                            }
                            else if (sivdItem == 5)
                            {
                                return DataTypeEnum.Double; // 其他数据类型//不知道效果
                            }
                            else if (sivdItem == 6)
                            {
                                return DataTypeEnum.UInt64; // 其他数据类型
                            }
                            else if (sivdItem == 7)
                            {
                                return DataTypeEnum.Int64; // 其他数据类型
                            }
                            else {
                                return DataTypeEnum.UInt16; // 其他数据类型
                            }
                        }

                        if (sivdItem.functioncode == "") { continue; }
                        modbusInputList.Add(new ModbusInput()
                        {
                            Address = sivdItem.address,
                            DataType = GetDataType(sivdItem.mergemode),
                            FunctionCode = (byte)int.Parse(sivdItem.functioncode),
                            StationNumber = 1
                        });

                        modbusTransferDic.Add(sivdItem.functioncode + sivdItem.address, new ModbusTransfer()
                        {
                            serveraddress = sivdItem.serveraddress,
                            serverfunctioncode = sivdItem.serverfunctioncode,
                            DataType = sivdItem.mergemode,
                            scalemultiple = float.Parse(sivdItem.scalemultiple),
                            scaleoffset = float.Parse(sivdItem.scaleoffset)
                        });
                    }

                    var modbusThread = new ModbusTcpClientThread(item.deviceip, int.Parse(item.deviceport), modbusInputList);
                    modbusThread.StartTransferThread(modbusTransferDic);
                    modbusTcpClientThreadDic.Add(item.deviceip, modbusThread);
                }
                else if (item.connectiontype == "FINS")
                {
              
                }
                else if (item.connectiontype == "COM")
                {

                }                
            }

            var modbusTcpServer = new ModbusTcpServer(502,"127.0.0.1");

            modbusTcpServer.Start();
            //1、Instantiate the client-enter the correct IP and port
            ModbusTcpClient client = new ModbusTcpClient("127.0.0.1", 502);
            client.Open();
            //2、Write operation-parameters are: address, value, station number, function code
            client.Write("4", (short)33, 2, 16);
            byte stationNumber = 2;
            //2.1、[Note] When writing data, you need to clarify the data type
            client.Write("0", (short)33, 2, 16);    //Write short type value
            client.Write("4", (ushort)61133, 2, 16);   //Write ushort type value
            client.Write("8", (int)330000, 2, 16);      //Write int type value
            client.Write("12", (uint)33, 2, 16);    //Write uint type value
            client.Write("16", (long)330000, 2, 16);    //Write long type value
            client.Write("20", (ulong)33, 2, 16);   //Write ulong type value
            client.Write("24", (float)33, 2, 16);   //Write float type value
            client.Write("28", (double)33, 2, 16);  //Write double type value
            client.Write("32", true, 2, 5);         //Write Coil type value
            client.Write("100", "orderCode", stationNumber);  //Write string

            //3、Read operation-the parameters are: address, station number, function code
            for (int i = 0; i < 10000; i++)
            {
                var value = client.ReadInt16("4", 2, 3).Value;

                //3.1、Other types of data reading
                //var result = client.ReadInt16("0", stationNumber, 3);    //short type data read
                //var isSucceed = result.IsSucceed;
                //Console.WriteLine(result.Value);
            }
            client.ReadUInt16("4", stationNumber, 3);   //ushort type data read
            client.ReadInt32("8", stationNumber, 3);    //int type data read
            client.ReadUInt32("12", stationNumber, 3);  //uint type data read
            var sdg=client.ReadInt64("16", stationNumber, 3);   //long type data read
            client.ReadUInt64("20", stationNumber, 3);  //ulong type data read
            client.ReadFloat("24", stationNumber, 3);   //float type data read
            client.ReadDouble("28", stationNumber, 3);  //double type data read
            client.ReadCoil("32", stationNumber, 1);    //Coil type data read
            client.ReadDiscrete("32", stationNumber, 2);//Discrete type data read
            //for (int i = 0; i < 10000; i++)
            //{
            //    var result = client.ReadString("100", stationNumber, readLength: 10); //Read string
            //    //Console.WriteLine(result.Value);
            //}
            //4、If there is no active Open, it will automatically open and close the connection every time you read and write operations, which will greatly reduce the efficiency of reading and writing. So it is recommended to open and close manually.
            // client.Open();

            ////5、Read and write operations will return the operation result object Result
            //var result = client.ReadInt16("4", 2, 3);
            ////5.1 Whether the reading is successful (true or false)
            //var isSucceed = result.IsSucceed;
            ////5.2 Exception information for failed reading
            //var errMsg = result.Err;
            ////5.3 Read the request message actually sent by the operation
            //var requst = result.Requst;
            ////5.4 Read the response message from the server
            //var response = result.Response;
            ////5.5 Read value
            //var value3 = result.Value;

            //6、Batch read
            var list = new List<ModbusInput>();
            list.Add(new ModbusInput()
            {
                Address = "4",
                DataType = DataTypeEnum.Int16,
                FunctionCode = 3,
                StationNumber = 2
            });
            list.Add(new ModbusInput()
            {
                Address = "1",
                DataType = DataTypeEnum.UInt16,
                FunctionCode = 3,
                StationNumber = 2
            });
            list.Add(new ModbusInput()
            {
                Address = "8",
                DataType = DataTypeEnum.Int32,
                FunctionCode = 3,
                StationNumber = 2
            });
            var result = client.BatchRead(list);
           // var s=result.Value;
            var a = 1;
            ////7、Other parameters of the constructor
            ////IP, port, timeout time, big and small end settings
            //ModbusTcpClient client = new ModbusTcpClient("127.0.0.1", 502, 1500, EndianFormat.ABCD);
        }
    }
}