using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppSettings.NET;

namespace AppSettings.Test
{    
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
    }

    [TestClass]
    public class SettingsManagerTest
    {
        [TestMethod]
        public void GetValueByKey()
        {
            var zhangsan = SettingsManager.AppSettings["zhangsan"];
            Assert.IsNotNull(zhangsan);
        }

        [TestMethod]
        public void GetAttributesValue()
        {
            var age = SettingsManager.GetAttributesValue("person", "age");
            Assert.IsNotNull(age);
        }

        [TestMethod]
        public void GetEntity()
        {
            var person = SettingsManager.GetEntity<Person>();
            Assert.IsNotNull(person);
        }
    }
}
