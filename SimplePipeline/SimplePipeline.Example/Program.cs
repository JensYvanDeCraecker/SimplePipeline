using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SimplePipeline.Example.Models;
using SimplePipeline.Example.Pipelines;

namespace SimplePipeline.Example
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            Int32 year = 2000;
            IPipeline<String, IEnumerable<Person>> pipeline = new PersonPipeline(year);
            if (pipeline.Execute(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\persons.json"))
            {
                Console.WriteLine($"Persons born in {year}");
                foreach (Person person in pipeline.Output)
                    Console.WriteLine($"Name: {person.FirstName} {person.LastName}");
            }
            else
            {
                Console.WriteLine(pipeline.Exception);
            }
            Console.ReadLine();
        }
    }
}