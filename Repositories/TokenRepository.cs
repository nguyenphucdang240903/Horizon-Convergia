using BusinessObjects.Models;
using DataAccessObjects.Data;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TokenRepository : GenericRepository<Token>, ITokenRepository
    {
        public TokenRepository(AppDbContext context) : base(context) { }
    }
}
