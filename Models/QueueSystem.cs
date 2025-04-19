using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace queue_simulation_engine.Models
{
    public class QueueSystem
    {
        private List<Customer> CustomerList = new List<Customer>();
        private List<Customer> ActiveCustomers => CustomerList.Where(c => c.Status == CustomerStatus.Waiting || c.Status == CustomerStatus.BeingServed).ToList();
        private List<Worker> WorkersList { get; set; } = new List<Worker>();
        private bool AvailableWorkers => WorkersList.Where(w => w.Status == WorkerStatus.Free).Count() > 0;
        private List<Worker> AvailableWorkersList => WorkersList.Where(w => w.Status == WorkerStatus.Free).ToList();
        private bool AwaitingCustomers => CustomerList.Where(c => c.Status == CustomerStatus.Waiting).Count() > 0;
        private List<Customer> AwaitingCustomersList => CustomerList.Where(c => c.Status == CustomerStatus.Waiting).ToList();
        private TimeOnly NextClientArrivalSimulation {  get; set; }
        private Poisson ArrivalDistribution { get; }
        public QueueSystem(int nWorkers, int meanOrderTime, int meanArrivalTime, int meanCancellationTime) 
        {
            for (int i = 0; i < nWorkers; i++)
            {
                WorkersList.Add(new Worker(i + 1));
            }
            this.ArrivalDistribution = new Poisson(meanArrivalTime);

        }

        public void StartDay(TimeOnly? startTime = null, TimeOnly? closingTime = null)
        {
            // We hard-code the values for now
            TimeOnly currentTime = new TimeOnly(8, 0);
            NextClientArrivalSimulation = currentTime.AddMinutes(ArrivalDistribution.Sample());
            Console.WriteLine("The store has opened, it's 8AM.");
            closingTime = new TimeOnly(17, 0);

            int currentClient = 0;

            while (currentTime < closingTime)
            {
                List<string> events = new List<string>();
                if(NextClientArrivalSimulation == currentTime)
                {
                    NextClientArrivalSimulation = currentTime.AddMinutes(ArrivalDistribution.Sample());
                    if (NextClientArrivalSimulation == currentTime)
                    {
                        // At least one more minute (we will fix this later)
                        NextClientArrivalSimulation.AddMinutes(1);
                    }
                    CustomerList.Add(new Customer(currentClient++, NextClientArrivalSimulation));
                    Console.WriteLine("Customer arrived");
                }

                // First we check if any order will be fullfilled
                foreach (var worker in WorkersList.Where(w => w.Status == WorkerStatus.Serving)) { 
                    if(worker.IsFinishingNow(currentTime))
                    {
                        UpdateCustomerAfterOrderCompletion((int)worker.CurrentClientId);
                        worker.FreeWorker();
                        // Add event
                        Console.WriteLine("Customer served");
                    }
                }

                if (AvailableWorkers && AwaitingCustomers)
                {
                    // Assign customer to worker (at random)
                    // Add event
                    do
                    {
                        AssignCustomerToWorker(currentTime);
                        // Add event
                        Console.WriteLine("Customer assigned");
                    } while (AvailableWorkers && AwaitingCustomers);
                }

                if (AwaitingCustomers)
                {
                    // Check if customer has been fed up
                    foreach (var awaitingCustomer in AwaitingCustomersList) 
                    {
                        if (awaitingCustomer.IsFedUp(currentTime))
                        {
                            awaitingCustomer.LeaveAngry();
                            // Add event
                            Console.WriteLine("ANGRY CUSTOMER!!!");
                        }
                    }
                }

                Thread.Sleep(100);
                currentTime = currentTime.AddMinutes(1);
                Console.WriteLine(currentTime.ToString());
            }
        }

        public void AssignCustomerToWorker(TimeOnly currentTime)
        {
            // Move to functions
            var firstClientInLine = AwaitingCustomersList.First();
            firstClientInLine.SetAsBeingServed();
            AvailableWorkersList.First().AssignCustomer(firstClientInLine, currentTime);
        }

        public void UpdateCustomerAfterOrderCompletion(int customerId)
        {
            var customer = ActiveCustomers.Where(c => c.Id == customerId).Single();
            customer.Leave();
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public int PositionInLine { get; set; }
        public int WaitToleranceInMinutes { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public TimeOnly? ServedTime { get; private set; }
        public TimeOnly LimitTime => ArrivalTime.AddMinutes(WaitToleranceInMinutes);
        public CustomerStatus Status { get; set; }
        public int ServerId { get; set; }

        public Customer(int id, TimeOnly arrivalTime)
        {
            Id = id;
            ArrivalTime = arrivalTime;
            //hard-code
            WaitToleranceInMinutes = 10;
        }

        public bool IsFedUp(TimeOnly currentTime)
        {
            return currentTime == ArrivalTime;
        }

        public void SetAsBeingServed()
        {
            Status = CustomerStatus.BeingServed;
        }

        public void LeaveAngry()
        {
            Status = CustomerStatus.LeftBeforeOrderFilled;
        }

        public void Leave()
        {
            Status = CustomerStatus.OrderFilled;
        }
    }

    public class Worker
    {
        public int Id { get; set; }
        public WorkerStatus Status { get; set; }
        public int? CurrentClientId { get; set; }
        public TimeOnly? EndOfOrderTime { get; set; }
        public bool IsFree => CurrentClientId == null;

        public Worker(int id)
        {
            Id = id;
            Status = WorkerStatus.Free;
            CurrentClientId = null;
        }

        public bool IsFinishingNow(TimeOnly currentTime)
        {
            return Status == WorkerStatus.Serving && EndOfOrderTime == currentTime;
        }

        public void AssignCustomer(Customer customer, TimeOnly currentTime)
        {
            CurrentClientId = customer.Id;
            Status = WorkerStatus.Serving;
            EndOfOrderTime = currentTime.AddMinutes(5);
        }

        public void FreeWorker()
        {
            Status = WorkerStatus.Free;
            EndOfOrderTime = null;
            CurrentClientId = null;
        }
    }

    public enum CustomerStatus
    {
        Waiting,
        BeingServed,
        OrderFilled,
        LeftBeforeOrderFilled
    }

    public enum WorkerStatus
    {
        Serving,
        Free
    }
}


// COMMENTS

// We need to keep track of:

// How many people are being served
// How many people are waiting in line
// When will each people waiting will be fed up and leave
// When a new client will enter the store

// The first important question is how will we store this information
// I also want to give the people (both workers and clients) an identifier to know who does what

// As improvements we could consider:
// - Clients who are regulars (come always at a certain hour)
// - Workers who are faster than others
// - Times of day when people come more to the store
// - Possibility of newcomers to leave immediately when they see a lot of people waiting in line