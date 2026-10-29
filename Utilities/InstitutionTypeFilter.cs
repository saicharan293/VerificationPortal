

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;

public class InstitutionTypeFilter : IAsyncActionFilter
{
    private readonly ApplicationDbContext _context;

    public InstitutionTypeFilter(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.Controller as Controller;

        var collegeCode = context.HttpContext.Request.Query["collegeCode"].ToString();

        if (string.IsNullOrEmpty(collegeCode))
        {
            collegeCode = context.HttpContext.Session.GetString("CollegeCode");
        }

        if (!string.IsNullOrEmpty(collegeCode))
        {
            var institutionType = await (
                from institute in _context.AffInstitutionsDetails.AsNoTracking()
                join type in _context.MstInstitutionTypes.AsNoTracking()
                    on institute.TypeOfInstitution equals 
                    type.InstitutionTypeId.ToString()
                where institute.CollegeCode == collegeCode
                select type.InstitutionType
            ).FirstOrDefaultAsync();
            
            
            if(controller != null)
            {
                controller.ViewData["TypeOfInstitution"] = institutionType;
            }
        }

        await next();
    }
}