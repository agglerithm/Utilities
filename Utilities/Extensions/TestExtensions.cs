using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilities.Extensions
{
    public static class TestExtensions
    {
        public static void ShouldEqual<T>(this T val1, T val2)
        {
            Assert.AreEqual(val1, val2);
        }
        public static void ShouldNotEqual<T>(this T val1, T val2)
        {
            Assert.AreNotEqual(val1, val2);
        }
        public static void ShouldBeNull<T>(this T val)
        {
            Assert.IsNull(val);
        }
        public static void ShouldNotBeNull<T>(this T val)
        {
            Assert.IsNotNull(val);
        }
    }
}