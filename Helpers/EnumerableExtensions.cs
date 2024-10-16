using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
// ReSharper disable MemberCanBePrivate.Global

namespace SokoolTools.VsTools
{
	//------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//------------------------------------------------------------------------------------------------------------------
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Performs the specified Function of Type T on all the children of this enumerable of type T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="getChildren">An enumerable of all the children of each parent of type T.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Performs the specified Action of type T on all items of this enumerable of type T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="action">This enumerable.</param>
		[DebuggerStepThrough]
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
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
			var addChild = (Action<T>)(item =>
			{
				IEnumerable<T> source1 = getChildren(item);
				if (source1 == null)
					return;
				source1.ForEach(queue.Enqueue);
			});
			if (addChild == null) 
				throw new ArgumentNullException(nameof(addChild));
			addChild(source);
			while (queue.Any())
			{
				T current = queue.Dequeue();
				if (current != null)
					yield return current;
				addChild(current);
			}
		}
	}
}
