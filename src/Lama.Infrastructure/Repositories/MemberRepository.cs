using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;
using Lama.Infrastructure.Data;
using Lama.Application.Repositories;

namespace Lama.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para Member
/// </summary>
public class MemberRepository(LamaDbContext context) : IMemberRepository
{
    private readonly LamaDbContext _context = context;

    public async Task<Member?> GetByIdAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .AsNoTracking()
            .Include(m => m.Vehicles)
            .Include(m => m.Attendances)
            .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);
    }

    public async Task<Member?> GetByOrderAsync(int order, CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Order == order, cancellationToken);
    }

    public async Task<IEnumerable<Member>> GetByChapterAsync(int chapterId, CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .AsNoTracking()
            .Where(m => m.ChapterId == chapterId && m.Status == "ACTIVE")
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Member>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .AsNoTracking()
            .Include(m => m.Vehicles)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Member>> SearchByNameAsync(string searchTerm, int take = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<Member>();
        }

        // Normalizar el término de búsqueda (uppercase, sin tildes)
        var normalizedTerm = NormalizeSearchTerm(searchTerm);

        // Query ejecutada en SQL Server con índice en CompleteNamesNormalized
        return await _context.Members
            .Where(m => m.CompleteNamesNormalized != null &&
                        m.CompleteNamesNormalized.Contains(normalizedTerm))
            .OrderBy(m => m.CompleteNames)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Normaliza texto para búsqueda: uppercase, sin tildes, sin espacios extra
    /// </summary>
    private static string NormalizeSearchTerm(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Remover tildes
        var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
        var chars = normalized.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
            != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray();

        return new string(chars)
            .ToUpperInvariant()
            .Trim();
    }

    public async Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        await _context.Members.AddAsync(member, cancellationToken);
    }

    public Task UpdateAsync(Member member, CancellationToken cancellationToken = default)
    {
        _context.Members.Update(member);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int memberId, CancellationToken cancellationToken = default)
    {
        var member = await GetByIdAsync(memberId, cancellationToken);
        if (member != null)
        {
            _context.Members.Remove(member);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
