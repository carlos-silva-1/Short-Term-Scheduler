using System.ComponentModel;

namespace SchedulerSimulator
{
    public class FIFO : SchedulingAlgorithm
    {
        public string[] QueueTypes { get; } = { "READY", "DISK", "PRINTER", "TAPE" };
        public string[] ReadyQueueTypes { get; } = { "READY" };
        public string[] IOQueueTypes { get; } = { "DISK", "PRINTER", "TAPE" };

        BindingList<SchedulerQueue> queues;
        private bool finishedIOOperation = false;

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
            { "DISK", "READY" },
            { "PRINTER", "READY" },
            { "TAPE", "READY" }
        };

        // Relates the process status (key) to the appropriate queue type for that process (value)
        public readonly Dictionary<string, string> queueBasedOnProcessStatus = new Dictionary<string, string>
        {
            { "NEW", "READY" },
            { "PREEMPTED", "READY" }
        };

        public FIFO(BindingList<SchedulerQueue> queues)
        {
            this.queues = queues;
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

        // Gets a process which can be executed by the cpu
        public Process? GetReadyProcess(BindingList<SchedulerQueue> queues)
        {
            Console.WriteLine("GetReadyProcess");
            SchedulerQueue queue;
            Process? readyProcess = null;

            foreach (string type in ReadyQueueTypes)
            {
                if (queues.Any(q => q.GetQueueType().Equals(type)))
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

        // Performs actions done by the scheduler automatically, such as updating state variables
        public void Run()
        {
            Console.WriteLine("Run");

            SimulateIOOperations();
            if (finishedIOOperation)
                UpdateQueues();
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
    }
}
