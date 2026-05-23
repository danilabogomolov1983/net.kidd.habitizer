using LanguageExt;
using LanguageExt.Common;

namespace Wst.Tools.PosiBridge.Shared.Kernel.Functional;

public static class FinExtensions
{
    extension<A>(Fin<A> that)
    {
        public Fin<Unit> ToUnit()
        {
            return that.Bind<Unit>(_ => Unit.Default);
        }

        public Fin<A> MapError(Func<Error> mappedError)
        {
            var newError = mappedError();
            return that.BiBind(
                a => a,
                err => Fin<A>.Fail(Error.New(newError.Message, err)));
        }

        public Fin<TOut> Try<TOut>(Func<A, TOut> func)
        {
            return that.Bind(a =>
            {
                try
                {
                    return func(a) ?? Fin<TOut>.Fail(Error.New($"function execution on failed. that={a}"));
                }
                catch (Exception e)
                {
                    return Error.New($"Error trying to execute a function {nameof(func)},{func.Method.Name}. that={a}",
                        e);
                }
            });
        }

        public Fin<Unit> Try(Action<A> action)
        {
            return that.Bind(a =>
            {
                try
                {
                    action(a);
                    return Fin<Unit>.Succ(Unit.Default);
                }
                catch (Exception e)
                {
                    return Error.New(
                        $"Error trying to execute an action. {nameof(action)},{action.Method.Name}. that={a}",
                        e.Message);
                }
            });
        }
    }
}

