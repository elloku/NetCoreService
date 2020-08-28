using System;
using System.Collections.Generic;
using System.Linq;

namespace Mediinfo.Infrastructure.Core
{
    public static class ParallelExtension
    {
        public static void ForEachParallel<T>(this IList<T> list, Action<T> action)
        {
            list.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount*5)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism).ForAll(action);
        }
    }
}
