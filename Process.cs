using System.Diagnostics;

namespace SchedulerSimulator
{
    public struct IOOperation
    {
        // Relates the time when a process makes a request to the status the process assumes to indicate it made the request. 
        // The time when a process makes a request (dict key) indicates the time the process has been executed for. 
        // For example, the pair {2, SimulatorConfig.ProcessStatus.DISK_OP_REQUEST} indicates that when the process 
        // has been executed for two clock 'pulses', it makes a request to access the DISK peripheral
        public Dictionary<int, string> IORequestAtGivenTime;
        public string currentIORequest;
        public int currentOperationRemainingTime;
    }

    public class Process
    {
        public readonly string[] statuses = { "NEW", "PREEMPTED", "IO_REQUEST", "FINISHED_IO", "DONE" };
        public const int MAX_ARRIVAL_TIME = 10;
        public const int MAX_SERVICE_TIME = 15;

        private int _id;
        private int _totalServiceTime;
        private int _remainingServiceTime;

        public int ID { get; set; }
        private string Status { get; set; } = "NEW";
        private int arrivalTime = 1;
        
        private int timeExecuted = 0;
        private IOOperation IOOperation;

        public Process() { }

        public Process (int id) { ID = id; }

        // FOR DISPLAYING DATA GRAPHICALLY
        public string IDStr 
        { 
            get
            {
                return $"ID: {ID}";
            }
        }

        public int TotalServiceTime
        {
            get { return _totalServiceTime; }
            set 
            { 
                _totalServiceTime = value;
                _remainingServiceTime = value;
            }
        }

        public string RemainingServiceTime 
        { 
            get 
            { 
                return $"Time: {_remainingServiceTime}";
            }
        }

        public int ArrivalTime
        {
            get { return arrivalTime; }
            set { arrivalTime = value; }
        }

        public int GetID()
        {
            Console.WriteLine("GetID");
            return ID;
        }

        public string GetStatus()
        {
            Console.WriteLine("GetStatus");
            return Status;
        }

        public int GetArrivalTime()
        {
            Console.WriteLine("GetArrivalTime");
            return arrivalTime;
        }

        public int GetRemainingServiceTime() 
        {
            Console.WriteLine("GetRemainingServiceTime");
            return _remainingServiceTime;
        }

        public void SetStatus(string status)
        {
            Console.WriteLine("SetStatus");
            Status = status;
        }

        public bool HasRequest()
        {
            Console.WriteLine("HasRequest");
            if (Status == "IO_REQUEST")
                return true;
            return false;
        }

        public string GetIORequest()
        {
            Console.WriteLine("GetIORequest");
            return IOOperation.currentIORequest;
        }

        public bool HasFinishedExecution()
        {
            Console.WriteLine("HasFinishedExecution");
            if (Status == "DONE")
                return true;
            return false;
        }

        public bool HasFinishedIOOperation()
        {
            Console.WriteLine("HasFinishedIOOperation");
            if(Status == "FINISHED_IO")
                return true;
            return false;
        }

        public void Preempt()
        {
            SetStatus("PREEMPTED");
        }

        public void SimulateExecution()
        {
            Console.WriteLine("SimulateExecution");
            _remainingServiceTime--;
            timeExecuted++;
            Console.WriteLine($"Time Executed: {timeExecuted}");
            if (IOOperation.IORequestAtGivenTime != null && IOOperation.IORequestAtGivenTime.ContainsKey(timeExecuted))
            {
                Console.WriteLine("IF");
                Status = "IO_REQUEST";
                IOOperation.currentIORequest = IOOperation.IORequestAtGivenTime[timeExecuted];
                Console.WriteLine($"Status: {Status}");
                IOOperation.currentOperationRemainingTime = IO.operationTime[IOOperation.currentIORequest];
                Console.WriteLine($"IOOperation.currentOperationRemainingTime: {IOOperation.currentOperationRemainingTime}");
            }
            else if (_remainingServiceTime <= 0)
            {
                Console.WriteLine("ELSE");
                Status = "DONE";
            }
                
        }

        public void SimulateIOExecution()
        {
            Console.WriteLine("SimulateIOExecution");
            IOOperation.currentOperationRemainingTime--;
            Console.WriteLine($"Process #{ID} has {IOOperation.currentOperationRemainingTime} remaining");
            if (IOOperation.currentOperationRemainingTime <= 0)
                Status = "FINISHED_IO";
        }

        public static List<Process> CreateProcesses(int numProcesses)
        {
            List<Process> newProcesses = Util.NewList<Process>(numProcesses);
            int processID = 1;
            foreach (Process p in newProcesses)
            {
                p.InitializeSimpleProcess(processID);
                processID++;
            }
            return newProcesses;
        }
        
        public static List<Process> CreateUserProcesses(int numProcesses)
        {
            List<Process> newProcesses = Util.NewList<Process>(numProcesses);
            int processID = 1;
            foreach (Process p in newProcesses)
            {
                //p.InitializeUserProcess(processID);
                processID++;
            }
            return newProcesses;
        }
        
        // FOR TESTS
        private void InitializeSimpleProcess(int processID)
        {
            ID = processID;
            Status = "NEW";
            arrivalTime = processID;
            _remainingServiceTime = 5;
            //_totalServiceTime = RemainingServiceTime.ToString();
            _totalServiceTime = _remainingServiceTime;

            IOOperation.IORequestAtGivenTime = new Dictionary<int, string> { { 2, "PRINTER" } };
            IOOperation.currentOperationRemainingTime = IO.operationTime["PRINTER"];
        }

        private void InitializeProcessRandomly(int processID)
        {
            Random rnd = new System.Random();

            ID = processID;
            Status = "NEW";
            arrivalTime = rnd.Next(1, MAX_ARRIVAL_TIME);
            _remainingServiceTime = rnd.Next(1, MAX_SERVICE_TIME);
            //_totalServiceTime = RemainingServiceTime;
            _totalServiceTime = _remainingServiceTime;

            IOOperation.IORequestAtGivenTime = new Dictionary<int, string>();
            int numIOOperations = rnd.Next(0, 4);

            for (int i = 0; i < numIOOperations; i++)
                IOOperation.IORequestAtGivenTime.Add(i+1, IO.IORequestTypes[i%3]);
        }

        /*
        private void InitializeUserProcess(int processID)
        {
            ID = processID;
            int numIOOps = 0;
            bool validInputPending = true;

            Console.WriteLine($"Enter process #{ID}'s arrival time: ");
            while(validInputPending)
            {
                if (!int.TryParse(Console.ReadLine(), out arrivalTime))
                    Util.PrintColoredMessage("ERROR - INVALID INPUT. \nInput must be a number", ConsoleColor.Red);
                else
                    validInputPending = false;
            }

            validInputPending = true;
            Console.WriteLine($"Enter process #{ID}'s service time: ");
            while(validInputPending)
            {
                if (!int.TryParse(Console.ReadLine(), out remainigServiceTime))
                    Util.PrintColoredMessage("ERROR - INVALID INPUT. \nInput must be a number", ConsoleColor.Red);
                else
                    validInputPending = false;
            }
         
            _totalServiceTime = remainingServiceTime.ToString();

            validInputPending = true;
            Console.WriteLine($"Enter process #{ID}'s number of IO operations (maximum of 3): ");
            while (validInputPending)
            {
                if (!int.TryParse(Console.ReadLine(), out numIOOps) || numIOOps > 3 || numIOOps < 0)
                    Util.PrintColoredMessage("ERROR - INVALID INPUT. \nInput must be a number between 0 and 3", ConsoleColor.Red);
                else
                    validInputPending = false;
            }

            IOOperation.IORequestAtGivenTime = new();

            string[] IOOps = new string[numIOOps];
            string[] IOOpsRequestTimes = new string[numIOOps];

            for(int i = 0; i < numIOOps; i++)
            {
                Console.WriteLine($"Enter the type of operation request: ");
                validInputPending = true;
                while (validInputPending)
                {
                    IOOps[i] = Console.ReadLine();
                    if (IOOps[i] == null || !IO.IORequestTypes.Contains(IOOps[i]))
                        Util.PrintColoredMessage("ERROR - INVALID INPUT. \nValid operation types can be found in 'IO.cs'", ConsoleColor.Red);
                    else
                        validInputPending = false;
                }

                Console.WriteLine($"Enter the time after which the process will make the request: ");
                validInputPending = true;
                while(validInputPending)
                {
                    IOOpsRequestTimes[i] = Console.ReadLine();
                    if(IOOpsRequestTimes[i] == null || !int.TryParse(IOOpsRequestTimes[i], out int throwaway) 
                        || int.Parse(IOOpsRequestTimes[i]) < 1 || int.Parse(IOOpsRequestTimes[i]) > remainigServiceTime)
                    {
                        Util.PrintColoredMessage("ERROR - INVALID INPUT. \nInput must be a number between 1 and the service time of the process.", ConsoleColor.Red);
                    }
                    else
                        validInputPending = false;
                }
            }

            for (int i = 0; i < numIOOps; i++)
            {
                int key = int.Parse(IOOpsRequestTimes[i]);
                IOOperation.IORequestAtGivenTime.Add(key, IOOps[i]);
            }
        }
        */

        public void Print()
        {
            Console.WriteLine("");
            Console.WriteLine($"ID: {GetID()}");
            Console.WriteLine($"ArrivaleTime: {GetArrivalTime()}");
            Console.WriteLine($"ServiceTime: {RemainingServiceTime}");
            Console.WriteLine("IO Operation Requests:");
            if(IOOperation.IORequestAtGivenTime != null)
            {
                foreach (KeyValuePair<int, string> item in IOOperation.IORequestAtGivenTime)
                    Console.Write(item.Key + "=>" + item.Value.ToString() + "\n");
            }
        }
    }
}

// CREATE PROPERTIES FOR THE IO
// HOW?
// HAVE A DROPDOWN WITH THE POSSIBLE NUMBER OF IO OPS
// IS IT POSSIBLE TO HAVE A VARIABLE NUMBER OF COLUMNS IN THE GRID?