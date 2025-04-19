using queue_simulation_engine.InputValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace queue_simulation_engine.DescriptionMessages
{
    public static class DescriptionMessages
    {
        public static void DescribeUserInputs(int nWorkers, int meanOrderTime, int meanArrivalTime, int meanCancellationTime)
        {
            Console.WriteLine($"The number of workers in the store is:                                  {nWorkers}");
            Console.WriteLine($"The average time it takes an order to be filled is:                     {meanOrderTime}");
            Console.WriteLine($"The average time it takes a new client to come to the store is:         {meanArrivalTime}");
            Console.WriteLine($"The average time it takes a client to get fed up with waiting is:       {meanCancellationTime}");
        }
    }
}