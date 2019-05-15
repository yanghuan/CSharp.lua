using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3795 - {0}")]
    public class Bridge3795
    {
        [Reflectable]
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class DynamicPropertyAttribute : Attribute
        {
        }

        [Reflectable]
        public class RootViewItem
        {
            [DynamicProperty]
            public virtual bool ReadOnly { get; set; }
        }

        [Reflectable]
        public class PropertyViewItem : RootViewItem
        {
            public override bool ReadOnly { get; set; }
        }

        [Reflectable]
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        public class DynamicProperty2Attribute : Attribute
        {
        }

        [Reflectable]
        public class RootViewItem2
        {
            [DynamicProperty2]
            public virtual bool ReadOnly { get; set; }
        }

        [Reflectable]
        public class PropertyViewItem2 : RootViewItem2
        {
            [DynamicProperty2]
            public override bool ReadOnly { get; set; }
        }

        [Reflectable]
        public class RootViewItem3
        {
            public virtual bool ReadOnly { get; set; }
        }

        [Reflectable]
        public class PropertyViewItem3 : RootViewItem3
        {
            public override bool ReadOnly { get; set; }
        }

        [Reflectable]
        public class RootViewItem4
        {
            public virtual bool ReadOnly { get; set; }
        }

        [Reflectable]
        public class PropertyViewItem4 : RootViewItem4
        {
            [DynamicProperty]
            public override bool ReadOnly { get; set; }
        }

        [Test]
        public static void TestReflection()
        {
            var type = typeof(PropertyViewItem);
            var property = type.GetProperty("ReadOnly");
            Assert.AreEqual("ReadOnly", property.Name, "Retrieved property name is 'ReadOnly'.");
            Assert.AreEqual(typeof(PropertyViewItem), property.DeclaringType, "Retrieved property's declaring type is PropertyViewItem.");

            var attr = property.GetCustomAttributes().OfType<DynamicPropertyAttribute>().FirstOrDefault();
            Assert.NotNull(attr, "Retrieved property has a custom attribute.");

            type = typeof(PropertyViewItem);
            var properties = type.GetProperties();
            Assert.AreEqual(1, properties.Length, "PropertyViewItem has exactly one property.");
            Assert.AreEqual("ReadOnly", properties[0].Name, "Only property in PropertyViewItem is 'ReadOnly'.");
            Assert.AreEqual(typeof(PropertyViewItem), properties[0].DeclaringType, "Property's declaring type is PropertyViewItem.");

            attr = properties[0].GetCustomAttributes().OfType<DynamicPropertyAttribute>().FirstOrDefault();
            Assert.NotNull(attr, "Property has one 'DynamicProperty' custom attribute.");

            type = typeof(PropertyViewItem2);
            properties = type.GetProperties();
            Assert.AreEqual(1, properties.Length, "PropertyViewItem2 has exactly one property.");
            Assert.AreEqual("ReadOnly", properties[0].Name, "Only property in PropertyViewItem2 is 'ReadOnly'.");
            Assert.AreEqual(typeof(PropertyViewItem2), properties[0].DeclaringType, "Property's declaring type is PropertyViewItem2.");

            var attrs = properties[0].GetCustomAttributes().OfType<DynamicProperty2Attribute>();
            Assert.AreEqual(2, attrs.Count(), "Property has two 'DynamicProperty2' custom attributes.");

            type = typeof(PropertyViewItem3);
            properties = type.GetProperties();
            Assert.AreEqual(1, properties.Length, "PropertyViewItem3 has exactly one property.");
            Assert.AreEqual("ReadOnly", properties[0].Name, "Only property in PropertyViewItem3 is 'ReadOnly'.");
            Assert.AreEqual(typeof(PropertyViewItem3), properties[0].DeclaringType, "Property's declaring type is PropertyViewItem3.");

            attr = properties[0].GetCustomAttributes().OfType<DynamicPropertyAttribute>().FirstOrDefault();
            Assert.Null(attr, "Property has no 'DynamicProperty' custom attribute.");
            attrs = properties[0].GetCustomAttributes().OfType<DynamicProperty2Attribute>();
            Assert.AreEqual(0, attrs.Count(), "Property has no 'DynamicProperty2' custom attribute.");

            type = typeof(PropertyViewItem4);
            properties = type.GetProperties();
            Assert.AreEqual(1, properties.Length, "PropertyViewItem4 has exactly one property.");
            Assert.AreEqual("ReadOnly", properties[0].Name, "Only property in PropertyViewItem4 is 'ReadOnly'.");
            Assert.AreEqual(typeof(PropertyViewItem4), properties[0].DeclaringType, "Property's declaring type is PropertyViewItem4.");

            attr = properties[0].GetCustomAttributes().OfType<DynamicPropertyAttribute>().FirstOrDefault();
            Assert.NotNull(attr, "Property has one 'DynamicProperty' custom attribute.");
        }
    }
}