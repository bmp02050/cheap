using cheap.Entities;
using Microsoft.EntityFrameworkCore;

namespace cheap.Services;

public interface IUserPreferenceService
{
    Task<UserPreference> Update(UserPreference userPreference);
}

public class UserPreferenceService : IUserPreferenceService
{
    private readonly Context _context;

    public UserPreferenceService(Context context)
    {
        _context = context;
    }

    public async Task<UserPreference> Update(UserPreference userPreference)
    {
        var userPreferences =
            await _context.UserPreferences.FirstOrDefaultAsync(x =>
                x.Id == userPreference.Id && x.UserId == userPreference.UserId);
        if (userPreferences is null)
        {
            var user = await _context.Users.FindAsync(userPreference.UserId);
            if (user is null)
                throw new Exception("User does not exist");
            await _context.UserPreferences.AddAsync(userPreference);
        }
        else
        {
            foreach (var propertyInfo in userPreference.GetType().GetProperties())
            {
                // Check if the property is not null
                var newValue = propertyInfo.GetValue(userPreference);
                var currentValue = propertyInfo.GetValue(userPreferences);

                if (newValue != null && !newValue.Equals(currentValue))
                {
                    propertyInfo.SetValue(userPreferences, newValue);
                }
            }
        }

        await _context.SaveChangesAsync();
        return userPreference;
    }
}