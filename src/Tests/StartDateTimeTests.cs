using System.Text;
using BriefingRoom4DCS.Generator.Mission;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;

namespace BriefingRoom4DCS.Tests;

[Collection("Database collection")]
public class StartDateTimeTests
{
    private readonly DatabaseFixture fixture;

    public StartDateTimeTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void ExplicitStartDateTimeSetsMissionValues()
    {
        var template = new MissionTemplate(fixture.Db)
        {
            ContextDecade = Decade.Decade2020,
            StartDateTimeYear = 2026,
            StartDateTimeMonth = 2,
            StartDateTimeDay = 28,
            StartDateTimeHour = 18,
            StartDateTimeMinute = 30
        };
        var templateRecord = new MissionTemplateRecord(fixture.Db, template);
        var mission = new DCSMission(fixture.Db, "en", templateRecord);

        var month = Temporal.GenerateMissionDate(ref mission);
        Temporal.GenerateMissionTime(ref mission, month);

        Assert.Equal("28", mission.GetValue("DateDay"));
        Assert.Equal("2", mission.GetValue("DateMonth"));
        Assert.Equal("2026", mission.GetValue("DateYear"));
        Assert.Equal("28/02/2026", mission.GetValue("BriefingDate"));
        Assert.Equal("18:30", mission.GetValue("BriefingTime"));
        Assert.Equal((18 * 3600 + 30 * 60).ToString(), mission.GetValue("StartTime"));
    }

    [Fact]
    public void StartDateTimeRoundTripsThroughTemplateIni()
    {
        var template = new MissionTemplate(fixture.Db)
        {
            ContextDecade = Decade.Decade2020,
            StartDateTimeYear = 2026,
            StartDateTimeMonth = 7,
            StartDateTimeDay = 14,
            StartDateTimeHour = 0,
            StartDateTimeMinute = 5
        };

        var loaded = new MissionTemplate(fixture.Db);
        loaded.LoadFromString(Encoding.ASCII.GetString(template.GetIniBytes()));

        Assert.Equal(2026, loaded.StartDateTimeYear);
        Assert.Equal(7, loaded.StartDateTimeMonth);
        Assert.Equal(14, loaded.StartDateTimeDay);
        Assert.Equal(0, loaded.StartDateTimeHour);
        Assert.Equal(5, loaded.StartDateTimeMinute);
    }

    [Fact]
    public void StartDateTimeDayIsLimitedByMonthAndLeapYear()
    {
        var template = new MissionTemplate(fixture.Db)
        {
            ContextDecade = Decade.Decade2020,
            StartDateTimeYear = 2024,
            StartDateTimeMonth = 2,
            StartDateTimeDay = 29
        };

        Assert.True(template.HasStartDate);

        template.StartDateTimeYear = 2026;
        Assert.Equal(28, template.StartDateTimeDay);
        Assert.True(template.HasStartDate);

        template.StartDateTimeMonth = 4;
        template.StartDateTimeDay = 31;
        Assert.Equal(30, template.StartDateTimeDay);
    }

    [Fact]
    public void StartDateTimeYearIsLimitedByTimePeriod()
    {
        var template = new MissionTemplate(fixture.Db)
        {
            ContextDecade = Decade.Decade1940
        };

        Assert.Equal(1940, template.GetStartDateTimeFirstYear());
        Assert.Equal(1949, template.GetStartDateTimeLastYear());

        template.StartDateTimeYear = 1950;
        Assert.Equal(StartDateTimeSettings.DisabledDatePart, template.StartDateTimeYear);

        template.StartDateTimeYear = 1949;
        Assert.Equal(1949, template.StartDateTimeYear);

        template.ContextDecade = Decade.Decade2020;
        Assert.Equal(StartDateTimeSettings.DisabledDatePart, template.StartDateTimeYear);
    }
}
