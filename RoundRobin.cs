using System.ComponentModel;

namespace SchedulerSimulator
{
    public class RoundRobin : SchedulingAlgorithm
    {
        public string[] QueueTypes { get; } = { "HIGH_PRIORITY", "LOW_PRIORITY", "DISK", "PRINTER", "TAPE" };
        public string[] ReadyQueueTypes { get; } = { "HIGH_PRIORITY", "LOW_PRIORITY" };
        public string[] IOQueueTypes { get; } = { "DISK", "PRINTER", "TAPE" };

        public const int TIME_SLICE = 4;

        private CPU cpu;
        BindingList<SchedulerQueue> queues;

        private int timeSliceClock = 1;
        private bool finishedIOOperation = false;

        // Used to be able to say if the cpu has started executing a new process, so that 'timeSliceClock' can be updated correctly
        private bool cpuIdleDuringLastClockSignal = true;
        
        // Relates the type of IO request (key) to the appropriate queue type for a process making that request (value)
        public readonly Dictionary<string, string> queueBasedOnRequest = new Dictionary<string, string>
        {
            { "DISK", "DISK" },
            { "PRINTER", "PRINTER" },
            { "TAPE", "TAPE" }
        };

        // Relates the type of IO operation that was done (key) to the appropriate queue type for a process that executed that operation (value)
        public readonly Dictionary<string, string> queueBasedOnFinishedIO = new Dictionary<string, string>
        {
            { "DISK", "LOW_PRIORITY" },
            { "PRINTER", "HIGH_PRIORITY" },
            { "TAPE", "HIGH_PRIORITY" }
        };

        // Relates the process status (key) to the appropriate queue type for that process (value)
        public readonly Dictionary<string, string> queueBasedOnProcessStatus = new Dictionary<string, string>
        {
            { "NEW", "HIGH_PRIORITY" },
            { "PREEMPTED", "LOW_PRIORITY" }
        };

        public RoundRobin(CPU cpu, BindingList<SchedulerQueue> queues)
        {
            this.cpu = cpu;
            this.queues = queues;
        }

        public void Run()
        {
            Console.WriteLine("Run");

            SimulateIOOperations();
            if (finishedIOOperation)
                UpdateQueues();

            // When a new process is pushed into the cpu, the clock of the process' time slice should be reset
            if (NewProcessPushed(cpu))
                ResetTimeSliceClock(1);

            cpuIdleDuringLastClockSignal = cpu.IsIdle();

            if (ClockInterrupt() && !cpu.IsIdle())
            {
                Process p = cpu.Pop();
                p.Preempt();
                Schedule(p, queues);
                ResetTimeSliceClock(1);
            }

            if (!cpu.IsIdle())
                timeSliceClock++;
        }

        private bool NewProcessPushed(CPU cpu)
        {
            Console.WriteLine("NewProcessPushed");
            if (cpuIdleDuringLastClockSignal)
                if(!cpu.IsIdle())
                    return true;
            return false;
        }

        public void Schedule(List<Process> processes, BindingList<SchedulerQueue> queues)
        {
            Console.WriteLine("Schedule");
            foreach (Process process in processes)
                Schedule(process, queues);
        }

        public void Schedule(Process process, BindingList<SchedulerQueue> queues)
        {
            Console.WriteLine("Schedule");
            string appropriateQueueType = GetAppropriateQueueType(process);
            Console.WriteLine($"APPROPRIATE QUEUE TYPE: {appropriateQueueType}");
            SchedulerQueue queue = SchedulerQueue.GetQueue(queues, appropriateQueueType);

            queue.Enqueue(process);
        }

        private string GetAppropriateQueueType(Process p)
        {
            Console.WriteLine("GetAppropriateQueueType");
            string appropriateQueueType;

            if (p.HasRequest())
                appropriateQueueType = queueBasedOnRequest[p.GetIORequest()];
            else if (p.HasFinishedIOOperation())
                appropriateQueueType = queueBasedOnFinishedIO[p.GetIORequest()];
            else
                appropriateQueueType = queueBasedOnProcessStatus[p.GetStatus()];

            return appropriateQueueType;
        }

        public Process? GetReadyProcess(BindingList<SchedulerQueue> queues)
        {
            Console.WriteLine("GetReadyProcess");
            SchedulerQueue queue;
            Process? readyProcess = null;

            foreach(string type in ReadyQueueTypes)
            {
                if(queues.Any(q => q.GetQueueType().Equals(type)))
                {
                    queue = (from q in queues
                             where q.GetQueueType().Equals(type)
                             select q).First();

                    if (queue.Count > 0)
                    {
                        readyProcess = queue.Dequeue();
                        break;
                    }
                }
            }

            return readyProcess;
        }

        private void SimulateIOOperations()
        {
            Console.WriteLine("SimulateIOOperations");
            BindingList<SchedulerQueue> IOQueues = IO.GetIOQueues(queues);
            foreach (SchedulerQueue queue in IOQueues)
            {
                if (queue.Count > 0)
                {
                    Process p = queue.Peek();
                    p.SimulateIOExecution();
                    if (p.HasFinishedIOOperation())
                        finishedIOOperation = true;
                }
            }
        }

        private void UpdateQueues()
        {
            Console.WriteLine("UpdateQueues");
            BindingList<SchedulerQueue> IOQueues = IO.GetIOQueues(queues);
            foreach (SchedulerQueue queue in IOQueues)
            {
                if (queue.Count > 0)
                {
                    Process p = queue.Peek();
                    if (p.HasFinishedIOOperation())
                    {
                        p = queue.Dequeue();
                        Schedule(p, queues);
                    }
                }
            }
        }

        public bool ClockInterrupt()
        {
            Console.WriteLine("ClockInterrupt");
            Console.WriteLine($"timeSliceClock:{timeSliceClock} TIME_SLICE:{TIME_SLICE}");
            if (timeSliceClock % TIME_SLICE == 0)
                return true;
            return false;
        }

        public void ResetTimeSliceClock(int value = 0)
        {
            Console.WriteLine("ResetTimeSliceClock");
            timeSliceClock = value;
        }
    }
}
