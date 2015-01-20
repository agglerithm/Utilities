using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace $rootnamespace$.TestUtils
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

        public static void ShouldBeGreaterThan<T>(this T val1, T val2)
        { 
            Assert.IsTrue(val1.GreaterThan(val2));
        }
        public static void ShouldBeGreaterThanOrEqualTo<T>(this T val1, T val2)
        { 
            Assert.IsTrue(val1.GreaterThan(val2));
        }

        public static void ShouldBeLessThan<T>(this T val1, T val2)
        {
            Assert.IsTrue(val1.LessThan(val2));
        }
        

        public static void ShouldBeLessThanOrEqualTo<T>(this T val1, T val2)
        {
            Assert.IsTrue(val1.LessThan(val2) || val1.EqualTo(val2));
        }

        private static bool EqualTo<T>(this T val1, T val2)
        {
            return Compare(val1,val2) == 0;
        }
        
        private static bool GreaterThan<T>(this T val1, T val2)
        {
            return Compare(val1,val2) == 1;
        }

        private static bool LessThan<T>(this T val1, T val2)
        {
            return Compare(val1,val2) == -1;
        }

        public static int Compare(object tVal, object other)
        {
            var mType = tVal.GetType();
            if (mType == typeof (int))
            {
                return new IntCompare(tVal).CompareTo(other);
            }
            if (mType == typeof (decimal))
            {
                return new DecimalCompare(tVal).CompareTo(other);
            }
            if (mType == typeof (char))
            {
                return new CharCompare(tVal).CompareTo(other);
            }
            if (mType == typeof (string))
            { 
                return String.CompareOrdinal((string)tVal, (string)other);
            }
            if (mType == typeof (DateTime))
            {
                return new DateTimeCompare(tVal).CompareTo(other);
            }
            throw new Exception("Unknown type!");
        }
    }

      internal abstract class CompareBase<T>  
        {
                protected T tVal; 

            protected CompareBase(object val)
                {
                    tVal = (T) val; 
                }

            public abstract int CompareTo(object other);
        }
        internal class IntCompare : CompareBase<int>
        {
            internal IntCompare(object val) : base(val)
            {
                
            }
            public override int CompareTo(object other)
            {
                if (tVal == (int)other) return 0;
                if (tVal > (int)other) return 1;
                return -1;
            }
        }        
        
        internal class DecimalCompare : CompareBase<decimal>
        {
            internal DecimalCompare(object val) : base(val)
            {
                
            }
            public override int CompareTo(object other)
            {
                if (tVal == (decimal)other) return 0;
                if (tVal > (decimal)other) return 1;
                return -1;
            }
        }
        
        internal class DateTimeCompare : CompareBase<DateTime>
        {
            internal DateTimeCompare(object val) : base(val)
            {
                
            }
            public override int CompareTo(object other)
            {
                if (tVal == (DateTime)other) return 0;
                if (tVal > (DateTime)other) return 1;
                return -1;
            }
        }        
        
        internal class CharCompare : CompareBase<char>
        {
            internal CharCompare(object val) : base(val)
            {
                
            }
            public override int CompareTo(object other)
            {
                if (tVal == (char)other) return 0;
                if (tVal > (char)other) return 1;
                return -1;
            }
        }
}