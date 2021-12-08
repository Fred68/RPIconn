using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;	  

namespace RPIconn
	{
	public class DictionaryWithLock<DAT> // where DAT : default
		{

		Dictionary<string, DAT> dict;			// Generic dictionary

		public DictionaryWithLock()
			{
			dict = new Dictionary<string, DAT>();
			}

		/// <summary>
		/// Get value by key
		/// with lock
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="ok">if found</param>
		/// <returns>value or default for DAT</returns>
		public DAT Get(string key, out bool ok)
			{
			ok = false;
			DAT d;
			lock(dict)
				{
				if(dict.TryGetValue(key, out d))
					{
					ok = true;
					}
				else
					{
					d = default(DAT);
					}
				return d;
				}
			}

		/// <summary>
		/// Set value by key
		/// with lock
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="val">valore</param>
		/// <returns>true if done</returns>
		public bool Set(string key, DAT val)
			{
			bool ok = false;
			if(key != null)
				if(key != String.Empty)
					lock(dict)
						{
						dict[key] = val;
						ok = true;
						}
			return ok;
			}

		/// <summary>
		/// Indexer
		/// Using Get() e Set() with lock
		/// </summary>
		/// <param name="s">key</param>
		/// <returns></returns>
		public DAT this[string s]
			{
			get
				{
				DAT x;
				bool ok;
				x = Get(s, out ok);
				return x;
				}
			set
				{
				Set(s, value);
				}
			}

		/// <summary>
		/// Values collection property
		/// </summary>
		public Dictionary<string, DAT>.ValueCollection Values
			{
			get
				{ return dict.Values; }
			}
			
		/// <summary>
		/// Key collection property
		/// </summary>
		public Dictionary<string, DAT>.KeyCollection Keys
			{
			get
				{ return dict.Keys; }
			}

		/// <summary>
		/// ContainsKey method
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsKey(string key)
			{
			return dict.ContainsKey(key);
			}	

		/// <summary>
		/// Add method
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		public void Add (string key, DAT val)
			{
			dict.Add(key, val);
			}
		}
	}
