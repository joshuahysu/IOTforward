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
namespace IOTforward
{
    internal class Program
    {        

        private static void Main(string[] args)
        {
            //addLogService();

            Dictionary<String, ModbusTcpClientThread> modbusTcpClientThreadDic = new Dictionary<String, ModbusTcpClientThread>();
            Dictionary<String, OmronFinsClientThread> finsClientThreadDic = new Dictionary<String, OmronFinsClientThread>();

            var deviceinfoTable = SQLiteDb.deviceinfoModel();
            var modbusInputList = new List<ModbusInput>();
            var modbusTransferList = new List<ModbusTransfer>();
            var modbusTransferDic = new Dictionary<string, ModbusTransfer>();

            var modbusTcpServer = new ModbusTcpServer(503, "127.0.0.1");

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

                if (item.connectiontype == "Modbus TCP")
                {
                    
                    var svidinfoTable = SQLiteDb.selectSvidinfoModbusN(item.modbusn, item.mapfile);
                    foreach (var sivdItem in svidinfoTable)
                    {     

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
                    var omronFinsInputList = new Dictionary<string, DataTypeEnum>();
                    var svidinfoTable = SQLiteDb.selectSvidinfoModbusN(item.modbusn, item.mapfile);
                    foreach (var sivdItem in svidinfoTable)
                    {
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
                    var omronFinsThread = new OmronFinsClientThread(item.deviceip, int.Parse(item.deviceport), omronFinsInputList);
                    omronFinsThread.StartTransferThread(modbusTransferDic);
                    finsClientThreadDic.Add(item.deviceip, omronFinsThread);
                }
                else if (item.connectiontype == "COM")
                {

                }
            }

            while (true)
            {
                Thread.Sleep(1000);
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