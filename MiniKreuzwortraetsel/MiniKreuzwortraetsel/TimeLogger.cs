using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Media;

namespace TimeLogger
{
    public static class TimeLogger
    {
        static int number_of_iterations = 100;
        static int sets_of_iterations = 5;
        public enum TimeUnit { Millisecond = 1, Second = 1000};

        static public void Benchmark(Action benchmark_code, TimeUnit timeUnit)
        {
            int timeUnitDivisor = (int)timeUnit;
            // Log benchmark specs
            Debug.Write
            (
                "number_of_iterations: " + number_of_iterations + "\n" +
                "sets_of_iterations: " + sets_of_iterations + "\n" +
                "total iterations: " + (sets_of_iterations * number_of_iterations) + "\n" +
                "unit: " + timeUnit + "\n"
            );

            double sum_of_average_time_for_a_single_iteration = 0;
            // Run through the sets of iterations
            for (int i = 0; i < sets_of_iterations; i++)
            {

                // Start the stopwatch
                Stopwatch sw = Stopwatch.StartNew();

                // Run through the iterations
                for (int k = 0; k < number_of_iterations; k++)
                {
                    // Call the code that should be benchmarked
                    benchmark_code.DynamicInvoke();
                }

                // Stop the stopwatch
                sw.Stop();

                // Log average time for a single iteration
                double average_time_for_a_single_iteration = sw.ElapsedMilliseconds / (double)number_of_iterations / timeUnitDivisor;
                sum_of_average_time_for_a_single_iteration += average_time_for_a_single_iteration;
                Debug.WriteLine("average_single_iteration_time: " + Math.Round(average_time_for_a_single_iteration, 3));
            }

            // Log average of the average iteration times
            Debug.WriteLine("Average of the average iteration times: " + Math.Round(sum_of_average_time_for_a_single_iteration / sets_of_iterations, 3));

            // Play finish sound
            SoundPlayer simpleSound = new SoundPlayer(@"c:\Windows\Media\chimes.wav");
            simpleSound.Play();
        }
    }
}
