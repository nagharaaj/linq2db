﻿using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;

using NUnit.Framework;

namespace Tests.Linq
{
	using Model;

	[TestFixture]
	public class AsyncTest : TestBase
	{
		[Test, DataContextSource(false)]
		public async void Test(string context)
		{
			Test1(context);

			using (var db = GetDataContext(context + ".LinqService"))
			{
				var list = await db.Parent.ToArrayAsync();
				Assert.That(list.Length, Is.Not.EqualTo(0));
			}
		}

		[Test, DataContextSource(false)]
		public void Test1(string context)
		{
			using (var db = GetDataContext(context + ".LinqService"))
			{
				var list = db.Parent.ToArrayAsync().Result;
				Assert.That(list.Length, Is.Not.EqualTo(0));
			}
		}

		// IT : # async test.
		[Test, DataContextSource]
		public async void TestForEach(string context)
		{
			using (var db = GetDataContext(context))
			{
				var list = new List<Parent>();

				await db.Parent.ForEachAsync(list.Add);

				Assert.That(list.Count, Is.Not.EqualTo(0));
			}
		}

		[Test, DataContextSource(false)]
		public async void TestExecute1(string context)
		{
			using (var conn = new TestDataConnection(context))
			{
				var sql = conn.Person.Where(p => p.ID == 1).Select(p => p.Name).Take(1).ToString().Replace("-- Access", "");

				var res = await conn.SetCommand(sql).ExecuteAsync<string>();

				Assert.That(res, Is.EqualTo("John"));
			}
		}

		[Test, DataContextSource(false)]
		public void TestExecute2(string context)
		{
			using (var conn = new TestDataConnection(context))
			{
				var sql = conn.Person.Where(p => p.ID == 1).Select(p => p.Name).Take(1).ToString().Replace("-- Access", "");

				var res = conn.SetCommand(sql).ExecuteAsync<string>().Result;

				Assert.That(res, Is.EqualTo("John"));
			}
		}

		[Test, DataContextSource(false)]
		public async void TestQueryToArray(string context)
		{
			using (var conn = new TestDataConnection(context))
			{
				var sql = conn.Person.Where(p => p.ID == 1).Select(p => p.Name).Take(1).ToString().Replace("-- Access", "");

				using (var rd = await conn.SetCommand(sql).ExecuteReaderAsync())
				{
					var list = await rd.QueryToArrayAsync<string>();

					Assert.That(list[0], Is.EqualTo("John"));
				}
			}
		}
	}
}
