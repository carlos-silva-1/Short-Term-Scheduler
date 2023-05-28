namespace SchedulerSimulator
{
    internal class Util
    {
        public static List<T> NewList<T>(int length) where T : new()
        {
            List<T> list = new List<T>(length);
            for (int i = 0; i < length; i++)
            {
                list.Add(new T());
            }
            return list;
        }

        public static void PrintColoredMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
