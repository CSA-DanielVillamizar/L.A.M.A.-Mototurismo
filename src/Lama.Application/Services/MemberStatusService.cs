using Lama.Domain.Entities;
using Lama.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Lama.Application.Services;

/// <summary>
/// Servicio para gestionar tipos de estados de miembros
/// Proporciona acceso a la lista de estados disponibles para dropdowns en COR
/// </summary>
public interface IMemberStatusService
{
    /// <summary>
    /// Obtiene todos los tipos de estado ordenados para mostrar en dropdown
    /// </summary>
    /// <returns>Lista de tipos de estado disponibles</returns>
    Task<IEnumerable<MemberStatusType>> GetAllStatusTypesAsync();

    /// <summary>
    /// Obtiene tipos de estado filtrados por categoría
    /// </summary>
    /// <param name="category">Categoría (CHAPTER, REGIONAL_OFFICER, etc.)</param>
    /// <returns>Tipos de estado de la categoría especificada</returns>
    Task<IEnumerable<MemberStatusType>> GetStatusTypesByCategoryAsync(string category);

    /// <summary>
    /// Obtiene un tipo de estado específico por nombre
    /// </summary>
    /// <param name="statusName">Nombre del estado</param>
    /// <returns>El tipo de estado, null si no existe</returns>
    Task<MemberStatusType?> GetStatusTypeByNameAsync(string statusName);

    /// <summary>
    /// Obtiene todas las categorías de estados disponibles
    /// </summary>
    /// <returns>Lista de nombres de categorías únicas</returns>
    Task<IEnumerable<string>> GetAllCategoriesAsync();
}

/// <summary>
/// Implementación del servicio de tipos de estados
/// Inyecta ILamaDbContext (abstracción) en lugar de LamaDbContext (implementación concreta)
/// </summary>
public class MemberStatusService : IMemberStatusService
{
    private readonly ILamaDbContext _context;

    public MemberStatusService(ILamaDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene todos los tipos de estado ordenados por DisplayOrder
    /// </summary>
    public async Task<IEnumerable<MemberStatusType>> GetAllStatusTypesAsync()
    {
        return await _context.MemberStatusTypes
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene tipos de estado filtrados por categoría, ordenados por DisplayOrder
    /// </summary>
    public async Task<IEnumerable<MemberStatusType>> GetStatusTypesByCategoryAsync(string category)
    {
        return await _context.MemberStatusTypes
            .Where(s => s.Category == category)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un tipo de estado específico por nombre exacto
    /// </summary>
    public async Task<MemberStatusType?> GetStatusTypeByNameAsync(string statusName)
    {
        return await _context.MemberStatusTypes
            .FirstOrDefaultAsync(s => s.StatusName == statusName);
    }

    /// <summary>
    /// Obtiene todas las categorías de estados disponibles
    /// </summary>
    public async Task<IEnumerable<string>> GetAllCategoriesAsync()
    {
        return await _context.MemberStatusTypes
            .Select(s => s.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
}
