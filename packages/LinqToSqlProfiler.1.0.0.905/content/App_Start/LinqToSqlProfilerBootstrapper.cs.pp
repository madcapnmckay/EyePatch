﻿using HibernatingRhinos.Profiler.Appender.LinqToSql;

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.LinqToSqlProfilerBootstrapper), "PreStart")]
namespace $rootnamespace$.App_Start
{
	public static class LinqToSqlProfilerBootstrapper
	{
		public static void PreStart()
		{
			// Initialize the profiler
			LinqToSqlProfiler.Initialize();
			
			// You can also use the profiler in an offline manner.
			// This will generate a file with a snapshot of all the LinqToSql activity in the application,
			// which you can use for later analysis by loading the file into the profiler.
			// var filename = @"c:\profiler-log";
			// LinqToSqlProfiler.InitializeOfflineProfiling(filename);
		}
	}
}

