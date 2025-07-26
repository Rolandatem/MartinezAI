using MartinezAI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MartinezAI.Data;

public class StoredProcedures(
    MartinezAIDbContext _context)
{
    public async Task<string?> GetUserPasswordAsync(
        string email)
    {
        StringScalarResult? result = await _context.StringScalarResults
            .FromSqlInterpolated(
                $"SELECT \"GetUserPassword\"({email}) as \"Value\"")
            .SingleOrDefaultAsync();

        return result?.Value ?? null;
    }
}
