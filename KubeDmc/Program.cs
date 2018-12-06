using System;
using System.Diagnostics;

namespace KubeDmc
{
    class Program
    {
        static void Main(string[] args)
        {

            // https://www.mankier.com/1/kubectl-cluster-info

            new Board().Execute();

            Console.WriteLine("Bye !");
        }
    }
}
