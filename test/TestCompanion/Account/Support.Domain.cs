using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.TestCompanion.Account;

public static class Support
{
    public static class Domain
    {
        public static AccountName NewAccountName() => new(Wst.Tools.PosiBridge.TestCompanion.Support.NewFaker().Random.Replace("???_####"));

        public static Wst.Tools.PosiBridge.Domain.Account.Account NewAccount() =>
            new(AccountId.New(), Source.Support.Domain.NewSource(), NewAccountName());
    }
}
