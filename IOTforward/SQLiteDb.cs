using Dapper;
using IOTforward.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTforward
{
    internal class SQLiteDb
    {

        static string dbPath = Path.Combine(Directory.GetCurrentDirectory(),"5giot.db");  //@".\5g_iot\web\sqlite\5giot.db";
        static string cnStr = "data source=" + dbPath;

        static void InitSQLiteDb()
        {
            if (File.Exists(dbPath)) return;
            using (var cn = new SQLiteConnection(cnStr))
            {
                cn.Execute(@"
CREATE TABLE Player (
    Id VARCHAR(16),
    Name VARCHAR(32),
    RegDate DATETIME,
    Score INTEGER,
    BinData BLOB,
    CONSTRAINT Player_PK PRIMARY KEY (Id)
)");
            }
        }

        //static void TestInsert()
        //{
        //    using (var cn = new SQLiteConnection(cnStr))
        //    {
        //        cn.Execute("DELETE FROM Player");
        //        //參數是用@paramName
        //        var insertScript =
        //            "INSERT INTO Player VALUES (@Id, @Name, @RegDate, @Score, @BinData)";
        //        cn.Execute(insertScript, TestData);
        //        //測試Primary Key
        //        try
        //        {
        //            //故意塞入錯誤資料
        //            cn.Execute(insertScript, TestData[0]);
        //            throw new ApplicationException("失敗：未阻止資料重複");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"測試成功:{ex.Message}");
        //        }
        //    }
        //}

       public static IEnumerable<SvidinfoModel> selectSvidinfo(string addSql)
       {
            using (var cn = new SQLiteConnection(cnStr))
            {
                // 定义 SQL 查询
                string sql = "SELECT * FROM svidinfo "+ addSql;

                // 使用 Dapper 执行查询并获取结果
                IEnumerable<SvidinfoModel> result = cn.Query<SvidinfoModel>(sql);

                return result;
                //// 处理查询结果
                //foreach (var item in result)
                //{
                //    Console.WriteLine($"ID: {item.Id}, Name: ");
                //}
            }
       }

        public static IEnumerable<SvidinfoModel> selectSvidinfoModbusN(string modbusn,string mapfile)
        {
            using (var cn = new SQLiteConnection(cnStr))
            {
                // 定义 SQL 查询
                string sql = @$"SELECT 
                t1.parameterid,
                printf('%0*d', length(t1.serveraddress), CAST(REPLACE(t1.serveraddress, 'N', CAST(t2.modbusn AS CHAR(255))) AS INT)) AS serveraddress,
                t1.serverfunctioncode,printf('%0*d', length(t1.address), CAST(t1.address AS INTEGER) - d.base01+1) AS address,
                t1.num,t1.functioncode,t1.readfreq,t1.tagname,t1.scaletype,t1.scalemultiple,t1.scaleoffset,t1.unit,t1.max,
                t1.min,t1.signed,t1.fixed,t1.startbit,t1.endbit 
                FROM svidinfo t1
                JOIN deviceinfo t2 ON t2.mapfile = '{mapfile}' AND t1.class = t2.mapfile AND t2.modbusn = '{modbusn}'
                JOIN devicedef d ON d.mapfile = t1.class AND t1.class = '{mapfile}' GROUP BY t1.parameterid";

                // 使用 Dapper 执行查询并获取结果
                IEnumerable<SvidinfoModel> result = cn.Query<SvidinfoModel>(sql);

                return result;

            }
        }

        public static IEnumerable<DeviceinfoModel> deviceinfoModel()
        {
            using (var cn = new SQLiteConnection(cnStr))
            {
                // 定义 SQL 查询
                string sql = "SELECT * FROM Deviceinfo";

                // 使用 Dapper 执行查询并获取结果
                IEnumerable<DeviceinfoModel> result = cn.Query<DeviceinfoModel>(sql);

                return result;

            }
        }
    }
}
