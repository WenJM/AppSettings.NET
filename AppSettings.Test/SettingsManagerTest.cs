using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppSettings.Client;

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
    }
}
