using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        int N = 4;
        int K = 17;

        RunTask1Tests(K);
        RunTask2Tests(K);
        Task3_QueueSimulation(N, K);
        Task4_CustomStackDemo(K);
        RunEdgeCases();
    }

    public static (bool isBalanced, int errorIndex) CheckBrackets(string input)
    {
        if (string.IsNullOrEmpty(input)) return (true, -1);

        Stack<(char ch, int index)> stack = new Stack<(char ch, int index)>();

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '(' || c == '[' || c == '{' || c == '<')
            {
                stack.Push((c, i));
            }
            else if (c == ')' || c == ']' || c == '}' || c == '>')
            {
                if (stack.Count == 0)
                {
                    return (false, i);
                }

                var top = stack.Pop();
                if (!IsMatchingPair(top.ch, c))
                {
                    return (false, i);
                }
            }
        }

        if (stack.Count > 0)
        {
            var firstUnmatched = stack.Pop();
            while (stack.Count > 0) firstUnmatched = stack.Pop();
            return (false, firstUnmatched.index);
        }

        return (true, -1);
    }

    private static bool IsMatchingPair(char open, char close)
    {
        return (open == '(' && close == ')') ||
               (open == '[' && close == ']') ||
               (open == '{' && close == '}') ||
               (open == '<' && close == '>');
    }

    static void RunTask1Tests(int K)
    {
        string[] tests = new string[]
        {
            "(a[b]{c})",
            "([)]",
            ")(",
            "(("
        };

        foreach (var t in tests)
        {
            var res = CheckBrackets(t);
            Console.WriteLine($"Рядок: \"{t}\" -> {(res.isBalanced ? "збалансований" : "незбалансований")}, індекс помилки: {res.errorIndex}");
        }

        string customUnbalanced = new string('(', K - 1) + "[]" + new string(')', K - 2);
        var customRes = CheckBrackets(customUnbalanced);
        Console.WriteLine($"Спеціальний тест: \"{customUnbalanced}\" -> {(customRes.isBalanced ? "збалансований" : "незбалансований")}, індекс помилки: {customRes.errorIndex}");
    }

    public static void EvaluateRPN(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            Console.WriteLine("Помилка: Порожній вираз!");
            return;
        }

        string[] tokens = expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        Stack<double> stack = new Stack<double>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else if (token == "+" || token == "-" || token == "*" || token == "/")
            {
                if (stack.Count < 2)
                {
                    Console.WriteLine($"Помилка у виразі \"{expression}\": Бракує чисел для операції '{token}'");
                    return;
                }

                double b = stack.Pop();
                double a = stack.Pop();

                if (token == "/" && b == 0)
                {
                    Console.WriteLine($"Помилка у виразі \"{expression}\": Ділення на нуль!");
                    return;
                }

                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/": stack.Push(a / b); break;
                }
            }
            else
            {
                Console.WriteLine($"Помилка у виразі \"{expression}\": Невідомий токен '{token}'");
                return;
            }
        }

        if (stack.Count == 1)
        {
            Console.WriteLine($"Вираз \"{expression}\" -> {stack.Pop()}");
        }
        else
        {
            Console.WriteLine($"Помилка у виразі \"{expression}\": Забагато чисел без операторів!");
        }
    }

    static void RunTask2Tests(int K)
    {
        string[] tests = new string[]
        {
            "3 4 + 2 *",
            "5 3 -",
            "8 2 /",
            "2 3 4 * +",
            "5 0 /"
        };

        foreach (var t in tests)
        {
            EvaluateRPN(t);
        }

        string myExpression = "20 5 - 2 / 10 +";
        EvaluateRPN(myExpression);
    }

    class Job
    {
        public int Id { get; set; }
        public int RemainingWork { get; set; }
        public int TotalWork { get; set; }
        public int StartTime { get; set; } = -1;
    }

    public static void Task3_QueueSimulation(int N, int K)
    {
        int jobCount = 8 + (K % 5);
        int quantum = (K % 3) + 2;

        Random rnd = new Random(N);
        Queue<Job> queue = new Queue<Job>();

        for (int i = 1; i <= jobCount; i++)
        {
            int work = rnd.Next(1, 10);
            queue.Enqueue(new Job { Id = i, TotalWork = work, RemainingWork = work });
        }

        int currentTime = 0;
        int maxQueueLength = queue.Count;

        while (queue.Count > 0)
        {
            if (queue.Count > maxQueueLength) maxQueueLength = queue.Count;

            Job currentJob = queue.Dequeue();

            if (currentJob.StartTime == -1)
            {
                currentJob.StartTime = currentTime;
            }

            int timeSpent = Math.Min(currentJob.RemainingWork, quantum);
            currentJob.RemainingWork -= timeSpent;
            currentTime += timeSpent;

            if (currentJob.RemainingWork > 0)
            {
                queue.Enqueue(currentJob);
            }
            else
            {
                Console.WriteLine($"Заявка №{currentJob.Id}: Вага={currentJob.TotalWork}, Початок={currentJob.StartTime} хв, Завершення={currentTime} хв");
            }
        }

        Console.WriteLine($"Максимальна довжина черги: {maxQueueLength}");
    }

    public class IntStack
    {
        private int[] _array;
        private int _count;

        public int Count => _count;
        public int Capacity => _array.Length;

        public IntStack(int initialCapacity)
        {
            _array = new int[initialCapacity];
            _count = 0;
        }

        public void Push(int x)
        {
            if (_count == _array.Length)
            {
                int[] newArray = new int[_array.Length * 2];
                for (int i = 0; i < _array.Length; i++)
                {
                    newArray[i] = _array[i];
                }
                _array = newArray;
            }
            _array[_count++] = x;
        }

        public int Pop()
        {
            if (_count == 0) throw new InvalidOperationException("Стек порожній!");
            return _array[--_count];
        }

        public int Peek()
        {
            if (_count == 0) throw new InvalidOperationException("Стек порожній!");
            return _array[_count - 1];
        }
    }

    static void Task4_CustomStackDemo(int K)
    {
        int initialCap = (K % 4) + 2;
        IntStack customStack = new IntStack(initialCap);

        int pushCount = (K % 4) + 3;
        for (int i = 1; i <= pushCount; i++)
        {
            customStack.Push(i * 10);
            Console.WriteLine($"Push({i * 10}) -> Count: {customStack.Count}, Capacity: {customStack.Capacity}");
        }

        string testStr = "(a[b]{c})";
        bool isBal = CheckBracketsCustomStack(testStr);
        Console.WriteLine($"Завдання 1 на власному стеку: {(isBal ? "збалансований" : "незбалансований")}");
    }

    public static bool CheckBracketsCustomStack(string input)
    {
        if (string.IsNullOrEmpty(input)) return true;
        IntStack stack = new IntStack(10);

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == '(' || c == '[' || c == '{' || c == '<')
            {
                stack.Push(c);
            }
            else if (c == ')' || c == ']' || c == '}' || c == '>')
            {
                if (stack.Count == 0) return false;
                char top = (char)stack.Pop();
                if (!IsMatchingPair(top, c)) return false;
            }
        }
        return stack.Count == 0;
    }

    static void RunEdgeCases()
    {
        CheckBrackets("");
        CheckBrackets("Hello world");
        EvaluateRPN("42");
        EvaluateRPN("10 0 /");

        try
        {
            IntStack s = new IntStack(2);
            s.Pop();
        }
        catch (InvalidOperationException)
        {
        }
    }
}