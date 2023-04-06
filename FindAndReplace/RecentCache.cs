using System;
using System.Collections.Specialized;
using Microsoft.Win32;
using SokoolTools.VsTools.FindAndReplace.Common;

namespace SokoolTools.VsTools.FindAndReplace
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Enumeration defining whether the cache type comes from a find, replace, or directory.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public enum CacheType
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Find = 0
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		Find,
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Replace = 1
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		Replace,
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Scope = 2
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		Directory
	}

	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	public class RecentCache
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the cache key.
		/// </summary>
		/// <param name="cacheType">Type of cache (e.g. Find, Replace, or Directory).</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static RegistryKey GetCacheKey(CacheType cacheType)
		{
			string keyName;
			switch (cacheType)
			{
				case CacheType.Find:
					keyName = Info.REG_FIND_KEY;
					break;
				case CacheType.Replace:
					keyName = Info.REG_REPLACE_KEY;
					break;
				case CacheType.Directory:
					keyName = Info.REG_DIRECTORY_KEY;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null);
			}

			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyName, true) ??
									  Registry.CurrentUser.CreateSubKey(keyName);
			return registryKey;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the recent list for the specified cache type.
		/// </summary>
		/// <param name="cacheType">Type of the cache.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static StringCollection GetRecentList(CacheType cacheType)
		{
			StringCollection cacheList;
			using (RegistryKey registryKey = GetCacheKey(cacheType))
			{
				cacheList = new StringCollection();
				for (int i = 1; i <= Info.MAX_CACHE_COUNT; i++)
				{
					string cache = registryKey.GetValue(cacheType.ToString() + i) as string;
					if (!String.IsNullOrEmpty(cache))
						cacheList.Add(cache);
				}
			}
			return cacheList;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the specified content to the cache.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="cacheType">Type of the cache.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public static void Add(string content, CacheType cacheType)
		{
			using (RegistryKey registryKey = GetCacheKey(cacheType))
			{
				StringCollection cacheList = GetRecentList(cacheType);
				if (!cacheList.Contains(content))
				{
					if (cacheList.Count == Info.MAX_CACHE_COUNT)
						cacheList.RemoveAt(0);
					cacheList.Add(content);
				}
				for (int i = 1; i <= cacheList.Count; i++)
					registryKey.SetValue(cacheType.ToString() + i, cacheList[i - 1]);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Clears the recent list.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public static void ClearRecentList()
		{
			using (RegistryKey registryKey = GetCacheKey(CacheType.Find))
			{
				foreach (string name in registryKey.GetValueNames())
					registryKey.DeleteValue(name);
			}
			using (RegistryKey registryKey = GetCacheKey(CacheType.Replace))
			{
				foreach (string name in registryKey.GetValueNames())
					registryKey.DeleteValue(name);
			}
			using (RegistryKey registryKey = GetCacheKey(CacheType.Directory))
			{
				foreach (string name in registryKey.GetValueNames())
					registryKey.DeleteValue(name);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the cache pattern.
		/// </summary>
		/// <param name="cacheType">Type of the cache.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static string GetCachePattern(CacheType cacheType, int index)
		{
			return GetRecentList(cacheType)[GetRecentList(cacheType).Count - 1 - index];
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the specified cache type is cached.
		/// </summary>
		/// <param name="cacheType">Type of the cache.</param>
		/// <param name="pattern">The pattern.</param>
		/// <returns><c>true</c> if the specified cache type is cached; otherwise, <c>false</c>.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public static bool IsCached(CacheType cacheType, string pattern)
		{
			return GetRecentList(cacheType).Contains(pattern);
		}
	}
}