using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using AjourBT.Domain.Concrete;

namespace AjourBT.Tests.Infrastructure
{
    [TestFixture]
    public class SchedulerTest
    {

    #region getTimeSpan
        [Test]
        public void getTimespan_eventTimeLesserThanNow_properTimespan()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(20,10,12);
            TimeSpan now = new TimeSpan(21,8,10);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(23, 02, 02), result);
        }

        [Test]
        public void getTimespan_eventTimeGreaterThanNow_properTimespan()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan (21, 8, 10);
            TimeSpan now = new TimeSpan(20, 10, 12);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(00, 57, 58), result);
        }

        [Test]
        public void getTimespan_eventTimeEqualToNow_properTimespan()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(20, 10, 12);
            TimeSpan now = new TimeSpan(20, 10, 12);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(1,0, 0, 0), result);
        }

        [Test]
        public void getTimespan_eventTimeGreaterThan24Hours_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(25, 10, 12);
            TimeSpan now = new TimeSpan(21, 8, 10);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12,00,00), result);
        }

        [Test]
        public void getTimespan_eventTime0_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(0, 0, -1);
            TimeSpan now = new TimeSpan(21, 8, 10);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12, 00, 00), result);
        }

        [Test]
        public void getTimespan_nowIsGreaterThan24Hours_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(20, 10, 12);
            TimeSpan now = new TimeSpan(28, 8, 10);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12, 00, 00), result);
        }

        [Test]
        public void getTimespan_nowIsLesserThan0_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(20, 10, 12);
            TimeSpan now = new TimeSpan(0, 0, -1);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12,00,00), result);
        }

        [Test]
        public void getTimespan_resultIsLesserThan1second_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(0, 0, 0, 0, 1);
            TimeSpan now = new TimeSpan(0, 0, 0);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12, 00, 00), result);
        }
    #endregion
    }
}
