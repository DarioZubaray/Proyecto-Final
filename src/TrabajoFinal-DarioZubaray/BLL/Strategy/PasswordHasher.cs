using System;
using System.Collections.Generic;

namespace BLL.Strategy
{
    public class PasswordHasher
    {
        private static readonly Lazy<PasswordHasher> _default = new Lazy<PasswordHasher>(() => new PasswordHasher());

        private readonly IPasswordStrategy _defaultStrategy;
        private readonly IReadOnlyList<IPasswordStrategy> _strategies;

        public PasswordHasher()
            : this(new BcryptPasswordStrategy(), new IPasswordStrategy[]
            {
                new BcryptPasswordStrategy(),
                new LegacySha256PasswordStrategy()
            })
        {
        }

        public PasswordHasher(IPasswordStrategy defaultStrategy, IReadOnlyList<IPasswordStrategy> strategies)
        {
            _defaultStrategy = defaultStrategy;
            _strategies = strategies;
        }

        public static PasswordHasher Default => _default.Value;

        public string Hash(string password)
        {
            return _defaultStrategy.Hash(password);
        }

        public bool Verify(string plain, string stored)
        {
            for (int i = 0; i < _strategies.Count; i++)
            {
                IPasswordStrategy strategy = _strategies[i];
                if (strategy.Matches(stored))
                {
                    return strategy.Verify(plain, stored);
                }
            }

            return _defaultStrategy.Verify(plain, stored);
        }
    }
}
