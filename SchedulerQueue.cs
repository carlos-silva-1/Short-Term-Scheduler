/*
 * Normal queue with the type of the queue added as a property.
 * This type is used to decide which processes should be enqueued into it.
 * The types of process appropriate for a given queue type are given in the file of the scheduling algorithm being used.
 */

using System.Collections.Specialized;
using System.ComponentModel;

namespace SchedulerSimulator
{
    public class SchedulerQueue : Queue<Process>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private string _queueType;
        private Process[] _processes;

        public string QueueType
        {
            get { return _queueType; }
            private set { _queueType = value; }
        }

        public Process[] Processes
        {
            get 
            {
                //UpdateProcesses();
                return _processes; 
            }
            set
            {
                _processes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Processes)));
            }
        }

        public void UpdateProcesses()
        {
            Processes = base.ToArray();
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public SchedulerQueue(string type) 
        {
                _queueType = type;
        }

        public new void Enqueue(Process p)
        {
            base.Enqueue(p);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, p));
        }

        public new Process Dequeue()
        {
            Process p = base.Dequeue();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, p, 0));
            return p;
        }

        public new void Clear()
        {
            base.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new bool TryDequeue(out Process p)
        {
            bool success = base.TryDequeue(out p);
            if (success)
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, p, 0));
            return success;
        }

        public string GetQueueType()
        {
            Console.WriteLine("GetQueueType");
            return _queueType;
        }

        public static SchedulerQueue GetQueue(BindingList<SchedulerQueue> queues, string queueType)
        {
            Console.WriteLine("GetQueue");
            SchedulerQueue queue;

            if (!queues.Any(q => q.GetQueueType().Equals(queueType)))
                queues.Add(new SchedulerQueue(queueType));

            queue = (from q in queues
                     where q.GetQueueType().Equals(queueType)
                     select q).First();

            return queue;
        }

        public void PrintQueue()
        {
            Console.WriteLine("PrintQueue");
            Console.WriteLine(_queueType);
            if (Count == 0)
                Console.WriteLine("Queue is empty");
            else
            {
                foreach (Process p in this)
                {
                    Console.WriteLine(p.GetID());
                }
            }
        }
    }
}

//! NEXT
// USE BASE'S TOARRAY METHOD TO CREATE A PROPERTY OF THE QUEUE OF PROCESSES IN EACH QUEUE 