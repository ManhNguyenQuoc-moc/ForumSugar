// File: Repositories/CommentReportRepository.cs
using ForumSugar.Data;
using ForumSugar.Models.Entities;
using ForumSugar.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForumSugar.Repositories
{
    public class CommentReportRepository : GenericRepository<CommentReport>, ICommentReportRepository
    {
        private readonly AppDbContext _context;

        public CommentReportRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommentReport>> GetReportedCommentsWithDetailsAsync()
        {
            return await _context.CommentReports
                .Include(r => r.Comment)
                .Include(r => r.User)
                .ToListAsync();
        }
    }
}
