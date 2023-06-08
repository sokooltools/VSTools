using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SokoolTools.VsTools
{
	//------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//------------------------------------------------------------------------------------------------------------------
	public static class EnumerableExtensions
	{
		[DebuggerStepThrough]
		public static IEnumerable<T> Recursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren)
		{
			IEnumerable<T> argumentValue = source.ToList();
			ArgumentNotNull(argumentValue, nameof(source));
			ArgumentNotNull(getChildren, nameof(getChildren));
			foreach (T obj in argumentValue)
				yield return obj;
			foreach (T source1 in argumentValue)
			{
				foreach (T obj in SearchBreadthFirst(source1, getChildren))
					yield return obj;
			}
		}

		[DebuggerStepThrough]
		private static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			IEnumerable<T> argumentValue = source.ToList();
			ArgumentNotNull(argumentValue, nameof(source));
			ArgumentNotNull(action, nameof(action));
			foreach (T obj in argumentValue)
				action(obj);
		}

		private static void ArgumentNotNull<T>(T argumentValue, string argumentName) where T : class
		{
			if (argumentValue == null)
				throw new ArgumentNullException(argumentName);
		}

		private static IEnumerable<T> SearchBreadthFirst<T>(T source, Func<T, IEnumerable<T>> getChildren)
		{
			if (source == null)
				yield break;
			var queue = new Queue<T>();
			AddChild(source);
			while (queue.Any())
			{
				T current = queue.Dequeue();
				if (current != null)
					yield return current;
				AddChild(current);
			}
			void AddChild(T item)
			{
				IEnumerable<T> source1 = getChildren(item);
				source1?.ForEach(queue.Enqueue);
			}
		}
	}
}
