
using queue_simulation_engine.DescriptionMessages;
using queue_simulation_engine.InputValidator;
using queue_simulation_engine.Models;

Console.WriteLine("The store has opened.");

int NWorkers = InputValidator.ReadInteger("Indicate the number of workers:", 1);
int MeanOrderTime = InputValidator.ReadInteger("Indicate the average time until order is filled (in minutes):", 1, 60);
int MeanNewArrivalTime = InputValidator.ReadInteger("Indicate the average time until a new customer arrives (in minutes):", 1, 60);
int MeanCancellationTime = InputValidator.ReadInteger("Indicate the average time until a client is fed up and leaves (in minutes)", 5, 300);

DescriptionMessages.DescribeUserInputs(NWorkers, MeanOrderTime, MeanNewArrivalTime, MeanCancellationTime);

var simulation = new QueueSystem(NWorkers, MeanOrderTime, MeanNewArrivalTime, MeanCancellationTime);
simulation.StartDay();