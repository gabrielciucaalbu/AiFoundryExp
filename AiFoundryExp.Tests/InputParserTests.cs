using AiFoundryExp;

namespace AiFoundryExp.Tests;

public class InputParserTests
{
    [Fact]
    public void ParseFile_SingleLine_ReturnsBusinessIdea()
    {
        string temp = Path.GetTempFileName();
        File.WriteAllText(temp, "My idea");
        try
        {
            var result = InputParser.ParseFile(temp);
            Assert.Equal("My idea", result["business_idea"]);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void ParseFile_KeyValuePairs()
    {
        string temp = Path.GetTempFileName();
        File.WriteAllLines(temp, new[]{"target_market: SMEs", "revenue_model=subscription"});
        try
        {
            var result = InputParser.ParseFile(temp);
            Assert.Equal("SMEs", result["target_market"]);
            Assert.Equal("subscription", result["revenue_model"]);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void ParseFile_Json()
    {
        string temp = Path.GetTempFileName();
        File.WriteAllText(temp, "{\"technology_preferences\":\".net\"}");
        try
        {
            var result = InputParser.ParseFile(temp);
            Assert.Equal(".net", result["technology_preferences"]);
        }
        finally
        {
            File.Delete(temp);
        }
    }
}
