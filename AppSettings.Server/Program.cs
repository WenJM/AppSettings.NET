using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppSettings.Client;
using Microsoft.Owin.Hosting;
using System.Threading.Tasks;

namespace AppSettings.Server
{
    static class Program
    {
        static void Main(string[] arg)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                var url = "http://localhost:5000/";
                var startOpts = new StartOptions(url);
                WebApp.Start<Startup>(startOpts);
                Console.WriteLine("站点已启动：" + url);
            });

            while (!task.IsCompleted)
            {
                if (task.IsCompleted)
                {
                    AppSettingConfig.IsCacheConfig = true;
                    AppSettingConfig.ScanInterval = 1000;

                    //初始化配置
                    var zhangsan = AppSettingClient.AppSettings["zhangsan"];
                    var age = AppSettingClient.AttributesValue("person", "age");
                    var person = AppSettingClient<Person>.Load();
                    var orders = AppSettingClient<List<Order>>.Load("Orders");
                    Console.ReadKey();

                    //调试缓存是否存在
                    zhangsan = AppSettingClient.AppSettings["zhangsan"];
                    age = AppSettingClient.AttributesValue("person", "age");
                    person = AppSettingClient<Person>.Load();
                    orders = AppSettingClient<List<Order>>.Load("Orders");
                    Console.ReadKey();

                    //修改配置文件后再调试，缓存消失，缓存依赖策略起作用。
                    zhangsan = AppSettingClient.AppSettings["zhangsan"];
                    age = AppSettingClient.AttributesValue("person", "age");
                    person = AppSettingClient<Person>.Load();
                    orders = AppSettingClient<List<Order>>.Load("Orders");
                    Console.ReadKey();
                }
            }
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
    }

    public class Order
    {
        public decimal Amount { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}
