namespace SchedulerSimulator
{
    public class CPU
    {
        private Process currentProcess = new();
        private bool idle = true;

        public void Push(Process p)
        {
            Console.WriteLine("Push");
            if (idle)
            {
                currentProcess = p;
                idle = false;
            }
            else
                throw new Exception("CPU cannot get new process as it is already executing one.");
        }

        public Process Pop()
        {
            Console.WriteLine("Pop");
            if (!idle)
            {
                idle = true;
                return currentProcess;
            }
            else
                throw new Exception("CPU cannot return process as it is not executing any process.");
        }

        public Process Peek()
        {
            Console.WriteLine("Peek");
            if (!idle)
                return currentProcess;
            else
                throw new Exception("CPU cannot return process as it is not executing any process.");
        }

        public bool IsIdle()
        {
            Console.WriteLine("IsIdle");
            return idle;
        }

        public void SimulateProcessExecution()
        {
            Console.WriteLine("SimulateProcessExecution");
            if (!idle)
            {
                Console.WriteLine($"CPU executed process #{currentProcess.GetID()}");
                currentProcess.SimulateExecution();
            }
            else
                throw new Exception("CPU cannot execute process as it doesn't have a process.");
        }

        public bool FinishedExecutingProcess()
        {
            Console.WriteLine("FinishedExecutingProcess");
            if (!idle)
            {
                if(currentProcess.HasFinishedExecution())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
