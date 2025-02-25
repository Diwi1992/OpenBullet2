﻿using RuriLib.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace RuriLib.Tests.Helpers
{
    public class PauseTokenSourceTests
    {
        [Fact]
        public async Task PauseIfRequestedAsync_Paused_Wait()
        {
            var pts = new PauseTokenSource();
            var sw = new Stopwatch();
            sw.Start();

            // Start a task that takes 1000 ms to complete
            var task = Task.Run(async () =>
            {
                // Loop 10 times (100 ms each) and pause on each iteration if needed
                for (var i = 0; i < 10; i++)
                {
                    await Task.Delay(100);
                    await pts.Token.PauseIfRequestedAsync();
                }
            });

            // Wait 300 ms, then pause the task for 1 second, then resume
            await Task.Delay(300);
            await pts.PauseAsync();
            await Task.Delay(1000);
            await pts.ResumeAsync();

            // Wait until the task completes
            await task.WaitAsync(TimeSpan.FromSeconds(5));

            sw.Stop();
            var elapsed = sw.Elapsed;

            // Make sure the elapsed time is over 1500 ms
            Assert.True(elapsed > TimeSpan.FromMilliseconds(1500));
        }
    }
}
