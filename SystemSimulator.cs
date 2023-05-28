using System;
using System.ComponentModel;
using System.Net;

namespace SchedulerSimulator
{
    public class SystemSimulator
    {
        bool allProcessesTerminated;
        bool newProcessHasArrived;

        public int systemClock = 1;
        bool defaultData = true;
        int numProcess;

        const int DEFAULT_NUM_PROCESSES = 3;

        SchedulingAlgorithm _algorithm;
        string? algChoice;

        public CPU _cpu;
        public BindingList<Process> _memory;
        public BindingList<SchedulerQueue> _queues;
        public Scheduler _scheduler;
        
        
        public SystemSimulator(SchedulingAlgorithm algorithm, CPU cpu, BindingList<Process> memory, BindingList<SchedulerQueue> queues, Scheduler scheduler)
        {
            _algorithm = algorithm;
            _cpu = cpu;
            //_memory = new List<Process>(memory);
            _memory = memory;
            //_memory = new List<Process>(Process.CreateProcesses(3));
            _queues = queues;
            _scheduler = scheduler;
        }

        public int GetQueueCount()
        {
            return _queues.Count;
        }

        public int GetMemoryCount()
        {
            return _memory.Count;
        }

        public bool GetIsCPUIdle()
        {
            if(_cpu.IsIdle())
                return true;
            return false;
        }
        
        
        public void CheckForNewlyArrivedProcesses(BindingList<Process> memory)
        {
            Console.WriteLine("CheckForNewlyArrivedProcesses");
            //! CHANGE FROM 'EXISTS' -> CREATE A FUNCTION THAT TELLS IF THERE'S A PROCESS WITH ARRIVAL TIME EQUAL CURRENT TIME
            // DONT CREATE NEW FUNCTION, USE LINQ
            //bool newArrival = from process in memory        

            /*queue = (from q in queues
                     where q.GetQueueType().Equals(queueType)
                     select q).First();
             * */

            //a.Books.Any(b => b.BookID == bookID)
            if(memory.Any(p => p.GetArrivalTime() == systemClock))
                newProcessHasArrived = true;
            else
                newProcessHasArrived = false;

            /*
            if (memory.Exists(NewArrival))
                newProcessHasArrived = true;
            else
                newProcessHasArrived = false;
            */
        }

        //! THIS IS NOT USED
        private bool NewArrival(Process p)
        {
            Console.WriteLine("NewArrival");
            if (p.GetArrivalTime() == systemClock)
                return true;
            return false;
        }

        public List<Process> GetNewlyArrivedProcesses(BindingList<Process> memory)
        {
            Console.WriteLine("GetNewlyArrivedProcesses");
            List<Process> newlyArrivedProcesses = (from p in memory
                                                  where p.GetArrivalTime() == systemClock
                                                  select p).ToList();
            return newlyArrivedProcesses;
        }        

        public void TerminateProcess(BindingList<Process> memory, Process process)
        {
            Console.WriteLine("TerminateProcess");
            memory.Remove(process);
        }

        public void PreemptProcess(CPU cpu, Scheduler scheduler)
        {
            Console.WriteLine("PreemptProcess");
            Process process = cpu.Pop();

            if(!process.HasRequest())
                process.Preempt();

            scheduler.Schedule(process);
        }

        public void CheckIfAllProcessesDone(BindingList<Process> memory)
        {
            Console.WriteLine("CheckIfAllProcessesDone");

            if (memory.Count == 0)
                allProcessesTerminated = true;
            else
                allProcessesTerminated = false;
        }

        public void PrintProcesses(BindingList<Process> memory)
        {
            Console.WriteLine("PrintProcesses");
            foreach (Process process in memory)
            {
                process.Print();
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool ExecutingProcessMadeRequest(CPU cpu)
        {
            Console.WriteLine("ExecutingProcessMadeRequest");
            if (!cpu.IsIdle())
            {
                Process p = cpu.Peek();
                if (p.GetStatus().Equals("FINISHED_IO"))
                    return false;

                string request = p.GetIORequest();
                Console.WriteLine($"PROCESS ID: {p.GetID()} --------------- IO REQUEST: {request}");

                if (IO.IORequestTypes.Contains(request))
                    return true;
            }
            return false;
        }

        public void GetUserInput()
        {
            bool validInputPending = true;
            Console.WriteLine("Which algorithm should be used? (Round Robin, FIFO)");
            while(validInputPending)
            {
                algChoice = Console.ReadLine();
                if (algChoice == null || (!algChoice.Equals("Round Robin") && !algChoice.Equals("FIFO")))
                    Util.PrintColoredMessage("ERROR - INVALID INPUT. \nPossible algorithms are 'Round Robin' and 'FIFO'. \nPlease choose a valid algorithm.", ConsoleColor.Red);
                else
                    validInputPending = false;
            }

            validInputPending = true;

            Console.WriteLine("Do you wish to input custom data? [y/n]");
            while(validInputPending)
            {
                string? customDataChoice = Console.ReadLine();
                if (customDataChoice == null || (!customDataChoice.Equals("y") && !customDataChoice.Equals("n")))
                    Util.PrintColoredMessage("ERROR - INVALID INPUT. \nPlease type either 'y' or 'n' to choose to input your own data or not.", ConsoleColor.Red);
                else if (customDataChoice == "n")
                {
                    defaultData = true;
                    validInputPending = false;
                }
                else if (customDataChoice == "y")
                {
                    defaultData = false;
                    Console.WriteLine("Enter the number of processes: ");
                    while (validInputPending)
                    {
                        if (!int.TryParse(Console.ReadLine(), out numProcess))
                            Util.PrintColoredMessage("ERROR - INVALID INPUT. \nNumber of processes must be a number.", ConsoleColor.Red);
                        else
                            validInputPending = false;
                    }
                }
            }
        }

        
        public void SimulateExecution()
        {
            Console.WriteLine($"Clock: {systemClock}");
            CheckForNewlyArrivedProcesses(_memory);

            if (newProcessHasArrived)
            {
                List<Process> newlyArrivedProcesses = GetNewlyArrivedProcesses(_memory);
                _scheduler.Schedule(newlyArrivedProcesses);
            }

            if (_cpu.IsIdle())
            {
                Process? readyProcess = _scheduler.GetReadyProcess();
                if (readyProcess != null)
                    _cpu.Push(readyProcess);
            }

            if (!_cpu.IsIdle())
                _cpu.SimulateProcessExecution();

            if (_cpu.FinishedExecutingProcess())
            {
                Process p = _cpu.Pop();
                TerminateProcess(_memory, p);
            }
            else if (ExecutingProcessMadeRequest(_cpu))
                PreemptProcess(_cpu, _scheduler);

            _scheduler.RunSchedulingAlgorithm();

            _scheduler.PrintQueues();

            CheckIfAllProcessesDone(_memory);

            systemClock++;
            Console.WriteLine("\n");
        }
        
        
        /*
        static void Main(string[] args)
        {
            List<Process> memory;
            CPU cpu = new CPU();
            List<SchedulerQueue> queues = new List<SchedulerQueue>();
            
            GetUserInput();
            if (defaultData)
                memory = Process.CreateProcesses(DEFAULT_NUM_PROCESSES);
            else
                memory = Process.CreateUserProcesses(numProcess);

            if (algChoice.Equals("Round Robin"))
                _algorithm = new RoundRobin(cpu, queues);
            else if (algChoice.Equals("FIFO"))
                _algorithm = new FIFO(queues);

            Scheduler scheduler = new Scheduler(_algorithm, queues);

            PrintProcesses(memory);
            allProcessesTerminated = false;

            while (!allProcessesTerminated)
            {
                Console.WriteLine($"Clock: {systemClock}");
                CheckForNewlyArrivedProcesses(memory);

                if (newProcessHasArrived)
                {
                    List<Process> newlyArrivedProcesses = GetNewlyArrivedProcesses(memory);
                    scheduler.Schedule(newlyArrivedProcesses);
                }

                if (cpu.IsIdle())
                {
                    Process? readyProcess = scheduler.GetReadyProcess();
                    if (readyProcess != null)
                        cpu.Push(readyProcess);
                }
  
                if (!cpu.IsIdle())
                    cpu.SimulateProcessExecution();

                if (cpu.FinishedExecutingProcess())
                {
                    Process p = cpu.Pop();
                    TerminateProcess(memory, p);
                }
                else if (ExecutingProcessMadeRequest(cpu))
                    PreemptProcess(cpu, scheduler);

                scheduler.RunSchedulingAlgorithm();

                scheduler.PrintQueues();

                CheckIfAllProcessesDone(memory);

                systemClock++;
                Console.WriteLine("\n");
            }
            
            Console.WriteLine("Simulation finished ");
        }
        */
    }
}
