using IoTClient.Clients.Modbus;
using IoTClient.Enums;
using IoTClient.Models;
using IOTforward.model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IOTforward
{
    internal class ModbusTcpClientThread
    {
        List<ModbusInput> list = new List<ModbusInput>();
        ModbusTcpClient deviceClient;
        public string DeviceIsConnectionAddress { get; set; }
        internal ModbusTcpClientThread(string ip, int port, List<ModbusInput> inputs)
        {

            this.list = inputs;
            deviceClient = new ModbusTcpClient(ip, port);
            deviceClient.Open();

        }

        internal void StartTransferThread(Dictionary<string, ModbusTransfer> modbusTransferDic, string deviceIsConnectionAddress)
        {
            ModbusTcpClient localClient = new ModbusTcpClient("127.0.0.1", 503);
            localClient.Open();

            DeviceIsConnectionAddress = deviceIsConnectionAddress;

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        //第一次strartAddress執行會是0
                        //是否有連線
                        localClient.Write(DeviceIsConnectionAddress, Convert.ToUInt16(localClient.Connected));
                        var result = deviceClient.BatchRead(list);

                        foreach (var item in result.Value)
                        {
                            modbusTransferDic.TryGetValue(item.FunctionCode.ToString() + $"{Convert.ToInt32(item.Address):D4}", out ModbusTransfer modbusTransferAddress);
                            if (modbusTransferAddress != null)
                            {
                                if (modbusTransferAddress.DataType == 1)
                                {
                                    localClient.Write(modbusTransferAddress.serveraddress, ConvertToWithClamp(Convert.ToInt16(item.Value), modbusTransferAddress.scalemultiple, modbusTransferAddress.scaleoffset));
                                }
                                else if (modbusTransferAddress.DataType == 2)
                                {
                                    localClient.Write(modbusTransferAddress.serveraddress, ConvertToWithClamp(Convert.ToUInt32(item.Value), modbusTransferAddress.scalemultiple, modbusTransferAddress.scaleoffset));
                                }
                                else if (modbusTransferAddress.DataType == 3) { localClient.Write(modbusTransferAddress.serveraddress, ConvertToWithClamp(Convert.ToInt32(item.Value), modbusTransferAddress.scalemultiple, modbusTransferAddress.scaleoffset)); }
                                else if (modbusTransferAddress.DataType == 4) { localClient.Write(modbusTransferAddress.serveraddress, ConvertToWithClamp(Convert.ToDouble(item.Value), modbusTransferAddress.scalemultiple, modbusTransferAddress.scaleoffset)); }
                                else if (modbusTransferAddress.DataType == 5) { localClient.Write(modbusTransferAddress.serveraddress, ConvertToWithClamp(Convert.ToDouble(item.Value), modbusTransferAddress.scalemultiple, modbusTransferAddress.scaleoffset)); }
                                else if (modbusTransferAddress.DataType == 6) { localClient.Write(modbusTransferAddress.serveraddress, ConvertToWithClamp(Convert.ToUInt64(item.Value), modbusTransferAddress.scalemultiple, modbusTransferAddress.scaleoffset)); }
                                else if (modbusTransferAddress.DataType == 7) { localClient.Write(modbusTransferAddress.serveraddress, ConvertToWithClamp(Convert.ToInt64(item.Value), modbusTransferAddress.scalemultiple, modbusTransferAddress.scaleoffset)); }
                                else { localClient.Write(modbusTransferAddress.serveraddress, ConvertToWithClamp(Convert.ToUInt16(item.Value), modbusTransferAddress.scalemultiple, modbusTransferAddress.scaleoffset)); }
                            }

                        }
                    }
                    catch (Exception)
                    {
                        //throw;
                        Thread.Sleep(5000);
                    }
                    Thread.Sleep(300);
                }
            }).Start();
        }
        public static ushort ConvertToUInt16WithClamp(float value)
        {
            value = Math.Clamp(value, ushort.MinValue, ushort.MaxValue);
            return (ushort)value;
        }
        public static T ConvertToWithClamp<T>(T value, float b, float c)
        {
            if (b == 1 && c == 0)
            {
                return value;
            }

            // Convert value to float, perform calculations
            float result = Convert.ToSingle(value) * b + c;

            // Determine the appropriate clamp range based on T
            dynamic clampedValue;
            if (typeof(T) == typeof(byte))
            {
                clampedValue = (byte)Math.Clamp(result, byte.MinValue, byte.MaxValue);
            }
            else if (typeof(T) == typeof(short))
            {
                clampedValue = (short)Math.Clamp(result, short.MinValue, short.MaxValue);
            }
            else if (typeof(T) == typeof(ushort))
            {
                clampedValue = (ushort)Math.Clamp(result, ushort.MinValue, ushort.MaxValue);
            }
            else if (typeof(T) == typeof(int))
            {
                clampedValue = (int)Math.Clamp(result, int.MinValue, int.MaxValue);
            }
            else if (typeof(T) == typeof(uint))
            {
                clampedValue = (uint)Math.Clamp(result, uint.MinValue, uint.MaxValue);
            }
            else if (typeof(T) == typeof(long))
            {
                clampedValue = (long)Math.Clamp(result, long.MinValue, long.MaxValue);
            }
            else if (typeof(T) == typeof(ulong))
            {
                clampedValue = (ulong)Math.Clamp(result, ulong.MinValue, ulong.MaxValue);
            }
            else
            {
                throw new ArgumentException($"Type {typeof(T)} is not supported for clamping.");
            }

            // Convert the clamped value back to type T
            return (T)Convert.ChangeType(clampedValue, typeof(T));
        }

        public static short ConvertToInt16WithClamp(float value)
        {
            value = Math.Clamp(value, short.MinValue, short.MaxValue);
            return (short)value;
        }
        public static int ConvertToInt32WithClamp(float value)
        {
            value = Math.Clamp(value, int.MinValue, int.MaxValue);
            return (int)value;
        }

        public static uint ConvertToUInt32WithClamp(float value)
        {
            value = Math.Clamp(value, uint.MinValue, uint.MaxValue);
            return (uint)value;
        }

        public static Int64 ConvertToInt64WithClamp(float value)
        {
            value = Math.Clamp(value, Int64.MinValue, Int64.MaxValue);
            return (Int64)value;
        }

        internal void SetModbusInput(List<ModbusInput> inputs)
        {
            list = inputs;
        }
    }
}
