using System.Runtime.Serialization;

namespace Wst.Tools.PosiBridge.Persistence.Test;

public class EnumUtilTests
{
    [Theory]
    [InlineData(TestEnum.Start)]
    [InlineData(TestEnum.Failed)]
    [InlineData(TestEnum.Finished)]
    [InlineData(TestEnum.Processing)]
    public void ToEnum_ToEnumString(TestEnum expected)
    {
        var converted = EnumUtil.ToEnumString(expected);
        Assert.NotNull(converted);
        var actual = EnumUtil.ToEnum<TestEnum>(converted);

        Assert.Equal(expected, actual);
    }

    public enum TestEnum
    {
        [EnumMember(Value = "START")] Start,
        [EnumMember(Value = "FAILED")] Failed,
        [EnumMember(Value = "FINISHED")] Finished,
        [EnumMember(Value = "PROCESSING")] Processing
    }
}
