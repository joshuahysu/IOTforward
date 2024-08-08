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
using System.IO.BACnet.Serialize;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;
using System.Collections;
namespace IOTforward
{
    internal class Program
    {
        public static ModbusTcpServerJoshua modbusTcpServer;
        private static void Main(string[] args)
        {
            //addLogService();

            Dictionary<String, ModbusTcpClientThread> modbusTcpClientThreadDic = new Dictionary<String, ModbusTcpClientThread>();
            Dictionary<String, OmronFinsClientThread> finsClientThreadDic = new Dictionary<String, OmronFinsClientThread>();

            var deviceinfoTable = SQLiteDb.deviceinfoModel();
            var modbusInputList = new List<ModbusInput>();
            var modbusTransferList = new List<ModbusTransfer>();
        

            modbusTcpServer = new ModbusTcpServerJoshua(503, "0.0.0.0");
            //var modbusTcpServer = new ModbusTcpServer(503, "0.0.0.0");

            modbusTcpServer.Start();

            foreach (var item in deviceinfoTable)
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
                    else
                    {
                        return DataTypeEnum.UInt16; // 其他数据类型
                    }
                }
                string address_227 = "9999";
                var modbusTransferDic = new Dictionary<string, ModbusTransfer>();

                //區分不同通訊協定
                if (item.connectiontype == "Modbus TCP")
                {
                    
                    var svidinfoTable = SQLiteDb.selectSvidinfoModbusN(item.modbusn, item.mapfile);
                    
                    foreach (var sivdItem in svidinfoTable)
                    {

                        if (sivdItem.functioncode == "") { continue; }

                        //227代表對下裝置是否有連線
                        else if (sivdItem.parameterid=="227") {
                            address_227 = sivdItem.serveraddress;
                        }
                        modbusInputList.Add(new ModbusInput()
                        {
                            Address = sivdItem.address,
                            DataType = GetDataType(sivdItem.mergemode),
                            FunctionCode = (byte)int.Parse(sivdItem.functioncode),
                            StationNumber = Byte.Parse(item.devicestationid)
                        });


                        ModbusTransfer modbusTransferItem= new ModbusTransfer()
                        {
                            serveraddress = sivdItem.serveraddress,
                            serverfunctioncode = sivdItem.serverfunctioncode,
                            DataType = sivdItem.mergemode,
                            scalemultiple = float.Parse(sivdItem.scalemultiple),
                            scaleoffset = float.Parse(sivdItem.scaleoffset)
                        };

                        if (modbusTransferDic.TryGetValue(sivdItem.functioncode + sivdItem.address, out _))
                        {
                            // 键已经存在，覆盖值
                            modbusTransferDic[sivdItem.functioncode + sivdItem.address] = modbusTransferItem;
                        }
                        else
                        {
                            modbusTransferDic.Add(sivdItem.functioncode + sivdItem.address, modbusTransferItem);
                        }        
                    }

                    var modbusThread = new ModbusTcpClientThread(item.deviceip, int.Parse(item.deviceport), modbusInputList);

                    //需要讀取的點位及寫入的及斷連線點位
                    modbusThread.StartTransferThread(modbusTransferDic, address_227);
                    modbusTcpClientThreadDic.Add(item.deviceip+"."+item.devicestationid, modbusThread);
                }
                else if (item.connectiontype == "FINS")
                {
                    var omronFinsInputList = new Dictionary<string, DataTypeEnum>();
                    var svidinfoTable = SQLiteDb.selectSvidinfoModbusN(item.modbusn, item.mapfile);
                    foreach (var sivdItem in svidinfoTable)
                    {
                        if (sivdItem.parameterid == "227")
                        {
                            address_227 = sivdItem.serveraddress;
                        }
                        omronFinsInputList.Add(sivdItem.tagname, GetDataType(sivdItem.mergemode));

                        modbusTransferDic.Add(sivdItem.tagname, new ModbusTransfer()
                        {
                            serveraddress = sivdItem.serveraddress,
                            serverfunctioncode = sivdItem.serverfunctioncode,
                            DataType = sivdItem.mergemode,
                            scalemultiple = float.Parse(sivdItem.scalemultiple),
                            scaleoffset = float.Parse(sivdItem.scaleoffset)
                        });
                    }
                    var omronFinsThread = new OmronFinsClientThread(item.deviceip, int.Parse(item.deviceport), omronFinsInputList, int.Parse(item.finsdestination));
                    omronFinsThread.StartTransferThread(modbusTransferDic, address_227);
                    finsClientThreadDic.Add(item.deviceip+"."+ item.finsdestination, omronFinsThread);
                }
                else if (item.connectiontype == "COM")
                {
                    //var serialPortThread=new SerialPortThread(item.p,item.stop);

                }
                else if (item.connectiontype == "OPCUA")
                {

                }
            }

            while (true)
            {
                Thread.Sleep(5000);
            }
        }

        private static void addLogService()
        {
            // 创建服务集合并配置日志记录
            var serviceCollection = new ServiceCollection();

            // 构建服务提供者
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // 获取日志记录器
            var logger = serviceProvider.GetService<ILogger<Program>>();

            // 使用日志记录器记录信息
            logger.LogInformation("Application started");

            try
            {
                // 模拟操作
                logger.LogInformation("Performing some operation...");
                throw new InvalidOperationException("Something went wrong");
            }
            catch (Exception ex)
            {
                // 记录错误
                logger.LogError(ex, "An error occurred");
            }

            logger.LogInformation("Application finished");
        }
    }
}