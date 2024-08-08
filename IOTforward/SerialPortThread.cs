using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTforward
{
    public class SerialPortThread
    {
        public SerialPortThread(string portName,int baudRate,int parity,int stopBits,int dataBits,int handshake)
        {
            SerialPort mySerialPort = new SerialPort(portName);//new SerialPort("COM1");

            // 设置波特率、数据位、停止位和奇偶校验
            mySerialPort.BaudRate = baudRate;//9600;
            mySerialPort.Parity = (Parity)parity;//Parity.None;
            mySerialPort.StopBits = (StopBits)stopBits;//StopBits.One;
            mySerialPort.DataBits = dataBits;// 8;
            mySerialPort.Handshake = (Handshake)handshake;// Handshake.None;

            // 设置读取和写入超时时间（毫秒）
            mySerialPort.ReadTimeout = 500;   // 500毫秒读取超时
            mySerialPort.WriteTimeout = 500;  // 500毫秒写入超时


            while (true)
            {
                try
                {
                    if (!mySerialPort.IsOpen)
                    {
                        mySerialPort.Open();
                    }

                    mySerialPort.WriteLine("Hello, World!");

                    // 等待数据接收
                    string message = mySerialPort.ReadLine();
                    Console.WriteLine("Received message: " + message);
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine("操作超时：" + ex.Message);
                    ReopenSerialPort(mySerialPort);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("其他异常：" + ex.Message);
                    break;
                }
                finally
                {
                    // 延迟一段时间后再重试
                    Thread.Sleep(1000);
                }
            }

            mySerialPort.Close();
        }

        private static void ReopenSerialPort(SerialPort serialPort)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("重新打开串行端口时出错：" + ex.Message);
            }
        }

    }
}
