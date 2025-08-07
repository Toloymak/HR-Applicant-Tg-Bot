using System.Data;
using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Models.HrUsers;

namespace ApiCore.Services;

public class HrUserRepository
{
    private readonly HrBotContext _context;

    public HrUserRepository(HrBotContext context)
    {
        _context = context;
    }

    public async Task<int> CreateHrUser(
        CreateHrUserRequest user,
        CancellationToken ct)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.ReadUncommitted, ct);

        var botUser = await _context
            .BotUsers
            .Where(x => x.TgName == user.TgName)
            .FirstOrDefaultAsync(ct);

        if (botUser == null)
        {
            botUser = new BotUserDal
            {
                Id = Guid.CreateVersion7(),
                TgId = null,
                TgName = user.TgName
            };

            _context.BotUsers.Add(botUser);
            await _context.SaveChangesAsync(ct);
        }

        var newHr = new HrUserDal
        {
            BotUserId = botUser.Id,
            Alias = user.Alias,
            Position = user.Position
        };
        _context.HrUsers.Add(newHr);
        await _context.SaveChangesAsync(ct);
        

        await transaction.CommitAsync(ct);

        return newHr.Id;
    }

    public async Task<PaginationResult<HrUserListItemDal>> GetList(
        Pagination pagination,
        string search,
        CancellationToken ct)
    {
        var pageNumber = pagination.PageNumber < 1 ? 1 : pagination.PageNumber;
        var pageSize = pagination.PageSize < 1 ? 10 : pagination.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        
        var query = _context
                .HrUsers
                .Select(x => new HrUserListItemDal
                {
                    Id = x.Id,
                    Alias = x.Alias,
                    TgName = x.BotUser.TgName,
                    Position = x.Position
                })
                .Where(x => x.Alias.Contains(search)
                            || x.TgName.Contains(search)
                            || x.Position.Contains(search))
            ;

        var totalCount = await query.CountAsync(ct);
        var data = await query
            .Skip(skip)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<HrUserListItemDal>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber
        };
    }

    public async Task<HrUserListItemDal?> GetByTgName(
        string tgName, CancellationToken ct)
    {
        var user = await _context
            .HrUsers
            .Where(x => x.BotUser.TgName == tgName)
            .Select(x => new HrUserListItemDal
            {
                Id = x.Id,
                Alias = x.Alias,
                TgName = x.BotUser.TgName,
                Position = x.Position
            })
            .FirstOrDefaultAsync(ct);

        return user;
    }
}