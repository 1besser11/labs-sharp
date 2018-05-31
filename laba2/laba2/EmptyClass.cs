using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace laba2
{
    internal class Program
    {
        private static void Main()
        {
            var series = new Series<Integer>(5);

            series[0] = new Integer(2);
            series[1] = new Integer(3);
            series[2] = new Integer(4);
            series[3] = new Integer(1);
            series[4] = new Integer(5);


            Console.WriteLine("Series 1");
            Console.WriteLine(series);
            var series2 = series.DeepCopy();
            series[0] = series[1] + series[2];
            Console.WriteLine("Changed Series 1");
            Console.WriteLine(series);
            Console.WriteLine("Series 2");
            Console.WriteLine(series2);

            series[3] = series[2] / series[1];
            var series3 = new Series<Real>(5);
            try
            {
                series3[3] = series[2].ToReal() / new Real(0);
            }
            catch (CustomDivideByZero e)
            {
                Console.WriteLine(e.Message);
            }


            try
            {
                series3[50] = series[2].ToReal() / new Real(2);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            var series4 = new Series<Integer>(0);
            try
            {
                series4.OrderBy();
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


    class CustomInvalidCast : InvalidCastException
    {
        public CustomInvalidCast()
            : base() { }

        public CustomInvalidCast(string message)
            : base(message) { }

        public CustomInvalidCast(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public CustomInvalidCast(string message, Exception innerException)
            : base(message, innerException) { }

        public CustomInvalidCast(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }

    class CustomArrayTypeMismatch : ArrayTypeMismatchException
    {
        public CustomArrayTypeMismatch()
            : base() { }

        public CustomArrayTypeMismatch(string message)
            : base(message) { }

        public CustomArrayTypeMismatch(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public CustomArrayTypeMismatch(string message, Exception innerException)
            : base(message, innerException) { }

        public CustomArrayTypeMismatch(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }


    class CustomDivideByZero : DivideByZeroException
    {
        public CustomDivideByZero()
            : base() { }

        public CustomDivideByZero(string message)
            : base(message) { }

        public CustomDivideByZero(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public CustomDivideByZero(string message, Exception innerException)
            : base(message, innerException) { }

        public CustomDivideByZero(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }

    [Serializable]
    public class Series<T>
        where T : class, IComparable<T>
    {
        public Series<T> DeepCopy()
            
        {
            
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (Series<T>)formatter.Deserialize(ms);
            }

        }
        private static void Show_Message(string message)
        {
            Console.WriteLine(message);
        }
        public delegate void AddedNewValue(string message);
        public event AddedNewValue Added;


        private T[] array;

        public Series(int size)
        {
            this.Added += Show_Message;

            array = new T[size];
        }

        public int Length
        {
            get { return array.Length; }
        }

        public void OrderBy(bool desc = false)
        {
            if(desc == true)
                Sort((x, y) => x.CompareTo(y) < 0);
            else
                Sort((x, y) => x.CompareTo(y) > 0);
        }

        protected void Sort(Func<T, T, bool> func)
        {
            if (Length == 0)
                throw new IndexOutOfRangeException();
            for (var i = 0; i < Length - 1; i++)
                for (var j = i + 1; j < Length; j++)
                    if (func(array[i], array[j]))
                    {
                        var temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
        }

        public T this[int index]
        {
            get {
                if (index >= Length)
                    throw new IndexOutOfRangeException();
                return array[index]; 
            }

            set { if (Added != null)
                {
                    if (index >= Length)
                        throw new IndexOutOfRangeException();
                    Added($"Added value {value} with index {index}");
                }
                array[index] = value; }
        }
        public override string ToString()
        {
            string res = "";
            for (var i = 0; i < Length; i++)
                res = string.Concat(res, string.Concat(array[i] + "\n"));
            return res;
        }

    }
    [Serializable]
    public class Real : Number<double>
    {
        public Real(double value = 0)
        {
            Value = value;
        }

        public static Real operator +(Real one, Real other)
        {
            return new Real(one.Value + other.Value);
        }
        public static Real operator -(Real one, Real other)
        {
            return new Real(one.Value - other.Value);
        }
        public static Real operator *(Real one, Real other)
        {
            return new Real(one.Value * other.Value);
        }
        public static Real operator /(Real one, Real other)
        {
            if (DoubleEquals(other.Value, 0))
            {
                Console.WriteLine("Divsion by zero");
                throw new CustomDivideByZero();
            }
            return new Real(one.Value / other.Value);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Real))
                return false;
            else
                return DoubleEquals(((Real)obj).Value, Convert.ToDouble(Value));
            

        }
        public Real DeepCopy()
        {
            Real obj = new Real(this.Value);
            return obj;
        }

        private static bool DoubleEquals(double a, double b, double epsilon = 0.0001)
        {
            return Math.Abs(a - b) < epsilon;
        }
        public override int GetHashCode()
        {
            return Convert.ToInt32(this.Value * 10000);
        }

        public static bool operator ==(Real one, Real two)
        {
            if ((Object)one == null || (Object)two == null)//проверить на null
                return false;

            return one.Equals(two);
        }
        public static bool operator !=(Real one, Real two)
        {
            if ((Object)one == null || (Object)two == null)//проверить на null
                return true;

            return !one.Equals(two);
        }
    }
    [Serializable]
    public class Integer : Number<int>
    {
        
        public Integer(int value = 0)
        {
            Value = value;
        }

        public static Integer operator +(Integer one, Integer other)
        {
            return new Integer(one.Value + other.Value);
        }

        public static Integer operator -(Integer one, Integer other)
        {
            return new Integer(one.Value - other.Value);
        }
        public static Integer operator *(Integer one, Integer other)
        {
            return new Integer(one.Value * other.Value);
        }
        public static Integer operator /(Integer one, Integer other)
        {
            
            if (other.Value == 0)
            {
                Console.WriteLine("Divsion by zero");
                throw new CustomDivideByZero();
            }
            return new Integer(one.Value / other.Value);
        }

        public Real ToReal()
        {
            return new Real(Convert.ToDouble(this.Value));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Number<int>))
                return false;
            else
                return ((Number<int>)obj).Value == this.Value;

        }
        public Integer DeepCopy ()
        {
            Integer obj = new Integer(this.Value);
            return obj;
        }
       
        public override int GetHashCode()
        {
            try
            {
                return Convert.ToInt32(this.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        public static bool operator ==(Integer one, Integer two)
        {
            if ((Object)one == null || (Object)two == null)//проверить на null
                return false;

            return one.Equals(two);
        }
        public static bool operator !=(Integer one, Integer two)
        {
            if ((Object)one == null || (Object)two == null)//проверить на null
                return true;

            return !one.Equals(two);
        }
    }

    [Serializable]
    public abstract class Number<T> : IComparable<Number<T>>
        where T : struct, IComparable<T>
    {
        public T Value { get; protected set; }
        public int CompareTo(Number<T> other)
        {
            
                if (other == null)
                    throw new CustomInvalidCast();
                return Value.CompareTo(other.Value);
           
        }

        public override string ToString()
        {
            return string.Format("{0} of type {1}", Value, GetType().Name);
        }
        //public abstract Number<T> DeepCopy();
       

    }
}