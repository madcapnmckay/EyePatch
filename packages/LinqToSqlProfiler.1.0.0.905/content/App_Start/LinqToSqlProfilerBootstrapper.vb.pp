﻿Imports HibernatingRhinos.Profiler.Appender.LinqToSql

<assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.LinqToSqlProfilerBootstrapper), "PreStart")>
Namespace App_Start
	Public Class LinqToSqlProfilerBootstrapper
        Public Shared Sub PreStart()
            ' Initialize the profiler
			LinqToSqlProfiler.Initialize()

            ' You can also use the profiler in an offline manner.
            ' This will generate a file with a snapshot of all the LinqToSql activity in the application,
            ' which you can use for later analysis by loading the file into the profiler.
            ' Dim FileName as String = @"c:\profiler-log";
            ' LinqToSqlProfiler.InitializeOfflineProfiling(FileName)
        End Sub
    End Class
End Namespace

