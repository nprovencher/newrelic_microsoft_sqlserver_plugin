using System;
using System.Linq;

using NUnit.Framework;

using NewRelic.Microsoft.SqlServer.Plugin.QueryTypes;

namespace NewRelic.Microsoft.SqlServer.Plugin
{
	[TestFixture]
	public class FileIoViewTests
	{
		[Test]
		public void Assert_that_actual_query_text_is_valid()
		{
			var fileIoView = new FileIoView();

			var queryLocator = new QueryLocator(null);
			var query = queryLocator.PrepareQueries(new[] {fileIoView.GetType()}).Single();

			var actual = fileIoView.ParameterizeQuery(query.CommandText, new[] {"master"}, null);
			Assert.That(actual, Is.StringContaining("WHERE a.database_id IN (DB_ID('master'))"));
		}

		[Test]
		public void Assert_that_excluded_databases_are_added_to_command_text()
		{
			var sql = string.Format("foo {0} bar", FileIoView.WhereToken);

			var actual = new FileIoView().ParameterizeQuery(sql, null, new[] {"tempdb", "master", "model", "msdb",});

			Assert.That(actual, Is.EqualTo("foo WHERE a.database_id NOT IN (DB_ID('tempdb'), DB_ID('master'), DB_ID('model'), DB_ID('msdb')) bar"));
		}

		[Test]
		public void Assert_that_included_databases_are_added_to_command_text()
		{
			var sql = string.Format("foo {0} bar", FileIoView.WhereToken);

			var actual = new FileIoView().ParameterizeQuery(sql, new[] {"BigData", "LittleData",}, null);

			Assert.That(actual, Is.EqualTo("foo WHERE a.database_id IN (DB_ID('BigData'), DB_ID('LittleData')) bar"));
		}

		[Test]
		public void Should_throw_when_replacement_token_is_missing_from_sql()
		{
			Assert.Throws<Exception>(() => new FileIoView().ParameterizeQuery("no token here", null, null));
		}
	}
}
