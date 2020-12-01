using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Test {
    class Program {
        public static void Main(string[] args) {
            Console.WriteLine("Print Fibonacci sequence");
            Console.WriteLine("Input n:");
            string num = Console.ReadLine();
            int count;
            if(!int.TryParse(num, out count)) {
                throw new ArgumentException(num + " is unlawful");
            }
            var sequence = FibonacciSequence(count);
            foreach(int v in sequence) {
                Console.Write(v);
                Console.Write(',');
            }
	    Console.WriteLine("");
        }

        private static int FibonacciN(int n) {
            if(n == 0) {
                return 0;
            }
            else if(n == 1) {
                return 1;
            }
            else {
                return FibonacciN(n - 1) + FibonacciN(n - 2);
            }
        }

        private static IEnumerable<int> FibonacciSequence(int n) {
            for(int i = 0; i < n; ++i) {
                yield return FibonacciN(i);
            }
        }
    }
}
