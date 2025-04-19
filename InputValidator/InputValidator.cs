using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace queue_simulation_engine.InputValidator
{
    public static class InputValidator
    {
        private static string NULL_INPUT = "Please write a value before pressing Enter. ";
        private static string NON_INTEGER_INPUT = "Please write a valid integer. ";
        public static int ReadInteger(string msg, int? minValue = null, int? maxValue = null)
        {
            string msg_display = msg;
            while (true) 
            {
                Console.WriteLine(msg_display);
                string? user_input = Console.ReadLine();
                if (user_input == null)
                {
                    msg_display = NULL_INPUT + msg;
                    continue;
                }
                if(Int32.TryParse(user_input, out int value))
                {
                    return GetValueInsideOfBounds(value, minValue, maxValue);
                }
                msg_display = NON_INTEGER_INPUT + msg;
            }
        }

        private static int GetValueInsideOfBounds(int value, int? minValue = null, int? maxValue = null)
        {
            if (minValue != null && value < minValue)
            {
                Console.WriteLine($"The value defined was lower than the minimum value. The value has been adjusted to {minValue}");
                return (int)minValue;
            }
            if (maxValue != null && value > maxValue)
            {
                Console.WriteLine($"The value defined was greater than the maximum value. The value has been adjusted to {maxValue}");
                return (int)maxValue;
            }
            return value;
        }
    }
}
