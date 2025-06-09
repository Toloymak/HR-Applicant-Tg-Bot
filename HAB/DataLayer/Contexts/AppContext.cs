using System.Reflection;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Contexts;

public class HrBotContext : DbContext
{
    public HrBotContext(DbContextOptions<HrBotContext> options)
        : base(options)
    {
    }

    public DbSet<VacancyDal> Vacations => Set<VacancyDal>();
    public DbSet<BotUserDal> BotUsers => Set<BotUserDal>();
    public DbSet<QuestionDal> Questions => Set<QuestionDal>();
    public DbSet<ConditionDal> Conditions => Set<ConditionDal>();
    public DbSet<UserApplicationDal> UserApplications => Set<UserApplicationDal>();
    public DbSet<AnswerDal> Answers => Set<AnswerDal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}