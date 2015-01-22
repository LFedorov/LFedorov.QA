using SimpleCrypto;

namespace Domain.Entities
{
    public class Password
    {
        private readonly ICryptoService _cryptoService = new PBKDF2();

        protected Password()
        {
        }

        public Password(string password)
            : this()
        {
            Salt = _cryptoService.GenerateSalt();
            Hash = _cryptoService.Compute(password);
        }

        public string Hash { get; private set; }
        public string Salt { get; private set; }

        public virtual bool IsVerified(string password)
        {
            var hashToCompare = _cryptoService.Compute(password, Salt);
            return _cryptoService.Compare(Hash, hashToCompare);
        }
    }
}
