using System.ComponentModel;

namespace SchedulerSimulator
{
    public class IO
    {
        public static readonly string[] IORequestTypes = { "DISK", "PRINTER", "TAPE" };

        public static readonly Dictionary<string, int> operationTime = new Dictionary<string, int>
        {
            { "DISK", 2 },
            { "PRINTER", 4 },
            { "TAPE", 3 }
        };

        public static BindingList<SchedulerQueue> GetIOQueues(BindingList<SchedulerQueue> queues)
        {
            Console.WriteLine("GetIOQueues");
            BindingList<SchedulerQueue> IOQueues = new BindingList<SchedulerQueue>((from q in queues
                                                                                    where IORequestTypes.Contains(q.GetQueueType())
                                                                                    select q).ToList());
            return IOQueues;
        }
    }
}
