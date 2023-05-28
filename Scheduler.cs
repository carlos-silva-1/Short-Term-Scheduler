/*
 * Contains queues to manage the processes in the system. 
 * Schedules processes into appropriate queues depending on the status of the process. 
 * There are queues for processes that are ready for execution and queues for processes that are waiting for access to an IO peripheral. 
 */

using System.ComponentModel;

namespace SchedulerSimulator
{
    public class Scheduler
    {
        SchedulingAlgorithm algorithm;
        BindingList<SchedulerQueue> queues;

        public Scheduler(SchedulingAlgorithm algorithm, BindingList<SchedulerQueue> queues)
        {
            this.algorithm = algorithm;
            this.queues = queues;
        }

        public void Schedule(List<Process> processes)
        {
            algorithm.Schedule(processes, queues);
        }

        public void Schedule(Process p)
        {
            algorithm.Schedule(p, queues);
        }

        public Process? GetReadyProcess()
        {
            return algorithm.GetReadyProcess(queues);
        }

        public void RunSchedulingAlgorithm()
        {
            algorithm.Run(); 
        }

        public void PrintQueues()
        {
            Console.WriteLine("Queues:");
            foreach (SchedulerQueue queue in queues)
                queue.PrintQueue();
        }
    }
}
