using Microsoft.AspNetCore.Identity;

public class UserNameCustomValidation : UserValidator<IdentityUser>
{
    public UserNameCustomValidation(IdentityErrorDescriber errors) : base(errors ?? new IdentityErrorDescriber()) { }

    public override async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
    {
        var result = await base.ValidateAsync(manager, user);

        var errors = result.Errors
            .Where(e => e.Code != "InvalidUserName") 
            .ToList();

        return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
    }
}
