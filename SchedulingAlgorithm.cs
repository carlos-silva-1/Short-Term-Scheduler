using System.ComponentModel;

namespace SchedulerSimulator
{
    public interface SchedulingAlgorithm
    {
        public string[] QueueTypes { get; }
        public string[] ReadyQueueTypes { get; }
        public string[] IOQueueTypes { get; }

        public abstract void Schedule(List<Process> processes, BindingList<SchedulerQueue> queues);

        public abstract void Schedule(Process process, BindingList<SchedulerQueue> queues);

        // Gets a process which can be executed by the cpu
        public abstract Process? GetReadyProcess(BindingList<SchedulerQueue> queues);

        // Performs actions done by the scheduler automatically, such as updating state variables
        public abstract void Run();
    }
}
