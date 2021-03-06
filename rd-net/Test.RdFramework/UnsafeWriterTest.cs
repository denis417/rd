﻿using System.Linq;
using System.Threading;
using JetBrains.Serialization;
using NUnit.Framework;

namespace Test.RdFramework
{
  [TestFixture]
  public class UnsafeWriterTest
  {
    [Test]
    public void TestWriterIsReused01()
    {
      UnsafeWriter firstWriter = null, secondWriter = null;
      var thread = new Thread(() =>
      {
        UnsafeWriter.AllowUnsafeWriterCaching = true;
        try
        {
          using (var cookie = UnsafeWriter.NewThreadLocalWriter())
          {
            firstWriter = cookie.Writer;
          }

          using (var cookie = UnsafeWriter.NewThreadLocalWriter())
          {
            secondWriter = cookie.Writer;
          }
        }
        finally
        {
          UnsafeWriter.AllowUnsafeWriterCaching = false;
        }
      });
      thread.Start();
      thread.Join();
      Assert.IsNotNull(firstWriter, "firstWriter != null");
      Assert.IsNotNull(secondWriter, "secondWriter != null");
      Assert.IsTrue(ReferenceEquals(firstWriter, secondWriter), "object.ReferenceEquals(firstWriter, secondWriter)");
    }

    [Test]
    public void TestWriterIsReused02()
    {
      UnsafeWriter firstWriter = null, secondWriter = null;
      var thread = new Thread(() =>
      {
        UnsafeWriter.AllowUnsafeWriterCaching = true;
        try
        {
          using (var cookie = UnsafeWriter.NewThreadLocalWriter())
          {
            firstWriter = cookie.Writer;
          }

          using (var cookie = UnsafeWriter.NewThreadLocalWriterNoCaching())
          {
            secondWriter = cookie.Writer;
          }

        }
        finally
        {
          UnsafeWriter.AllowUnsafeWriterCaching = false;
        }
      });

      thread.Start();
      thread.Join();
      Assert.IsNotNull(firstWriter, "firstWriter != null");
      Assert.IsNotNull(secondWriter, "secondWriter != null");
      Assert.IsTrue(ReferenceEquals(firstWriter, secondWriter), "object.ReferenceEquals(firstWriter, secondWriter)");
    }

    [Test]
    public void TestWriterIsNotReused01()
    {
      UnsafeWriter firstWriter = null, secondWriter = null;
      var thread = new Thread(() =>
      {
        using (var cookie = UnsafeWriter.NewThreadLocalWriterNoCaching())
        {
          firstWriter = cookie.Writer;
        }

        using (var cookie = UnsafeWriter.NewThreadLocalWriterNoCaching())
        {
          secondWriter = cookie.Writer;
        }

      });
      thread.Start();
      thread.Join();
      Assert.IsNotNull(firstWriter, "firstWriter != null");
      Assert.IsNotNull(secondWriter, "secondWriter != null");
      Assert.IsFalse(ReferenceEquals(firstWriter, secondWriter), "object.ReferenceEquals(firstWriter, secondWriter)");
    }

    [Test]
    public void TestWriterIsNotReused02()
    {
      UnsafeWriter firstWriter = null, secondWriter = null;
      var thread = new Thread(() =>
      {
        using (var cookie = UnsafeWriter.NewThreadLocalWriterNoCaching())
        {
          firstWriter = cookie.Writer;
        }

        using (var cookie = UnsafeWriter.NewThreadLocalWriter())
        {
          secondWriter = cookie.Writer;
        }
      });
      thread.Start();
      thread.Join();
      Assert.IsNotNull(firstWriter, "firstWriter != null");
      Assert.IsNotNull(secondWriter, "secondWriter != null");
      Assert.IsFalse(ReferenceEquals(firstWriter, secondWriter), "object.ReferenceEquals(firstWriter, secondWriter)");
    }

    [Test]
    public void TestReportAccessCounter()
    {
      var thread = new Thread(() =>
      {
        UnsafeWriterStatistics.ClearEvents();
        for (int i = 0; i < UnsafeWriterStatistics.ReportAccessCounterThreshold + UnsafeWriterStatistics.ReportOnOfN; ++i)
        {
          using (var cookie = UnsafeWriter.NewThreadLocalWriter())
          {
          }
        }
      });
      thread.Start();
      thread.Join();
      var accessCounterEvent = UnsafeWriterStatistics.GetEvents().First(@event => @event.Type == UnsafeWriterStatistics.EventType.ACCESS_COUNTER);
    }

    [Test]
    public void ReportAllocationOnNonCachedThread()
    {
      var thread = new Thread(() =>
      {
        UnsafeWriterStatistics.ClearEvents();
        using (var cookie = UnsafeWriter.NewThreadLocalWriter())
        {
          for (int i=0; i<UnsafeWriterStatistics.ReportAllocationOnNonCachedThreadThreshold+1; ++i)
            cookie.Writer.Write(0);
        }
      });
      thread.Start();
      thread.Join();
      var memoryAllocationEvent = UnsafeWriterStatistics.GetEvents().First(@event => @event.Type == UnsafeWriterStatistics.EventType.MEMORY_ALLOCATION);
    }

    [Test]
    public void ReportReentrancy01()
    {
      var thread = new Thread(() =>
      {
        UnsafeWriter.AllowUnsafeWriterCaching = true;
        UnsafeWriterStatistics.ClearEvents();
        using (var cookie = UnsafeWriter.NewThreadLocalWriter())
        {
          using (var nestedCookie = UnsafeWriter.NewThreadLocalWriter())
            cookie.Writer.Write(0);
        }
      });
      thread.Start();
      thread.Join();
      var reentrancyEvent = UnsafeWriterStatistics.GetEvents().FirstOrDefault(@event => @event.Type == UnsafeWriterStatistics.EventType.REENTRANCY);
      Assert.IsNull(reentrancyEvent);
    }

    [Test]
    public void ReportReentrancy02()
    {
      var thread = new Thread(() =>
      {
        UnsafeWriter.AllowUnsafeWriterCaching = true;
        UnsafeWriterStatistics.ClearEvents();
        UnsafeWriterStatistics.ReportReentrancy = true;
        try
        {
          using (var cookie = UnsafeWriter.NewThreadLocalWriter())
          {
            using (var nestedCookie = UnsafeWriter.NewThreadLocalWriter())
              cookie.Writer.Write(0);
          }
        }
        finally
        {
          UnsafeWriterStatistics.ReportReentrancy = false;
        }
      });
      thread.Start();
      thread.Join();
      var reentrancyEvent = UnsafeWriterStatistics.GetEvents().First(@event => @event.Type == UnsafeWriterStatistics.EventType.REENTRANCY);
      Assert.IsTrue(reentrancyEvent.Stacktraces.Count == 2, "reentrancyEvent.Stacktraces.Count == 2");
    }

    [Test]
    public void TestNullWriterInCookie()
    {
      using (var cookie = new UnsafeWriter.Cookie())
      {
      }
    }
  }
}