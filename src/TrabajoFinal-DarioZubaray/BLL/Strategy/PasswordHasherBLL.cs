using System;
using System.Collections.Generic;

namespace BLL.Strategy
{
    public class PasswordHasherBLL
    {
        private static readonly Lazy<PasswordHasherBLL> _default = new Lazy<PasswordHasherBLL>(() => new PasswordHasherBLL());

        private readonly IPasswordStrategyBLL _defaultStrategy;
        private readonly IReadOnlyList<IPasswordStrategyBLL> _strategies;

        public PasswordHasherBLL()
            : this(new BcryptPasswordStrategyBLL(), new IPasswordStrategyBLL[]
            {
                new BcryptPasswordStrategyBLL(),
                new LegacySha256PasswordStrategyBLL()
            })
        {
        }

        public PasswordHasherBLL(IPasswordStrategyBLL defaultStrategy, IReadOnlyList<IPasswordStrategyBLL> strategies)
        {
            _defaultStrategy = defaultStrategy;
            _strategies = strategies;
        }

        public static PasswordHasherBLL Default => _default.Value;

        public string Hash(string password)
        {
            return _defaultStrategy.Hash(password);
        }

        public bool Verify(string plain, string stored)
        {
            for (int i = 0; i < _strategies.Count; i++)
            {
                IPasswordStrategyBLL strategy = _strategies[i];
                if (strategy.Matches(stored))
                {
                    return strategy.Verify(plain, stored);
                }
            }

            return _defaultStrategy.Verify(plain, stored);
        }
    }
}
