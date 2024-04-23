using FluentAssertions;
using RetoScheduler.Enums;
using RetoScheduler.Extensions;

namespace RetoSchedulerTest.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void Should_Be_Next_Day_Of_Week_Execution()
        {
            var res1 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Monday);
            var res2 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Tuesday);
            var res3 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Wednesday);
            var res4 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Thursday);
            var res5 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Friday);
            var res6 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Saturday);
            var res7 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Sunday);
            res1.Should().Be(new DateTime(2024, 4, 15));
            res2.Should().Be(new DateTime(2024, 4, 16));
            res3.Should().Be(new DateTime(2024, 4, 10));
            res4.Should().Be(new DateTime(2024, 4, 11));
            res5.Should().Be(new DateTime(2024, 4, 12));
            res6.Should().Be(new DateTime(2024, 4, 13));
            res7.Should().Be(new DateTime(2024, 4, 14));
        }
        [Fact]
        public void Should_Be_Next_Date_With_Ordinal()
        {
            var monday1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Monday);
            var monday2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Monday);
            var monday3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Monday);
            var monday4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Monday);
            var monday5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Monday);

            var tuesday1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Tuesday);
            var tuesday2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Tuesday);
            var tuesday3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Tuesday);
            var tuesday4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Tuesday);
            var tuesday5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Tuesday);

            var wednesday1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Wednesday);
            var wednesday2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Wednesday);
            var wednesday3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Wednesday);
            var wednesday4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Wednesday);
            var wednesday5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Wednesday);

            var thursday1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Thursday);
            var thursday2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Thursday);
            var thursday3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Thursday);
            var thursday4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Thursday);
            var thursday5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Thursday);

            var friday1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Friday);
            var friday2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Friday);
            var friday3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Friday);
            var friday4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Friday);
            var friday5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Friday);

            var saturday1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Saturday);
            var saturday2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Saturday);
            var saturday3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Saturday);
            var saturday4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Saturday);
            var saturday5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Saturday);

            var sunday1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Sunday);
            var sunday2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Sunday);
            var sunday3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Sunday);
            var sunday4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Sunday);
            var sunday5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Sunday);

            var day1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Day);
            var day2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Day);
            var day3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Day);
            var day4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Day);
            var day5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Day);

            var weekDay1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.WeekDay);
            var weekDay2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.WeekDay);
            var weekDay3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.WeekDay);
            var weekDay4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.WeekDay);
            var weekDay5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.WeekDay);

            var weekendDay1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.WeekEndDay);
            var weekendDay2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.WeekEndDay);
            var weekendDay3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.WeekEndDay);
            var weekendDay4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.WeekEndDay);
            var weekendDay5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.WeekEndDay);

            monday1.Should().Be(new DateTime(2020, 1, 6));
            monday2.Should().Be(new DateTime(2020, 1, 13));
            monday3.Should().Be(new DateTime(2020, 1, 20));
            monday4.Should().Be(new DateTime(2020, 1, 27));
            monday5.Should().Be(new DateTime(2020, 1, 27));

            tuesday1.Should().Be(new DateTime(2020, 1, 7));
            tuesday2.Should().Be(new DateTime(2020, 1, 14));
            tuesday3.Should().Be(new DateTime(2020, 1, 21));
            tuesday4.Should().Be(new DateTime(2020, 1, 28));
            tuesday5.Should().Be(new DateTime(2020, 1, 28));

            wednesday1.Should().Be(new DateTime(2020, 1, 1));
            wednesday2.Should().Be(new DateTime(2020, 1, 8));
            wednesday3.Should().Be(new DateTime(2020, 1, 15));
            wednesday4.Should().Be(new DateTime(2020, 1, 22));
            wednesday5.Should().Be(new DateTime(2020, 1, 29));

            thursday1.Should().Be(new DateTime(2020, 1, 2));
            thursday2.Should().Be(new DateTime(2020, 1, 9));
            thursday3.Should().Be(new DateTime(2020, 1, 16));
            thursday4.Should().Be(new DateTime(2020, 1, 23));
            thursday5.Should().Be(new DateTime(2020, 1, 30));

            friday1.Should().Be(new DateTime(2020, 1, 3));
            friday2.Should().Be(new DateTime(2020, 1, 10));
            friday3.Should().Be(new DateTime(2020, 1, 17));
            friday4.Should().Be(new DateTime(2020, 1, 24));
            friday5.Should().Be(new DateTime(2020, 1, 31));

            saturday1.Should().Be(new DateTime(2020, 1, 4));
            saturday2.Should().Be(new DateTime(2020, 1, 11));
            saturday3.Should().Be(new DateTime(2020, 1, 18));
            saturday4.Should().Be(new DateTime(2020, 1, 25));
            saturday5.Should().Be(new DateTime(2020, 1, 25));

            sunday1.Should().Be(new DateTime(2020, 1, 5));
            sunday2.Should().Be(new DateTime(2020, 1, 12));
            sunday3.Should().Be(new DateTime(2020, 1, 19));
            sunday4.Should().Be(new DateTime(2020, 1, 26));
            sunday5.Should().Be(new DateTime(2020, 1, 26));

            day1.Should().Be(new DateTime(2020, 1, 1));
            day2.Should().Be(new DateTime(2020, 1, 2));
            day3.Should().Be(new DateTime(2020, 1, 3));
            day4.Should().Be(new DateTime(2020, 1, 4));
            day5.Should().Be(new DateTime(2020, 1, 31));

            weekDay1.Should().Be(new DateTime(2020, 1, 1));
            weekDay2.Should().Be(new DateTime(2020, 1, 2));
            weekDay3.Should().Be(new DateTime(2020, 1, 3));
            weekDay4.Should().Be(new DateTime(2020, 1, 6));
            weekDay5.Should().Be(new DateTime(2020, 1, 31));

            weekendDay1.Should().Be(new DateTime(2020, 1, 4));
            weekendDay2.Should().Be(new DateTime(2020, 1, 5));
            weekendDay3.Should().Be(new DateTime(2020, 1, 11));
            weekendDay4.Should().Be(new DateTime(2020, 1, 12));
            weekendDay5.Should().Be(new DateTime(2020, 1, 26));
        }
        [Fact]
        public void Should_Be_Next_Date_With_Ordina_And_Datel()
        {
            var monday1 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.First, KindOfDay.Monday);
            var monday2 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Second, KindOfDay.Monday);
            var monday3 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Third, KindOfDay.Monday);
            var monday4 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Fourth, KindOfDay.Monday);
            var monday5 = new DateTime(2020, 1, 1).NextKindOfDay(Ordinal.Last, KindOfDay.Monday);

        }
    }
}
