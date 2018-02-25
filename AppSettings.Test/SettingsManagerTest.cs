using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppSettings.Client;
using System.Configuration;

namespace AppSettings.Test
{    
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

    [TestClass]
    public class SettingsManagerTest
    {
        [TestMethod]
        public void GetValueByKey()
        {
            var zhangsan = AppSettingClient.AppSettings["zhangsan"];
            Assert.IsNotNull(zhangsan);
        }

        [TestMethod]
        public void GetAttributesValue()
        {
            var age = AppSettingClient.AttributesValue("person", "age");
            Assert.IsNotNull(age);
        }

        [TestMethod]
        public void GetEntity()
        {
            var person = AppSettingClient<Person>.Load();
            Assert.IsNotNull(person);
        }

        [TestMethod]
        public void GetEntitys()
        {
            var orders = AppSettingClient<List<Order>>.Load("Orders");

            Assert.IsNotNull(orders);
            Assert.IsTrue(orders.Any());
            Assert.IsTrue(orders.Count == 2);
        }

        [TestMethod]
        public void RunTest()
        {
            AppSettingConfig.IsLoadCache = false; //缓存配置

            RunStart.Run("系统配置读取",100000, (i)=>
            {
                string filePath = ConfigurationManager.AppSettings["AppSettingsPath"];
                if (i == 0)
                {
                    Console.WriteLine($"AppSettingsPath：{filePath}");
                }
            });

            RunStart.Run("普通配置读取", 100000, (i) =>
            {
                var zhangsan = AppSettingClient.AppSettings["zhangsan"];
                if (i == 0)
                {
                    Console.WriteLine($"zhangshan：{zhangsan}");
                }
            });

            RunStart.Run("属性配置读取",100000, (i) =>
            {
                var age = AppSettingClient.AttributesValue("person", "age");
                if (i == 0)
                {
                    Console.WriteLine($"person：age={age}");
                }
            });

            RunStart.Run("类型配置读取",100000, (i) =>
            {
                var person = AppSettingClient<Person>.Load();
                if (i == 0)
                {
                    Console.WriteLine($"person：Name={person.Name},Age={person.Age},Height={person.Height}");
                }
            });

            RunStart.Run("类型集合配置读取",100000, (i) =>
            {
                var orders = AppSettingClient<List<Order>>.Load("Orders");
                if (i == 0)
                {
                    foreach (var o in orders)
                    {
                        Console.WriteLine($"order：Name={o.Name},Code={o.Code},Amount={o.Amount}");
                    }
                }
            });
        }
    }
}
