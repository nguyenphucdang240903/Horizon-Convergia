using DataAccessObjects.Data;
using Repositories.Interfaces;

namespace Repositories
{
    public class TokenRepository : GenericRepository<Token>, ITokenRepository
    {
        public TokenRepository(AppDbContext context) : base(context) { }
    }
}
